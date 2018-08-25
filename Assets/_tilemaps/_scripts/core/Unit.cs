using System;
using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using QPath;
using UnityEngine;

public class Unit : IQPathUnit {


    public string Name = "Unnamed Unit";
    public int HitPoints = 100;
    public int Strenght = 8;
    public int Movement = 2;
    public int MovementRemaining = 2;

    public delegate void UnitMovedDelegate(Hex oldHex, Hex newHex);
    public event UnitMovedDelegate OnUnitMoved;

    public Hex Hex { get; protected set; }

    private Queue<Hex> hexPath;

    public void SetHex(Hex newHex) {

        Hex oldHex = Hex;
        
        if (Hex != null) {
            newHex.RemoveUnit(this);
        }

        Hex = newHex;
        Hex.AddUnit(this);

        if (OnUnitMoved != null) {
            OnUnitMoved(oldHex, newHex);
        }

    }

    public void DoTurn() {
        //Do the turn/queued move?
        Debug.Log("Do Turn");
        //Do queue move
        if (hexPath == null || hexPath.Count == 0) {
            return;
        }

        //Grab the first hex from our queue
        Hex newHex = hexPath.Dequeue();


        //move to the new hex
        SetHex(newHex);

    }

    public int MovementCostToEnterHex( Hex hex) {
        //TODO override base movement cost based on our
        //movement mode + tile type (flying, walking etc)
        return hex.BaseMovementCost();
    }

    public float AggregateTurnsToEnterHex( Hex hex, float turnsToDate) {
        // the issue we have is that if you are trying to enter a hex with 
        // a movement cost greater than your current remaining movement points,
        //this will either result in a cheaper-than expected turn cost (civ5)
        // or a more-expensive-than expected turn cost (civ6)

        float baseTurnsToEnterHex = MovementCostToEnterHex(hex) / Movement; //Example: Enter a forest is "1" turn 2MP

        if (baseTurnsToEnterHex < 0) {
            //Impassible
            return -1;
        }

        if (baseTurnsToEnterHex > 1) {
            //Even if something costs 3 to enter and we have a max move of 2 
            //you can always enter it using a full turn of movement.
            baseTurnsToEnterHex = 1;
        }

        float turnsRemaning = MovementRemaining / Movement; //Example, if we are at 1 of 2 move we have .5 turns left.

        float turnsToDateWhole = Mathf.Floor(turnsToDate); //EX: 4.33 becomes 4
        float turnsToDateFraction = turnsToDate - turnsToDateWhole; //Example: 4.33 becomes 0.33

        if ((turnsToDateFraction < 0.01f && turnsToDateFraction > 0) || turnsToDateFraction > 0.99f) {
            Util.WriteDebugLog(
                string.Format("Looks like we have floating-point drift - Fraction: {0} TurnsToDate: {1}",
                    turnsToDateFraction, turnsToDate), GameManager.LogLevel_Notice, GameManager.instance.debug,
                GameManager.instance.LogLevel);
            //todo round things
            if (turnsToDateFraction < 0.01f) {
                turnsToDateFraction = 0f;
            }

            if (turnsToDateFraction > 0.99f) {
                turnsToDateWhole++;
                turnsToDateFraction = 0f;
            }
        }

        float turnsUsedAfterThismove = turnsToDateFraction + baseTurnsToEnterHex; //Example: 0.33 + 1

        if (turnsUsedAfterThismove > 1) {
            //we have the situation where we don't actually have enough movement to complete this move
            if (GameManager.MOVEMENT_RULES_LIKE_CIV6) {
                //We aren't allowed to enter the tile this move, that means we have to....

                if (turnsToDateFraction == 0) {
                    //We have full movement. but this isn't enough to enter the tile
                    //Example: we have a max move of 2 but the tile costs 3 to enter
                    //We are good to go
                }
                else {
                    //we are not on a fresh turn -- therefore we need to
                    //Sit idle for the remainder of the turn
                    turnsToDateWhole++;
                    turnsToDateFraction = 0f;
                }

                //So now we know for a fact we are starting the move into difficult terrain
                //on a fresh turn.
                turnsUsedAfterThismove = baseTurnsToEnterHex;
                if (turnsUsedAfterThismove > 1) {
                    turnsUsedAfterThismove = 1;
                }

            }
            else {
                //Civ5 style rules state that we can always enter a tile, even if we don't
                //have enough movement left
                turnsUsedAfterThismove = 1;
            }
        }

        //Turnsusedafterthismove is now  some value from 0..1.
        //this includes the fractional part of moves from previous turns

        


        //Do we return the number of turns THIS Move is going to take?
        //No, it's an aggregate so it should return the total turn cost
        //turnsTODate + turns for this move



        return turnsToDateWhole + turnsUsedAfterThismove;

    }

    /// <summary>
    /// Turn cost to enter a hex (i.e. 0.5 turns if a movement cost is 1 and we have 2 max movement)
    /// </summary>
    /// <param name="sourceTile"></param>
    /// <param name="destinationTile"></param>
    /// <returns>The </returns>
    public float CostToEnterHex(IQPathTile sourceTile, IQPathTile destinationTile) {
        return 1;
    }

    /// <summary>
    /// Clear out a HexPath if we need to kill a queue for a unit
    /// </summary>
    public void ClearHexPath() {
        this.hexPath = new Queue<Hex>();
    }

    public void SetHexPath(Hex[] hexArray)
    {
        this.hexPath = new Queue<Hex>(hexArray);

        if (hexPath.Count > 0) {
            this.hexPath.Dequeue(); //First hex is local hex get rid of it
        }
    }


    public void DUMMY_PATHING_FUNCTION() {
        Debug.Log("Dummy Path");

        Hex[] pathTiles = QPath.QPath.FindPath<Hex>(
            Hex.HexMap,
            this,
            Hex,
            Hex.HexMap.GetHexAt(Hex.Q +6 , Hex.R+1),
            Hex.CostEstimate
        );
      

        Debug.Log(string.Format("Get Pathfinding path of length: {0}", pathTiles.Length));

        SetHexPath(pathTiles);


    }

}

