using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitSelectionPanel : MonoBehaviour {

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Movement;
    public TextMeshProUGUI HexPath;

    private MouseController mouseController;

    public GameObject CityBuildButton;

	// Use this for initialization
	void Start () {
	    mouseController = GameObject.FindObjectOfType<MouseController>();
	}
	
	// Update is called once per frame
	void Update () {

	    if (mouseController.SelectedUnit != null ) {
	        Title.text = string.Format("{0}",mouseController.SelectedUnit.Name);
	        Movement.text = string.Format(
	            "{0}/{1}",
	            mouseController.SelectedUnit.MovementRemaining,
	            mouseController.SelectedUnit.Movement);

	        Hex[] hexPath = mouseController.SelectedUnit.GetHexPath();
            HexPath.text = string.Format("{0}", hexPath == null ? "0" : hexPath.Length.ToString());

            CityBuildButton.SetActive(mouseController.SelectedUnit.CanBuildCities);


	    }

	}
}
