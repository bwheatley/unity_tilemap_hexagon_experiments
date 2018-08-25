using System.Collections;
using System.Collections.Generic;
using CameraController.Util;
using UnityEngine;

namespace QPath
{
    
    /// <summary>
    ///
    ///     Tile[] ourPath = QPath.FindPath( ourWorld, startTile, endTile);
    ///
    ///     theUnit is an object that is the thing actually trying to path between tiles
    ///     It might have special logic based on it's movement type and the type of tiles being moved
    ///     through
    /// 
    ///     Our tiles need to be able to return the following information:
    ///     1) List of neighbors
    ///     2) The aggregate cost to enter this tile from another tile
    ///
    ///
    /// 
    /// </summary>
    public static class QPath 
    {

        //public static IEnumerable FindPath(IQPathWorld world, IQPathUnit unit, IQPathTile startTile, IQPathTile endTile)


        public static T[] FindPath<T>(
            IQPathWorld world,
            IQPathUnit unit,
            T startTile,
            T endTile,
            CostEstimateDelegate costEstimateFunc
            ) where T: IQPathTile
        {

            Util.WriteDebugLog(string.Format("QPath::FindPath() run"), GameManager.LogLevel_Error, GameManager.instance.debug, GameManager.instance.LogLevel);

            if (world == null || unit == null | startTile == null || endTile == null) {
                Util.WriteDebugLog(string.Format("Null value passed to QPath::FindPath()"), GameManager.LogLevel_Error, GameManager.instance.debug, GameManager.instance.LogLevel);
                Debug.LogError("QPath::FindPath() ERROR");
                return null;
            }

            // Call on our actual path solver
            QPath_AStar<T> resolver = new QPath_AStar<T>(world, unit, startTile, endTile, costEstimateFunc);

            resolver.DoWork();

            //TODO fix this
            return resolver.GetList();
        }
    }

    //public delegate float TileEnteringCostDelegate();

    public delegate float CostEstimateDelegate(IQPathTile a, IQPathTile b);


}
