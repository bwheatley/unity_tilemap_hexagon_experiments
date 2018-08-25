using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMap_continents : HexMap {
    private bool are;

    public override void GenerateMap() {
        //Generate all the hexes we need
        base.GenerateMap();

        int numContinents = 4;
        int continentSpacing = numColumns / numContinents;

        for (int c = 0; c < numContinents; c++) {
            //Make some kind of raised area
            //float[,] elevation = new float[numColumns, numRows];
            int numSplats = Random.Range(4, 8);
            for (int i = 0; i < numSplats; i++)
            {
                int range = Random.Range(5, 10);
                int y = Random.Range(range, numRows - range);
                int x = Random.Range(0, 10) - y / 2 + (c* continentSpacing);

                ElevateArea(x, y, range);
            }
        }

        //Add lumpiness with perlin
        GeneratePerlinNoise(0);

        //Set mesh to mountain/hill/flat/water

        //Simulate rainfall/moisture
        GeneratePerlinNoise(1);

        //Now make sure hex visuals are updated to match the data
        UpdateHexVisuals();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameter">0=Elevation 1=Moisture</param>
    void GeneratePerlinNoise(int parameter=0) {
        float noiseResolution = 0.05f;
        float noiseScale = 1.5f;  //Large noise scale will create more islands/peaks
        float noiseErosion = 0.5f;
        Vector2 noiseOffset = new Vector2(Random.Range(0,1f), Random.Range(0, 1f));

        //Test seeding
        Random.InitState(0);

        for (int column = 0; column < numColumns; column++) {
            for (int row = 0; row < numRows; row++) {
                Hex h = GetHexAt(column, row);
                
                float n = Mathf.PerlinNoise(
                    ((float)column / Mathf.Max(numColumns, numRows) / noiseResolution) + noiseOffset.x,
                    ((float)row / Mathf.Max(numColumns, numRows) / noiseResolution) + noiseOffset.y
                ) - noiseErosion;

                if (parameter == 0)
                {
                    h.Elevation += n * noiseScale;
                }
                else if (parameter == 1)
                {
                    h.Moisture = n * noiseScale;
                }

            }
        }
    }

    void ElevateArea(int q, int r, int range, float centerHeight = .95f) {

        Hex centerHex = GetHexAt(q, r);
        //centerHex.Elevation = 0.5f;

        Hex[] areaHexes = GetHexesWithinRangeOf(centerHex, range);


        for (int x = 0; x < areaHexes.Length; x++) {
            var h = areaHexes[x];
            //if (h.Elevation < 0) {
            //    h.Elevation = 0;
            //}

            h.Elevation = centerHeight * Mathf.Lerp(1f, 0.25f, Mathf.Pow(Hex.Distance(centerHex, h) / range, 2f));

        }



    }
}
