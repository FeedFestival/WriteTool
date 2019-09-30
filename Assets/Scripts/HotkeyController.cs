using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class HotkeyController : MonoBehaviour
{
    public static HotkeyController _hotkeyController;
    public static HotkeyController Instance { get { return _hotkeyController; } }

    public bool UseHotkeys;

    public Image HotkeysImage;

    private bool _canUseHotkeys;

    public delegate void OnHotkeyPress();
    private Dictionary<string, OnHotkeyPress> HotkeyComponents;
    private OnHotkeyPress _enterOnHotkeyPress;
    private OnHotkeyPress _escapeOnHotkeyPress;

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
        if (UseHotkeys == false || _canUseHotkeys == false)
            return;

        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.S))
        {
            HotkeyComponents["NewWrite_SceneHeading"]();
        }
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.A))
        {
            HotkeyComponents["NewWrite_Action"]();
        }
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.C))
        {
            HotkeyComponents["NewWrite_Character"]();
        }
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.D))
        {
            HotkeyComponents["NewWrite_Dialog"]();
        }

        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.S))
        {
            HotkeyComponents["Save"]();
        }

        if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Return))
        {
            _enterOnHotkeyPress?.Invoke();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            _escapeOnHotkeyPress?.Invoke();
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
    
    void Init()
    {
        
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
