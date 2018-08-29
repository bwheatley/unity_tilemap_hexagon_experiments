using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public bool debug = true;

    [Range(0, 7)][Tooltip("0 is only emergency messages 7 is debug log everything")]
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

    public static GameManager instance;

    //UI START
    public GameObject uiUnitSelectionPanel;

    //UI END

    //TODO testing, this should be moved to a game manager
    public const bool MOVEMENT_RULES_LIKE_CIV6 = false;


    public GameObject playerUnit;
    public GameObject CityLevel1Prefab;
    [HideInInspector] public Vector2 playerStartPos = new Vector2(36,10);


    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Creating a new version of GameManager.");
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("GameManager already exists.");
            Destroy(this.gameObject);
            return;
        }
    }
}
