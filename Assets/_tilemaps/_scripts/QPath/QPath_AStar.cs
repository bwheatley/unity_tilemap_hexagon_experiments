using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraController.Util;

namespace QPath
{
    //TODO make this not publically accessible
    public class QPath_AStar<T> where T : IQPathTile
    {

        private Queue<T> path;
        private IQPathWorld world;
        private IQPathUnit unit;
        private T startTile;
        private T endTile;
        private CostEstimateDelegate costEstimateFunc;



        public QPath_AStar(
            IQPathWorld world, 
            IQPathUnit unit, 
            T startTile,
            T endTile,
            CostEstimateDelegate costEstimateFunc
            ) 
        {

            //Do Setup
            this.world = world;
            this.unit = unit;
            this.startTile = startTile;
            this.endTile = endTile;
            this.costEstimateFunc = costEstimateFunc;


            //Do we need to explicitely create a graph?
        }


        public void DoWork() {
            path = new Queue<T>();

            Util.WriteDebugLog(string.Format("QPath_AStar::DoWork() run"), GameManager.LogLevel_Error, GameManager.instance.debug, GameManager.instance.LogLevel);
            //List for all our nodes
            HashSet<T> closedSet = new HashSet<T>();
            PathfindingPriorityQueue<T> openSet = new PathfindingPriorityQueue<T>();

            openSet.Enqueue(startTile,0);


            Dictionary<T, T> came_From = new Dictionary<T, T>();

            //Includes real cost
            Dictionary<T, float> g_score = new Dictionary<T, float>();
            g_score[startTile] = 0;

            //Includes estimated cost
            Dictionary<T, float> f_score = new Dictionary<T, float>();
            f_score[startTile] = costEstimateFunc(startTile, endTile);


            //Main loop
            while (openSet.Count > 0) {

                T current = openSet.Dequeue();

                //Check to see if we are there -- see if they are the same object in memory
                if (Object.ReferenceEquals(current, endTile)) {
                    Reconstruct_path(came_From, current);
                    return;
                }

                closedSet.Add(current);

                foreach (T edge_neighbor in current.GetNeighbors()) {
                    T neighbor = edge_neighbor;

                    if (closedSet.Contains(neighbor)) {
                        continue; //ignore this already completed neighbor
                    }

                    //float pathfinding_cost_to_neighbor = neighbor. ;

                    float total_pathfinding_cost_to_neighbor = neighbor.AggregateCostToEnter(g_score[current], current, unit);

                    //Check for impassible terrain - Less then 0 is impassible terrain
                    if (total_pathfinding_cost_to_neighbor < 0) {
                        continue;
                    }


                    float tentative_g_score = total_pathfinding_cost_to_neighbor;

                    /*
                     *Is the neighbor already in the open set?
                     * If so , and if this new score is worse than the old score
                     * discard this new result
                     */
                    if (openSet.Contains(neighbor) && tentative_g_score >= g_score[neighbor]) {
                        continue;
                    }
                     
                    //This is either a new tile or we just found a cheaper route to it
                    came_From[neighbor] = current;
                    g_score[neighbor] = tentative_g_score;
                    f_score[neighbor] = g_score[neighbor] + costEstimateFunc(neighbor, endTile);

                    openSet.EnqueueOrUpdate(neighbor, f_score[neighbor]);

                } //foreach neighbor



            }



        }

        private void Reconstruct_path(
            Dictionary<T, T> came_From,
            T current
        ) {
            // So at this point, current IS the goal
            // So what we want to do is walk backwards through the Came_from
            // map, until we reach the "end" of that map...which will be
            // our starting node!
            Queue<T> total_path = new Queue<T>();
            total_path.Enqueue(current);

            while (came_From.ContainsKey(current)) {
                /* came_From is a map, where the key => value
                 * relation is real saying
                 * some_node => we_got_there_from_this_node
                 */

                current = came_From[current];
                total_path.Enqueue(current);

            }

            /* At this point, total_path is a queue that is running
             * backwards from the END tile to the START tile, so lets reverse it
             */
            path = new Queue<T>(total_path.Reverse());

        }



        public T[] GetList() {
            return path.ToArray();
        }


    }
}
