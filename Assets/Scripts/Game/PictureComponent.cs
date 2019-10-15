using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureComponent : MonoBehaviour, IPrefabComponent, IPictureComponent, IElementComponent
{
    private int _id;
    public int Id { get { return _id; } set { _id = value; } }
    private int _uniqueId;
    public int UniqueId { get { return _uniqueId; } set { _uniqueId = value; } }
    public GameObject GameObject { get { return this.gameObject; } }
    private int _typeId;
    public int TypeId { get { return _typeId; } set { _typeId = value; } }
    private int _backspaceClick = 0;

    // public GameObject FirstImageGo;
    // public GameObject SecondImageGo;
    private int _imagesCount;
    public Image FirstImage;
    public Image SecondImage;

    public void AutoSelect()
    {
        if (_imagesCount == 0)
        {
            GameService.Instance.TakePic(OnPictureLoaded);
        }
        OnFocus();
    }

    private void OnPictureLoaded(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.Log("Couldn't load texture");
            return;
        }
        Rect rec = new Rect(0, 0, texture.width, texture.height);

        if (_imagesCount == 0)
        {
            FirstImage.sprite = Sprite.Create(texture, rec, new Vector2(0, 0), .01f);
        }
        else
        {
            _imagesCount++;
        }
    }

    public void OnFocus()
    {
        GameService.Instance.Debounce(Focussed, 0.1f);
    }

    private void Focussed()
    {
        _backspaceClick = 0;

        HotkeyController.Instance.RegisterForEnterKey(() =>
        {
            ElementsController.Instance.OnAddNewElement();
            OnBlur();
        });
        HotkeyController.Instance.RegisterForEscapeKey(() =>
        {
            OnBlur();
        });
        HotkeyController.Instance.RegisterBackspaceKey(() =>
        {
            if (_imagesCount == 0)
            {
                _backspaceClick++;
                if (_backspaceClick > 1)
                {
                    OnBlur();
                    ElementsController.Instance.DeleteElement(UniqueId);
                }
            }
        });
    }

    public void OnEditing(string value)
    {
        // if (!Input.GetKeyDown(KeyCode.Escape))
        // {

        // }
    }

    public void OnBlur()
    {
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {

        // }
        GameService.Instance.Debounce(Blurred, 0.1f);
    }

    private void Blurred()
    {
        HotkeyController.Instance.RegisterForForcedEnterKey(null);
    }
}
