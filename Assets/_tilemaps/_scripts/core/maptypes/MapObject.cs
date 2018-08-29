using System;
using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using QPath;
using UnityEngine;

public class MapObject {

    public MapObject() {

    }


    public string Name;
    public int HitPoints;
    public bool CanBeAttacked = true;
    public int FactionID = 0;

    public Hex Hex { get; protected set; }

    public delegate void ObjectMovedDelegate(Hex oldHex, Hex newHex);
    public event ObjectMovedDelegate OnObjectMoved;

    virtual public void SetHex(Hex newHex)
    {

        Hex oldHex = Hex;

        Hex = newHex;

        if (OnObjectMoved != null)
        {
            OnObjectMoved(oldHex, newHex);
        }

    }


}
