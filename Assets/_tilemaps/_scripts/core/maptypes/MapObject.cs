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

    public bool IsDestroyed {
        get;
        private set;
    }

    public Hex Hex { get; protected set; }

    public delegate void ObjectMovedDelegate( Hex oldHex, Hex newHex );
    public event ObjectMovedDelegate OnObjectMoved;

    public delegate void ObjectDestroyedDelegate( MapObject mo);
    public event ObjectDestroyedDelegate OnObjectDestroyed;

    virtual public void SetHex(Hex newHex)
    {

        Hex oldHex = Hex;

        Hex = newHex;

        if (OnObjectMoved != null)
        {
            OnObjectMoved(oldHex, newHex);
        }

    }

    /// <summary>
    /// This object is being removed from the map/game
    /// </summary>
    virtual public void Destroy(){

        IsDestroyed = true;
        if ( OnObjectDestroyed != null ) {
            OnObjectDestroyed( this );
        }
    }


}
