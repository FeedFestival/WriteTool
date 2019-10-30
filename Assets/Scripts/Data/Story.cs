using UnityEngine;
using System.Collections;
using SQLite4Unity3d;
using Assets.Scripts.Utils;

public class Story: IJsonConsole
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }

    public string GetStoryNamePath()
    {
        return Id + "_" + Assets.Scripts.Utils.UsefullUtils.RemoveWhitespace(Name);
    }

    public string GetActivePath()
    {
        return Path + "/";
    }

    public string ToJsonString()
    {
        return @"{
            Id: " + Id + @"
            Name: " + Name + @"
            Path: " + Path + @"
        }
        ";
    }
}
