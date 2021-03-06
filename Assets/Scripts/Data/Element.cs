﻿using UnityEngine;
using System.Collections;
using SQLite4Unity3d;
using System.Collections.Generic;

public class Element
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int StoryId { get; set; }
    public int PageId { get; set; }
    public int TypeId { get; set; }

    [Ignore]
    public ElementType ElementType
    {
        set
        {
            TypeId = (int)value;
        }
        get
        {
            return (ElementType)TypeId;
        }
    }

    public string Text { get; set; }
    public string EncodedText { get; set; }

    public int Index { get; set; }

    public bool IsNew;
    public bool ToDelete;
    public string[] Paths;
    public List<string> FileNames;

    public int UniqueId()
    {
        return System.Convert.ToInt32(this.Id.ToString() + this.Index.ToString() + ((int)this.ElementType).ToString());
    }
}

public enum ElementType
{
    SceneHeading,
    Action,
    Character,
    Dialog,
    Picture
}
