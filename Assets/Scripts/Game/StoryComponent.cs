using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryComponent : MonoBehaviour, IPrefabComponent
{
    public Text StoryId;
    public Text StoryName;
    public Text StoryPath;
    public Text StoryDate;

    private int _id;
    int IPrefabComponent.Id { get => _id; set => _id = value; }
    private int _uniqueId;
    int IPrefabComponent.UniqueId { get => _uniqueId; set => _uniqueId = value; }

    GameObject IPrefabComponent.GameObject => this.gameObject;

    private Button Button;

    void Awake()
    {
        Button = gameObject.GetComponent<Button>();
    }

    public void Select()
    {
        Button.Select();
        // set text color red;

        ProjectHotkeyEditor.Instance.RegisterForEnterKey(() => {
            Clicked();
        });
    }

    public void Deselect()
    {
        EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        // set text color grey;
    }

    public void Clicked()
    {
        StoryController.Instance.OpenStory(_id);
    }
}
