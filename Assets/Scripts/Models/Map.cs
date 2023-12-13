using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    // Start is called before the first frame update
    public Map()
    {

    }

    public Map(int _AccountID, int _MapID, string _MapName, string _MapType, string _Description, DateTime _CreatedDate, DateTime _ModifiedDate, bool _IsDeleted)
    {
        AccountID = _AccountID;
        MapID = _MapID;
        MapName = _MapName;
        MapType = _MapType;
        Description = _Description;
        CreatedDate = _CreatedDate;
        ModifiedDate = _ModifiedDate;
        IsDeleted = _IsDeleted;
    }

    public Map(int _AccountID, string _AccountName, int _MapID, string _MapName, string _MapType, string _Description, DateTime _CreatedDate, DateTime _ModifiedDate, bool _IsDeleted)
    {
        AccountID = _AccountID;
        AccountName = _AccountName;
        MapID = _MapID;
        MapName = _MapName;
        MapType = _MapType;
        Description = _Description;
        CreatedDate = _CreatedDate;
        ModifiedDate = _ModifiedDate;
        IsDeleted = _IsDeleted;
    }

    public int AccountID { get; set; }
    public string AccountName { get; set; }
    public int MapID { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string MapName { get; set; }
    public string MapType { get; set; }
    public string MapThumbnail { get; set; }
    public string Description { get; set; }
}
