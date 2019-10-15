﻿using UnityEngine;
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
    private Texture2D _currentLoadedPicture;
    private string _fileName;

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

    public delegate void OnPictureLoaded(Texture2D texture);
    private OnPictureLoaded _onPictureLoaded;

    public void TakePic(OnPictureLoaded onPictureLoaded)
    {
        _onPictureLoaded = onPictureLoaded;
        System.Windows.Forms.OpenFileDialog openFileDialog;
        openFileDialog = new System.Windows.Forms.OpenFileDialog()
        {
            InitialDirectory = @"D:\",
            Title = "Browse Text Files",

            CheckFileExists = true,
            CheckPathExists = true,

            DefaultExt = "png",
            Filter = "png files (*.png)|*.jpg",
            FilterIndex = 2,
            RestoreDirectory = true,

            ReadOnlyChecked = true,
            ShowReadOnly = true
        };

        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            Debug.Log(openFileDialog.FileName);
            var fileData = System.IO.File.ReadAllBytes(openFileDialog.FileName);
            _currentLoadedPicture = new Texture2D(2, 2);
            _currentLoadedPicture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            _fileName = openFileDialog.SafeFileName;
        }

        _onPictureLoaded(_currentLoadedPicture);
    }
}
