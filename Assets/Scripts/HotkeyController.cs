using UnityEngine;
using System.Collections;

public class HotkeyController : MonoBehaviour
{
    public static HotkeyController _hotkeyController;
    public static HotkeyController Instance { get { return _hotkeyController; } }

    public bool UseHotkeys;

    //public 

    // Update is called once per frame
    void Update()
    {
        if (UseHotkeys == false)
            return;

        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.N))
        {
            
        }
    }
}
