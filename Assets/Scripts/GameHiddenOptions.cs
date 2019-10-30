using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHiddenOptions : MonoBehaviour
{
    private static GameHiddenOptions _gameHiddenOptions;
    public static GameHiddenOptions Instance
    {
        get { return _gameHiddenOptions; }
    }

    void Awake()
    {
        _gameHiddenOptions = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool InstantDebug;
    public string ServerURL = "http://127.0.0.1:8080";

    //[Header("Execution Timers")]
    //public float TimeBeforeSessionCreation = 0.2f;
    //public float TimeBeforeGameStart = 0.1f;

    //[Header("Base Colors")]
    //public Color32 BlackColor;
    //public Color32 WhiteColor;
    //public Color32 RedColor;
    //public Color32 LightBlueColor;
    //public Color32 FullTransparentColor;
    //public Color32 GameNameColor;

    [Header("Input Colors")]
    public Color32 IconColor;
    public Color32 LabelColor;
    public Color32 DisabledTextColor;
    public Color32 NormalTextColor;
    public Color32 CarretColor;
    public Color32 TransparentColor;

    [Header("Prefabs")]
    public GameObject SceneHeadingPrefab;
    public GameObject ActionPrefab;
    public GameObject CharacterPrefab;
    public GameObject DialogPrefab;
    public GameObject PicturePrefab;
    public GameObject StoryComponentPrefab;
    public Sprite AddImageSprite;

    [Header("Miscs")]
    public CanvasScaler CanvasScaler;

    public const string _startIcon = "";

    public static int MAX_BYTE_SIZE = 1024;

    public GameObject GetPrefabElement(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.SceneHeading:
                return SceneHeadingPrefab;
            case ElementType.Action:
                return ActionPrefab;
            case ElementType.Character:
                return CharacterPrefab;
            case ElementType.Dialog:
                return DialogPrefab;
            case ElementType.Picture:
                return PicturePrefab;
            default:
                break;
        }
        return null;
    }

    public static byte[] StringToBytes(string str)
    {
        byte[] numArray = new byte[str.Length * 2];
        System.Buffer.BlockCopy((System.Array)str.ToCharArray(), 0, (System.Array)numArray, 0, numArray.Length);
        return numArray;
    }

    public static string BytesToString(byte[] bytes)
    {
        char[] chArray = new char[bytes.Length / 2];
        System.Buffer.BlockCopy((System.Array)bytes, 0, (System.Array)chArray, 0, bytes.Length);
        return new string(chArray);
    }

    internal float GetTime(float normalTime)
    {
        return InstantDebug ? 0f : normalTime;
    }

    public GameObject GetAnInstantiated(GameObject prefab)
    {
        return Instantiate(prefab);
    }
    //public Color32 TextOverBlackColor;
}
