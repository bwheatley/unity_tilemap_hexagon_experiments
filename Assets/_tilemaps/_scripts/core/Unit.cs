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

        Hex oldHex = Hex;
        Hex newHex = Hex.HexMap.GetHexAt(oldHex.Q + 1, oldHex.R);

        SetHex(newHex);

    }



}

