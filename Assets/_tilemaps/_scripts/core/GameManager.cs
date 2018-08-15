﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public bool debug = true;
    [Range(0, 7)]
    public int LogLevel;

    //0 Emergency
    public const int LogLevel_Emerg = 0;
    [Tooltip("1 Alert")]
    public const int LogLevel_Alert = 1;
    [Tooltip("2 Critical")]
    public const int LogLevel_Critical = 2;
    [Tooltip("3 Error")]
    public const int LogLevel_Error = 3;
    [Tooltip("4 Warning")]
    public const int LogLevel_Warning = 4;
    [Tooltip("5 Notice")]
    public const int LogLevel_Notice = 5;
    [Tooltip("6 Info")]
    public const int LogLevel_Info = 6;
    [Tooltip("7 Debug - Everythign!")]
    public const int LogLevel_Debug = 7;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
