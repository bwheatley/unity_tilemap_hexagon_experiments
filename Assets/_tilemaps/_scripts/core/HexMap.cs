using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraController;
using CameraController.Util;

public class HexMap : MonoBehaviour {

    public GameObject HexPrefab;
    public Material[] HexMaterials;


    public int numRows = 200;
    public int numColumns = 400;

	// Use this for initialization
	void Start () {
		GenerateMap();
	}

    public void GenerateMap() {

        for (int column = 0; column < numColumns; column++) {
            for (int row = 0; row < numRows; row++) {
         
                Hex h = new Hex(column, row);
                
                //Instantiate a hex
                GameObject hexGO = (GameObject) Instantiate(HexPrefab, h.Position(), Quaternion.identity, this.transform);

                var _hexcomp = hexGO.GetComponent<HexComponent>();
                _hexcomp.Hex = h;
                _hexcomp.HexMap = this;
                
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[Random.Range(0, HexMaterials.Length)];

            }
        }

        //If our map doesn't move we can do this
        //StaticBatchingUtility.Combine(this.gameObject);


    }

}
