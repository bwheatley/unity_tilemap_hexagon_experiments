using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QPath
{

    public interface IQPathWorld {

        IQPathTile GetTileAt(int x, int y);
        //IQPathTile[] GetAllTiles();


    }

}
