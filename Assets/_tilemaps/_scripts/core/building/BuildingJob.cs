using System;
using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using UnityEngine;
using UnityEngine.UI;

public class BuildingJob {

    public BuildingJob(Image BuildingJobIcon,
        string BuildingJobName,
        float totalProductionNeeded,
        float overFlowedProduction,
        ProductionCompleteDelegate OnProductionComplete,
        ProductionBonusDelegate ProductionBonusFunc
        ) {
        if (OnProductionComplete == null) {
            throw new UnityException();
        }

        this.BuildingJobIcon = BuildingJobIcon;
        this.BuildingJobName = this.BuildingJobName;
        this.totalProductionNeeded = totalProductionNeeded;
        productionDone = overFlowedProduction;
        this.OnProductionComplete = OnProductionComplete;
        this.ProductionBonusFunc = ProductionBonusFunc;
    }

    public float totalProductionNeeded;
    public float productionDone;

    public Image BuildingJobIcon;  //Ex: Image for Hanging Garden
    public string BuildingJobName; //Example: Hanging Gardens


    public delegate void ProductionCompleteDelegate();
    public event ProductionCompleteDelegate OnProductionComplete;

    public delegate float ProductionBonusDelegate();
    public ProductionBonusDelegate ProductionBonusFunc;

    public void DoWork(float rawProduction) {
        if (ProductionBonusFunc != null) {
            rawProduction *= ProductionBonusFunc();
        }

        productionDone += rawProduction;

        if (productionDone >= totalProductionNeeded) {
            OnProductionComplete();
        }

        //TODO: report overflow

    }


}
