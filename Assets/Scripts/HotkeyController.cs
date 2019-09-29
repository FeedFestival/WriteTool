using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HotkeyController : MonoBehaviour
{
    public static HotkeyController _hotkeyController;
    public static HotkeyController Instance { get { return _hotkeyController; } }

    public bool UseHotkeys;

    public delegate void OnHotkeyPress();
    private Dictionary<string, OnHotkeyPress> HotkeyComponents;

    private void Awake()
    {
        _hotkeyController = this;
    }

    void Start()
    {
        Init();
    }

    private KeyCode[] NewWrite = { KeyCode.LeftControl, KeyCode.LeftShift, KeyCode.W };
    public int NewWriteIndex = 0;

    // Update is called once per frame
    void Update()
    {
        if (UseHotkeys == false)
            return;

        if (NewWriteIndex < NewWrite.Length)
        {
            if (Input.GetKeyDown(NewWrite[NewWriteIndex]))
            {
                NewWriteIndex++;
            }
        }
        else
        {
            NewWriteIndex = 0;
            HotkeyComponents["NewWrite"]();
        }

        //if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.S))
        //{
        //    HotkeyComponents["NewWrite_SceneHeading"]();
        //}
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
    }

    void Init()
    {
        StartCoroutine(Interval());
    }

    IEnumerator Interval() {

        yield return new WaitForSeconds(1f);
        NewWriteIndex = 0;
        StartCoroutine(Interval());
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
