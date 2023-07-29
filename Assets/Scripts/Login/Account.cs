using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Account 
{
    public Account()
    {
        acc_email = "";
        acc_nickname = "";
        acc_password = "";
        acc_isActive = false;
    }

    public string acc_email { get; set; }
    public string acc_nickname { get; set; }
    public string acc_password { get; set; }
    public bool acc_isActive { get; set; }
}
