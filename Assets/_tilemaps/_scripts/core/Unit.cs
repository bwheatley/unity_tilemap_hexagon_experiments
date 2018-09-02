using System;
using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using QPath;
using UnityEngine;

public class Unit : MapObject, IQPathUnit {


    public Unit() {
        Name = "Soldier";
        HitPoints = 100;

    }



public int Strenght = 8;
    public int Movement = 2;
    public int MovementRemaining = 2;

    public bool CanBuildCities;
    public bool CanBuild;


    /// <summary>
    /// List of hexes to walk through, from pathfinder
    /// NOTE: First item is always the hex we are standing in
    /// </summary>
    private List<Hex> hexPath;


    public bool UnitWaitingForOrders() {
        //Returns true if we have movement left but nothing queued

        if (
            MovementRemaining > 0 && 
            (hexPath == null || hexPath.Count == 0)
            //TODO: maybe we've been told to Fority/Alert/Silent/SkipTurn
            ) {

            return true;
        }

        return false;
    }

    /// <summary>
    /// Processes one tile worth of movement for the unit.
    /// </summary>
    /// <returns><c>true</c>, if this should be called immediately again, <c>false</c> otherwise.</returns>
    public bool DoMove() {
        //Do the turn/queued move?
        Debug.Log("DoMove");

        if (MovementRemaining <= 0) {
            return false;
        }

        //Do queue move
        if (hexPath == null || hexPath.Count == 0) {
            return false;
        }

        //Grab the first hex from our queue
        Hex hexWeAreLeaving = hexPath[0];
        Hex newHex = hexPath[1];

        int costToEnter = MovementCostToEnterHex(newHex);

        if (costToEnter > MovementRemaining && MovementRemaining < Movement && GameManager.MOVEMENT_RULES_LIKE_CIV6) {
            //We can't enter the hex tis turn
            return false;
        }

        //Dequeue the first piece of the path
        hexPath.RemoveAt(0);

        /* If the length of hexpath is 1 it's because it's the only thing in the hex
         * is the current hex, so lets just clear
         */
        if (hexPath.Count == 1)
        {
            hexPath = null;
        }

        //move to the new hex
        SetHex(newHex);

        //Update movement remaining don't go under 0
        MovementRemaining = Mathf.Max(MovementRemaining - costToEnter, 0);

        //Return true if we should be called again.
        return hexPath != null && MovementRemaining > 0;
    }

    override public void SetHex( Hex newHex ) {

        if ( Hex != null ) {
            newHex.RemoveUnit( this );
        }

        base.SetHex( newHex );

        Hex.AddUnit( this );
    }

    override public void Destroy(  ) {

        Hex.RemoveUnit( this );
        base.Destroy( );

    }

    public int MovementCostToEnterHex( Hex hex) {
        //TODO override base movement cost based on our
        //movement mode + tile type (flying, walking etc)

         // Example of how to alter default movement
        //if (WeAreaHillWalker && hex.ElevationType == Hex.ELEVATION_TYPE.HILL)
            //return 1;

        //TODO implement other unit traits
        return hex.BaseMovementCost(false, false, false, false, false);
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

    public void RefreshMovement() {
        MovementRemaining = Movement;
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
        this.hexPath = new List<Hex>();
    }

    public void SetHexPath(Hex[] hexArray)
    {
        this.hexPath = new List<Hex>(hexArray);

        //if (hexPath.Count > 0) {
        //    this.hexPath.Dequeue(); //First hex is local hex get rid of it
        //}
    }


    public Hex[] GetHexPath(){
        return (this.hexPath == null ) ? null : this.hexPath.ToArray();
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

