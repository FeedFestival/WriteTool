using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompileStyles : MonoBehaviour
{
    public Vector2 ReferenceResolution;

    // Start is called before the first frame update
    void Awake()
    {
        Init(true);
    }

    public void Init(bool atRunTime = false)
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.GetComponent<Style>() != null)
            {
#if UNITY_EDITOR
                child.GetComponent<Style>().ApplyStyle(atRunTime);
#else
                child.GetComponent<Style>().ApplyStyle(true);
#endif
            }
            //Debug.Log(child.gameObject.name);
        }
    }   
}
