using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Assets.Scripts.Data;

public class TextEditorHotkeyController : MonoBehaviour
{
    public static TextEditorHotkeyController _hotkeyController;
    public static TextEditorHotkeyController Instance { get { return _hotkeyController; } }
    public delegate void OnHotkeyPress();
    private Dictionary<string, OnHotkeyPress> HotkeyComponents;
    private OnHotkeyPress _enterOnHotkeyPress;
    private OnHotkeyPress _forcedEnterOnHotkeyPress;
    private OnHotkeyPress _escapeOnHotkeyPress;
    private OnHotkeyPress _backspaceOnHotkeyPress;
    private OnHotkeyPress _tabEditOnHotkeyPress;

    public bool ShowOptions;

    [SerializeField]
    public AppState AppState;

    public InLineSelection InLineSelection;
    public GameObject FileOptionsSelection;
    public GameObject FileMainButtons;
    public Image CarretImage;

    private void Awake()
    {
        _hotkeyController = this;

        if (ShowOptions)
        {
            FileMainButtons.SetActive(true);
            FileOptionsSelection.SetActive(false);
            InLineSelection.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            EnterKey();
        }

        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
            && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
        {
            EnterKey(isForced: true);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            EscapeKey();
        }

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            BackspaceKey();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            FileKey();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CloseKey();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ExportKey();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            HotkeyComponents["NewWrite"]();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveKey();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            NewWrite("NewWrite_Action");
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            NewWrite("NewWrite_Character");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            NewWrite("NewWrite_Dialog");
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            NewWrite("NewWrite_Picture");
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            OnArrowKeys(goDown: false);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            OnArrowKeys();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            OnTabEdit();
        }
    }

    internal void RegisterForEnterKey(OnHotkeyPress enterOnHotkeyPress)
    {
        AppState = enterOnHotkeyPress != null ? AppState.Editing : AppState;
        _enterOnHotkeyPress = enterOnHotkeyPress;

        ShowCarret();
    }

    internal void RegisterForForcedEnterKey(OnHotkeyPress enterOnHotkeyPress)
    {
        AppState = enterOnHotkeyPress != null ? AppState.Editing : AppState;
        _forcedEnterOnHotkeyPress = enterOnHotkeyPress;

        ShowCarret();
    }

    internal void RegisterForEscapeKey(OnHotkeyPress escapeOnHotkeyPress)
    {
        _escapeOnHotkeyPress = escapeOnHotkeyPress;
    }

    internal void RegisterBackspaceKey(OnHotkeyPress backspaceOnHotkeyPress)
    {
        _backspaceOnHotkeyPress = backspaceOnHotkeyPress;
    }

    void Init()
    {

    }

    public void EscapeKey()
    {
        if (AppState == AppState.MainEdit)
        {
            // Debug.Log("Can't do shit");
        }
        else if (AppState == AppState.FileOptions)
        {
            ToggleFileOptions();
        }
        else if (AppState == AppState.Editing)
        {
            _escapeOnHotkeyPress?.Invoke();

            MainEdit();
            ShowCarret();
        }
        else
        {
            MainEdit();
        }
    }

    private void MainEdit()
    {
        if (ShowOptions)
        {
            FileMainButtons.SetActive(true);
            InLineSelection.gameObject.SetActive(false);
            FileOptionsSelection.gameObject.SetActive(false);
        }
        AppState = AppState.MainEdit;
    }

    public void OnTabEdit()
    {
        AppState = AppState.Editing;

        ElementsController.Instance.EditElement();
        ShowCarret();
    }

    public void EnterKey(bool isForced = false)
    {
        if (isForced)
        {
            _forcedEnterOnHotkeyPress?.Invoke();
            return;
        }
        _enterOnHotkeyPress?.Invoke();
    }

    public void BackspaceKey()
    {
        if (AppState == AppState.MainEdit)
        {
            return;
        }
        _backspaceOnHotkeyPress?.Invoke();
    }

    public void FileKey()
    {
        if (AppState == AppState.Editing)
        {
            return;
        }
        ToggleFileOptions();
    }

    public void CloseKey()
    {
        if (AppState == AppState.FileOptions)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }

    public void ExportKey()
    {
        if (AppState == AppState.FileOptions)
        {
            ElementsController.Instance.ExportToHtml();
        }
    }

    public void SaveKey()
    {
        if (AppState == AppState.FileOptions)
        {
            ElementsController.Instance.SaveElements();
        }
        else if (AppState == AppState.NewElement)
        {
            NewWrite("NewWrite_SceneHeading");
        }
    }

    public void NewWrite(string key)
    {
        if (AppState == AppState.Editing)
        {
            return;
        }

        HotkeyComponents[key]();
        AppState = AppState.MainEdit;
    }

    public void OnArrowKeys(bool goDown = true)
    {
        if (AppState == AppState.NewElement)
        {
            return;
        }

        if (AppState == AppState.Editing)
        {
            return;
        }
        ElementsController.Instance.MoveCarret(goDown);
    }

    public void AddAsComponent(string key, OnHotkeyPress value)
    {
        if (HotkeyComponents == null)
        {
            HotkeyComponents = new Dictionary<string, OnHotkeyPress>();
        }

        if (HotkeyComponents.ContainsKey(key) == false)
        {
            HotkeyComponents.Add(key, value);
        }
    }

    public void OnAddNewElement()
    {
        AppState = AppState.NewElement;

        if (ShowOptions)
        {
            FileMainButtons.SetActive(false);
            FileOptionsSelection.SetActive(false);
            InLineSelection.gameObject.SetActive(true);

            var options = FilterElementTypes();
            InLineSelection.Filter(options);
        }
    }

    private List<int> FilterElementTypes()
    {
        if (ElementsController.Instance.Elements == null)
        {
            ElementsController.Instance.InitElements(new List<Element>());
        }
        if (ElementsController.Instance.Elements.Count == 0)
        {
            return new List<int>() { 0 };
        }

        var previousElementType = ElementsController.Instance.GetElementAtCarretPosition();

        var options = new List<int>();
        int i = 0;
        foreach (ElementType elementType in (ElementType[])System.Enum.GetValues(typeof(ElementType)))
        {
            if (ElementsService.FilterNewElements(elementType, previousElementType))
            {
                options.Add(i);
            }
            i++;
        }
        return options;
    }

    public void ToggleFileOptions()
    {
        AppState = AppState == AppState.FileOptions
            ? AppState.MainEdit : AppState.FileOptions;

        if (ShowOptions)
        {
            if (AppState == AppState.FileOptions)
            {
                FileMainButtons.SetActive(false);
                FileOptionsSelection.SetActive(true);
            }
            else
            {
                FileMainButtons.SetActive(true);
                FileOptionsSelection.SetActive(false);
            }
            InLineSelection.gameObject.SetActive(false);
        }
    }

    public void ShowCarret()
    {
        if (AppState == AppState.MainEdit
            || AppState == AppState.FileOptions
            || AppState == AppState.NewElement)
        {
            CarretImage.color = GameHiddenOptions.Instance.CarretColor;
        }
        else
        {
            CarretImage.color = GameHiddenOptions.Instance.TransparentColor;
        }
    }
}

public enum AppState
{
    MainEdit,
    Editing,
    FileOptions,
    NewElement
}
