using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerMap 
{
    public PlayerMap(){

    }
    
    public PlayerMap(int _AccountID, int _MapID, int _RestartNumber, int _StepNumber, bool _IsVoted, bool _IsDeleted)
    {
        AccountID = _AccountID;
        MapID = _MapID;
        StepNumber = _StepNumber;
        RestartNumber = _RestartNumber;
        IsVoted = _IsVoted;
        IsDeleted = _IsDeleted;
        DeletedDate = null;
    }

    public int AccountID { get; set; }
    public int MapID {get; set; }
    public int StepNumber {get; set; }
    public int RestartNumber {get; set; }
    public bool IsVoted {get; set; }
    public bool IsDeleted {get; set; }
    public DateTime? DeletedDate { get; set; }
}