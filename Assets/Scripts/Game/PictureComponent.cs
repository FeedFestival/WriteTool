﻿using System.Collections;
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
            SecondImage.transform.parent.gameObject.SetActive(true);
            FirstImage.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_parentContainer.sizeDelta.x / 2, _parentContainer.sizeDelta.y);
            SecondImage.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_parentContainer.sizeDelta.x / 2, _parentContainer.sizeDelta.y);
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
            SecondImage.sprite = Sprite.Create(texture, rec, new Vector2(0, 0), .01f);
        }
        _imagesCount++;
        GameService.Instance.InternalWait(HotkeyController.Instance.EscapeKey, 0.5f);
    }

    private void CancelSecondImage()
    {
        SecondImage.transform.parent.gameObject.SetActive(false);
        FirstImage.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(_parentContainer.sizeDelta.x, _parentContainer.sizeDelta.y);
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
            _backspaceClick++;
            if (_backspaceClick > 1)
            {
                _backspaceClick = 0;
                if (_imagesCount == 0)
                {
                    OnBlur();
                    ElementsController.Instance.DeleteElement(UniqueId);
                }
                else
                {
                    SecondImage.transform.parent.gameObject.SetActive(false);
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
        if (_imagesCount > 0)
        {
            CancelSecondImage();
        }
        HotkeyController.Instance.RegisterForForcedEnterKey(null);
    }
}
