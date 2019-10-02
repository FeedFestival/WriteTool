using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class HotkeyController : MonoBehaviour
{
    public static HotkeyController _hotkeyController;
    public static HotkeyController Instance { get { return _hotkeyController; } }

    public bool UseHotkeys = true;

    public Image HotkeysImage;

    public bool CanUseTools;
    private bool _canUseHotkeys = true;

    public bool ShowFileOptions;

    public delegate void OnHotkeyPress();
    private Dictionary<string, OnHotkeyPress> HotkeyComponents;
    private OnHotkeyPress _enterOnHotkeyPress;
    private OnHotkeyPress _escapeOnHotkeyPress;
    private OnHotkeyPress _backspaceOnHotkeyPress;

    private void Awake()
    {
        _hotkeyController = this;
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
            _enterOnHotkeyPress?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            _escapeOnHotkeyPress?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            _backspaceOnHotkeyPress?.Invoke();
        }

        if (UseHotkeys == false || _canUseHotkeys == false)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            HotkeyComponents["NewWrite"]();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            HotkeyComponents["ShowFileOptons"]();
        }

        if (ShowFileOptions && !CanUseTools)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                HotkeyComponents["Save"]();
            }
        }

        if (CanUseTools && !ShowFileOptions)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                NewWrite("NewWrite_SceneHeading");
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
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            ElementsController.Instance.MoveCarret(false);
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            ElementsController.Instance.MoveCarret();
        }
    }

    internal void RegisterForEnterKey(OnHotkeyPress enterOnHotkeyPress)
    {
        _canUseHotkeys = (enterOnHotkeyPress == null);
        _enterOnHotkeyPress = enterOnHotkeyPress;

        if (_canUseHotkeys)
        {
            HotkeysImage.color = GameHiddenOptions.Instance.NormalTextColor;
        }
        else
        {
            HotkeysImage.color = GameHiddenOptions.Instance.DisabledTextColor;
        }
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

    private void NewWrite(string key)
    {
        CanUseTools = false;
        HotkeyComponents[key]();
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
}
