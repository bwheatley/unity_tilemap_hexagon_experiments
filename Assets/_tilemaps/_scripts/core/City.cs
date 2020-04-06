using System;
using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using UnityEngine;

public class City : MapObject {

    public City() {
        Name = "Baltimore";
        HitPoints = 100;

        EXAMPLE();
        }

    private BuildingJob buildingJob;

    private float productionPerTurn = 9001;


    override public void SetHex( Hex newHex ) {

        if ( Hex != null ) {
            //Will cities be able to move from 1 hex to another?
            newHex.RemoveCity( this );
            }

        base.SetHex( newHex );

        Hex.AddCity( this );
        }


    public void DoTurn() {
        if ( buildingJob != null ) {
            float workLeft = buildingJob.DoWork( productionPerTurn );
            if ( workLeft <= 0 ) {
                //Job is complete
                buildingJob = null;

                //TODO: save overflow for later
                }
            }
        }

    void EXAMPLE() {

        buildingJob = new BuildingJob( null,
            "Dwarf Warrior",
            100,
            0,
            () =>
            {
                this.Hex.HexMap.SpawnUnitAt(
                    new Unit(),
                    GameManager.instance.playerUnit,
                    this.Hex.Q,
                    this.Hex.R
                );
            },
            null

        );
        }
    }
//test