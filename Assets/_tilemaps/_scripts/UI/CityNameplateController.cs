using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityNameplateController : MonoBehaviour {

    public GameObject CityNameplatePrefab;



    //TODO add city handlers for city destruction etc.
	// Use this for initialization
	void Start () {
	    GameObject.FindObjectOfType<HexMap>().OnCityCreated += CreateCityNamePlate;
	}

    void OnDestroy() {
        //GameObject.FindObjectOfType<HexMap>().OnCityCreated -= CreateCityNamePlate;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void CreateCityNamePlate(City city, GameObject cityGO) {

        GameObject nameGO = (GameObject) Instantiate(CityNameplatePrefab, this.transform);
        nameGO.GetComponent<MapObjectNamePlate>().MyTarget = cityGO;

    }





}
