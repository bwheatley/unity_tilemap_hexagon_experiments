using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using UnityEngine;

public class TurnController : MonoBehaviour {

    HexMap hexMap;

    // Use this for initialization
    void Start() {
        hexMap = GameObject.FindObjectOfType<HexMap>();
    }


    public void EndTurnButton() {
        Util.WriteDebugLog(
            string.Format( "TurnController::EndTurn"
            ), GameManager.LogLevel_Notice, GameManager.instance.debug,
            GameManager.instance.LogLevel );

        Unit[] units = hexMap.Units;
        City[] cities = hexMap.Cities;

        // First check to see if there are any units that have enqueued moves, if so process them.

        //Now are any units waiting for orders? if so halt end turn()

        //heal resting units

        //Reset unit movement
        for ( int i = 0; i < units.Length; i++ ) {
            var u = units[ i ];
            u.RefreshMovement();
            //StartCoroutine( DoAllUnitMoves() );
            StartCoroutine( hexMap.DoUnitMoves( u ) );
        }

        //If we get to this point no units are waiting for orders
        //goto process cities

        //Loop through cities
        for ( int i = 0; i < cities.Length; i++ ) {
            var c = cities[ i ];
            c.DoTurn();
        }



        //Goto next player

        //End 
    }

}
