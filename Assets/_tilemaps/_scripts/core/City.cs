using System;
using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using UnityEngine;

public class City : MapObject
    {

        public City() {
            Name = "Baltimore";
            HitPoints = 100;
            

        }

        override public void SetHex(Hex newHex)
        {

            if (Hex != null)
            {
                //Will cities be able to move from 1 hex to another?
                newHex.RemoveCity(this);
            }

            base.SetHex(newHex);

            Hex.AddCity(this);
        }


}
