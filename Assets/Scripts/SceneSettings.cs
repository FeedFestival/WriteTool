using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine;

public class SceneSettings : MonoBehaviour
{
    public GameObject StarterGameObject;
    public GameObject[] DontDestroyPrefabs;

    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.Find("_GameHiddenOptions (DD)") == null) {
            foreach (var prefab in DontDestroyPrefabs)
            {
                UsefullUtils.CreateGameObject(Instantiate(prefab), prefab.name);   
            }
        }

        GameService.Instance.InternalWait(() => {
            StarterGameObject.GetComponent<ISceneStarter>().InitScene();
        }, 0.1f);
    }
}
