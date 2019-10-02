using UnityEngine;
using System.Collections;

public class GameService : MonoBehaviour
{
    private static GameService _GameService;
    public static GameService Instance { get { return _GameService; } }

    private void Awake()
    {
        _GameService = this;
    }

    public delegate void InternalWaitCallback();
    private InternalWaitCallback _internalWait;
    private InternalWaitCallback _debounceWait;
    public delegate void AsyncForEachCallback(int index);
    private AsyncForEachCallback _asyncForEach;
    private float _seconds;
    private int _lenght;
    private int _index;
    private bool _waitOneFrame;

    public void InternalWait(InternalWaitCallback internalWait, float? seconds = null)
    {
        if (seconds == null)
        {
            _waitOneFrame = true;
        }
        else
        {
            _seconds = seconds.Value;
        }
        _internalWait = internalWait;
        StartCoroutine(InternalWaitFunction());
    }

    private IEnumerator InternalWaitFunction()
    {
        if (_waitOneFrame)
        {
            _waitOneFrame = false;
            yield return 0;
        }
        else
        {
            yield return new WaitForSeconds(_seconds);
        }
        _internalWait();
    }

    public void Debounce(InternalWaitCallback debounceWait, float seconds)
    {
        Debug.Log("debounced");
        _debounceWait = debounceWait;
        StartCoroutine(DebounceFunction(seconds));
    }

    private IEnumerator DebounceFunction(float seconds)
    {
        if (_waitOneFrame)
        {
            _waitOneFrame = false;
            yield return 0;
        }
        else
        {
            yield return new WaitForSeconds(seconds);
        }
        _debounceWait();
    }

    public void AsyncForEach(int length, AsyncForEachCallback asyncForEach, float? seconds = null)
    {
        if (seconds == null)
        {
            _waitOneFrame = true;
        }
        else
        {
            _seconds = seconds.Value;
        }
        _lenght = length;
        _asyncForEach = asyncForEach;
        _index = 0;
        StartCoroutine(DoAsyncIteration());
    }

    IEnumerator DoAsyncIteration()
    {
        if (_waitOneFrame)
        {
            _waitOneFrame = false;
            yield return 0;
        }
        else
        {
            yield return new WaitForSeconds(_seconds);
        }

        _asyncForEach(_index);
        if (_index < _lenght - 1)
        {
            _index++;
            StartCoroutine(DoAsyncIteration());
        }
    }



    public bool CanAddNewElement(ElementType elementType, ElementType lastElementType)
    {
        switch (elementType)
        {
            case ElementType.SceneHeading:
                return true;
            case ElementType.Action:
                if (lastElementType == ElementType.Character)
                    return false;
                return true;
            case ElementType.Character:
                return true;
            case ElementType.Dialog:
                if (lastElementType == ElementType.Character)
                    return true;
                return false;
            default:
                return false;
        }
    }

    public bool FilterNewElements(ElementType elementType, ElementType lastElementType)
    {
        switch (lastElementType)
        {
            case ElementType.SceneHeading:
                if (elementType == ElementType.Dialog || elementType == ElementType.SceneHeading)
                {
                    return false;
                }
                return true;
            case ElementType.Action:
                if (elementType == ElementType.Dialog || elementType == ElementType.Action)
                    return false;
                return true;
            case ElementType.Character:
                if (elementType == ElementType.Dialog)
                    return true;
                return false;
            case ElementType.Dialog:
                if (elementType == ElementType.Dialog)
                    return false;
                return true;
            default:
                return false;
        }
    }
}
