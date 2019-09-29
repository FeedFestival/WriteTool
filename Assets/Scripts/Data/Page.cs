using UnityEngine;
using System.Collections;
using SQLite4Unity3d;

public class Page
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int StoryId { get; set; }
}
