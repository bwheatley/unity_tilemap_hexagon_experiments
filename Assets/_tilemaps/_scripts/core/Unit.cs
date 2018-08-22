using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {


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

    public void SetHexPath( Hex[] hexPath ) {
        this.hexPath = new Queue<Hex>(hexPath);
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

        return 1f;
    }

}

