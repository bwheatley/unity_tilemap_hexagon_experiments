using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap : MonoBehaviour {

    public GameObject HexPrefab;
    public Material[] HexMaterials;


	// Use this for initialization
	void Start () {
		GenerateMap();
	}

    public void GenerateMap() {

        for (int column = 0; column < 10; column++) {
            for (int row = 0; row < 10; row++) {
         
                Hex h = new Hex(column, row);
                
                //Instantiate a hex
                GameObject hexGO = (GameObject) Instantiate(HexPrefab, h.Position(), Quaternion.identity, this.transform);

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.material = HexMaterials[Random.Range(0, HexMaterials.Length)];

            }
        }

        //If our map doesn't move we can do this
        //StaticBatchingUtility.Combine(this.gameObject);


    }

}
