﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraController;
using CameraController.Util;
using QPath;
using TMPro;

public class HexMap : MonoBehaviour, IQPathWorld {

    public GameObject HexPrefab;

    public Mesh MeshWater;
    public Mesh MeshFlat;
    public Mesh MeshHill;
    public Mesh MeshGrassland;
    public Mesh MeshMountain;

    public GameObject ForestPrefab;
    public GameObject JunglePrefab;
    public GameObject HillPrefab;

    // readonly will keep this from showing up in the editor and being confused when you can't figure out
    // why maps not working like you want
    public readonly int numRows = 30;
    public readonly int numColumns = 90;

    public Material MatOcean;
    public Material MatMountain;
    public Material MatPlain;
    public Material MatGrassland;
    public Material MatHill;
    public Material MatDesert;

    //Tiles with height above is a tile type whatever
    [NonSerialized] public float HeightMountain = 0.85f;
    [NonSerialized] public float HeightHill = 0.60f;
    [NonSerialized] public float HeightFlat = 0.0f;

    //Moisture
    [NonSerialized] public float MoistureJungle = 0.66f;
    [NonSerialized] public float MoistureForest = 0.33f;
    [NonSerialized] public float MoistureGrasslands = 0f;
    [NonSerialized] public float MoisturePlains = -0.5f;

    private Hex[,] hexes;
    Dictionary<Hex, GameObject> hexToGameObjectMap;

    private HashSet<Unit> units;
    Dictionary<Unit, GameObject> unitToGameObjectMap;

    public bool AllowWrapEastWest = true;

    // Use this for initialization
    void Start () {
		GenerateMap();
	}

    void Update() {
        //Backspace will advanced turn
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            if (units != null) {
                foreach (Unit u in units) {
                    u.DoTurn();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            if (units != null)
            {
                foreach (Unit u in units)
                {
                    u.DUMMY_PATHING_FUNCTION();
                }
            }
        }
    }

    public Vector3 GetHexPosition(int q, int r) {
        Hex hex = GetHexAt(q, r);

        return GetHexPosition(hex);
    }

    public Vector3 GetHexPosition(Hex hex) {
        return hex.PositionFromCamera(Camera.main.transform.position, numRows, numColumns);
    }

    public Hex GetHexAt(int x, int y) {
        if (hexes == null) {
            //throw new UnityException("Hexes array not yet instantiated!");
            Debug.LogError("Hexes array not yet instantiated");
            return null;
        }

        if (AllowWrapEastWest) {
            x = x % numColumns;
            if (x < 0) {
                x += numColumns;
            }
        }

        try {
            return hexes[x, y];
        }
        catch {
            Util.WriteDebugLog(string.Format("GetHexAt: {0},{1}",x,y),GameManager.LogLevel_Error, GameManager.instance.debug, GameManager.instance.LogLevel);
            return null;
        }
    }

    virtual public void GenerateMap() {

        //Generate ocean filled map
        //Some ways to do lists
        //List<List<Hex>> hexes = new List<List<Hex>>();
        //hexes[0] = new List<Hex>();

        hexes = new Hex[numColumns, numRows];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();

        for (int column = 0; column < numColumns; column++) {
            for (int row = 0; row < numRows; row++) {
         
                //Ocean hex
                Hex h = new Hex(this, column, row);
                h.Elevation = -0.5f;

                hexes[column, row] = h;

                //Instantiate a hex
                GameObject hexGO = (GameObject) Instantiate(HexPrefab, h.Position(), Quaternion.identity, this.transform);

                hexToGameObjectMap[h] = hexGO;

                var _hexcomp = hexGO.GetComponent<HexComponent>();
                _hexcomp.Hex = h;
                _hexcomp.HexMap = this;
                
                //Set the hex label
                hexGO.transform.GetChild(1).GetComponent<TextMeshPro>().text = string.Format("{0},{1}\n{2}", column, row, h.BaseMovementCost());
            }
        }

        //If our map doesn't move we can do this
        //StaticBatchingUtility.Combine(this.gameObject);
        UpdateHexVisuals();

        Unit unit = new Unit();
        SpawnUnitAt(unit, GameManager.instance.playerUnit, (int) GameManager.instance.playerStartPos.x, (int) GameManager.instance.playerStartPos.y);
    }

    public Hex[] GetHexesWithinRangeOf(Hex centerHex, int range) {
        List<Hex> results = new List<Hex>();

        //Minus 1 is to trim extra columns
        //+1 is to add left rows
        for (int dx = -range; dx < range-1; dx++) {
            for (int dy = Mathf.Max(-range+1, -dx-range); dy < Mathf.Min(range, -dx+range-1); dy++) {
                results.Add(GetHexAt( centerHex.Q +dx, centerHex.R + dy ));
            }                        
        }

        return results.ToArray();
    }

    public void UpdateHexVisuals() {
        for (int column = 0; column < numColumns; column++) {
            for (int row = 0; row < numRows; row++) {

                Hex h = hexes[column, row];
                GameObject hexGO = hexToGameObjectMap[h];

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();

                //Reset the movement since we start with ocean then repaint
                h.MovementCost = 1;

                //Moisture
                if (h.Elevation >= HeightFlat && h.Elevation < HeightMountain) { //No mountains with trees
                    if (h.Moisture >= MoistureJungle) {
                        mr.material = MatGrassland;
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.1f;
                        }

                        h.MovementCost = 2;
                        GameObject.Instantiate(JunglePrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureForest) {                        
                        mr.material = MatGrassland;
                        //Spawn trees
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill) {
                            p.y += 0.1f;
                        }

                        h.MovementCost = 2;
                        GameObject.Instantiate(ForestPrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureGrasslands) {
                        mr.material = MatGrassland;
                    }
                    else if (h.Moisture >= MoisturePlains) {
                        mr.material = MatPlain;
                    }
                    else {
                        mr.material = MatDesert;
                    }
                }

                //Elevation
                if (h.Elevation >= HeightMountain)
                {
                    mr.material = MatMountain;
                    mf.mesh = MeshMountain;
                    h.MovementCost = -99;
                }
                else if (h.Elevation >= HeightHill)
                {
                    mf.mesh = MeshHill;
                    Vector3 p = hexGO.transform.position;
                    if (h.Elevation >= HeightHill)
                    {
                        p.y += 0.1f;
                    }
                    h.MovementCost = 2;
                    GameObject.Instantiate(HillPrefab, p, Quaternion.identity, hexGO.transform);

                }
                else if (h.Elevation >= HeightFlat)
                {
                    mf.mesh = MeshGrassland;
                }
                else
                {
                    mr.material = MatOcean;
                    mf.mesh = MeshWater;
                    h.MovementCost = -99;
                }

                //Set the hex label
                hexGO.transform.GetChild(1).GetComponent<TextMeshPro>().text = string.Format("{0},{1}\n{2}", column, row, h.BaseMovementCost());

            }
        }
    }

    public void SpawnUnitAt(Unit unit, GameObject prefab, int q, int r) {

        if (units == null) {
            units = new HashSet<Unit>();
            unitToGameObjectMap = new Dictionary<Unit, GameObject>();
        }


        Hex myHex = GetHexAt(q, r);
        GameObject myHexGO = hexToGameObjectMap[myHex];
        unit.SetHex(myHex);
        GameObject unitGO = GameObject.Instantiate(prefab, myHexGO.transform.position, Quaternion.identity, myHexGO.transform);
        //Register this event as a callback
        unit.OnUnitMoved += unitGO.GetComponent<UnitView>().OnUnitMoved;

        
        units.Add(unit);
        //unitToGameObjectMap[unit] = unitGO;
        unitToGameObjectMap.Add(unit, unitGO);
    }

    public IQPathTile GetTileAt(int x, int y) {
        throw new NotImplementedException();
    }
}
