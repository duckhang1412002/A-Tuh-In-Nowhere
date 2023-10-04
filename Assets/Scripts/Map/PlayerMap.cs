using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMap 
{
    public PlayerMap()
    {
        AccountID = 0;
        MapID = 0;
        StepNumber = 0;
        RestartNumber = 0;
        IsVoted = false;
    }

    public int AccountID { get; set; }
    public int MapID {get; set; }
    public int StepNumber {get; set; }
    public int RestartNumber {get; set; }
    public bool IsVoted {get; set; }
}