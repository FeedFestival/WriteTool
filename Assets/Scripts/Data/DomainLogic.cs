using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DomainLogic : MonoBehaviour
{
    private static DomainLogic _db;
    public static DomainLogic DB
    {
        get { return _db; }
    }

    private DataService _dataService;
    public DataService DataService
    {
        get
        {
            if (_dataService == null)
                _dataService = new DataService("Database.db", true);
            return _dataService;
        }
    }

    private void Awake()
    {
        _db = this;
        DontDestroyOnLoad(gameObject);
    }

    public SQLite4Unity3d.SQLiteConnection SqlConn()
    {
        return DataService.SqlConnection();
    }

    public void RecreateUserTable()
    {
        DataService.RecreateUserTable();
    }

    public void RecreateStoryTable()
    {
        DataService.RecreateStoryTable();
    }

    public void RecreateElementTable()
    {
        DataService.RecreateElementTable();
    }

    public void RecreatePageTable()
    {
        DataService.RecreatePageTable();
    }
}
