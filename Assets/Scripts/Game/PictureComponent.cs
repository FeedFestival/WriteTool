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
    private RectTransform _parentContainer;

    private int _imagesCount;
    public Image FirstImage;
    public Image SecondImage;

    public string[] Paths { get; set; }

    public void FillImages()
    {
        int count = 0;
        if (_parentContainer == null)
        {
            _parentContainer = FirstImage.transform.parent.parent.GetComponent<RectTransform>();
        }

        foreach (var path in Paths)
        {
            if (string.IsNullOrEmpty(path) == false)
            {
                if (count == 0)
                {
                    CancelSecondImage();
                }
                else
                {
                    ShowSecondImage();
                }
                count++;
                OnPictureLoaded(GameService.Instance.ReadPicture(path));
            }
        }
    }

    public void AutoSelect()
    {
        if (_parentContainer == null)
        {
            _parentContainer = FirstImage.transform.parent.parent.GetComponent<RectTransform>();
        }

        if (_imagesCount == 0)
        {
            GameService.Instance.TakePic(OnPictureLoaded);
            CancelSecondImage();
        }
        else
        {
            ShowSecondImage();
        }
        OnFocus();
    }

    private void OnPictureLoaded(Texture2D texture, string path = null)
    {
        if (texture == null)
        {
            Debug.Log("Couldn't load texture");
            return;
        }
        if (Paths == null)
        {
            Paths = new string[2];
        }

        Rect rec = new Rect(0, 0, texture.width, texture.height);
        if (_imagesCount == 0)
        {
            FirstImage.sprite = Sprite.Create(texture, rec, new Vector2(0, 0), .01f);
            if (path != null)
            {
                Paths[0] = path;
            }
        }
        else
        {
            SecondImage.sprite = Sprite.Create(texture, rec, new Vector2(0, 0), .01f);
            if (path != null)
            {
                Paths[1] = path;
            }
        }
        _imagesCount++;
        GameService.Instance.InternalWait(HotkeyController.Instance.EscapeKey, 0.5f);
    }

    private void CancelSecondImage()
    {
        SecondImage.transform.parent.gameObject.SetActive(false);
        FirstImage.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_parentContainer.sizeDelta.x, _parentContainer.sizeDelta.y);
    }

    private void ShowSecondImage()
    {
        SecondImage.transform.parent.gameObject.SetActive(true);
        FirstImage.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_parentContainer.sizeDelta.x / 2, _parentContainer.sizeDelta.y);
        SecondImage.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_parentContainer.sizeDelta.x / 2, _parentContainer.sizeDelta.y);
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
            GameService.Instance.TakePic(OnPictureLoaded);
        });
        HotkeyController.Instance.RegisterForForcedEnterKey(() =>
        {
            HotkeyController.Instance.OnAddNewElement();
            OnBlur();
        });
        HotkeyController.Instance.RegisterForEscapeKey(() =>
        {
            OnBlur();
        });
        HotkeyController.Instance.RegisterBackspaceKey(() =>
        {
            _backspaceClick++;
            if (_backspaceClick > 1)
            {
                _backspaceClick = 0;
                if (_imagesCount == 0)
                {
                    OnBlur();
                    ElementsController.Instance.DeleteElement(UniqueId);
                }
                else if (_imagesCount == 1)
                {
                    Paths[0] = null;
                    FirstImage.sprite = GameHiddenOptions.Instance.AddImageSprite;
                    SecondImage.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    Paths[1] = null;
                    SecondImage.sprite = GameHiddenOptions.Instance.AddImageSprite;
                    SecondImage.transform.parent.gameObject.SetActive(false);
                }
                _imagesCount--;
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
        if (_imagesCount != 2)
        {
            CancelSecondImage();
        }
        HotkeyController.Instance.RegisterForForcedEnterKey(null);
    }
}
