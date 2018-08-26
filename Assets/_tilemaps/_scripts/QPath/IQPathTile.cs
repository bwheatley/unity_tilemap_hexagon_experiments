using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QPath
{
    public interface IQPathTile {

        IQPathTile[] GetNeighbors();

        float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit);



    } 

    


}
