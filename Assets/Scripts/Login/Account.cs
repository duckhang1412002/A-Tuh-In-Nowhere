using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Account 
{
    public Account(){

    }

    // for login - register 
    public Account(int _ID, string _Email, string _Pwd, string _Fullname, bool _IsOnlined)
    {
        AccountID = _ID;
        Email = _Email;
        Pwd = _Pwd;
        Fullname = _Fullname;
        IsOnlined = _IsOnlined;

        // auto data
        StatusID = "";
        RoleID = 2;
        Nickname = "";
        Avatarlink = "";
        Ribbon = 0;
        Key = 0;
        Createddate = DateTime.Now;
        Lastactive = DateTime.Now;
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