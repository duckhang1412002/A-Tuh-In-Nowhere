using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Account 
{
    public Account(){

    }

    // for login - register 
    public Account(int _ID, string _Email, string _Pwd, string _Fullname, bool _IsOnlined, int _RoleID, string _Nickname, string _AvtLink, int _Ribbon, int _Key, DateTime _LastActive)
    {
        AccountID = _ID;
        Email = _Email;
        Pwd = _Pwd;
        Fullname = _Fullname;
        IsOnlined = _IsOnlined;
        RoleID = 2;
        Nickname = "";
        Avatarlink = "";
        Ribbon = 0;
        Key = 0;

        Lastactive = _LastActive;

        // auto data
        StatusID = "";
        Createddate = DateTime.Now;
        Deleteddate = null;
        IsDeleted = false;
    }

    public int AccountID { get; set; }
    public string StatusID { get; set; }
    public int RoleID { get; set; }
    public string Email { get; set; }
    public string Pwd { get; set; }
    public string Fullname { get; set; }
    public string Nickname { get; set; }
    public string Avatarlink { get; set; }
    public int Ribbon { get; set; }
    public int Key { get; set; }
    public DateTime Createddate { get; set; }
    public DateTime Lastactive { get; set;}
    public bool IsOnlined { get; set; }
    public DateTime? Deleteddate { get; set; }
    public bool IsDeleted { get; set; }
}