using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseTerrain;

public class FixSubChunk : MonoBehaviour
{
    public bool fixTileRules = true;
    public bool ready { get { return CheckReady(); } }
    protected virtual bool  CheckReady()
    {
        return true;
    }
    public virtual void Fix(SubChunk subChunk, int fixTileRuleBorder, TileRules tileRules)
    {
        List<bool> tilesList = subChunk.GetTilesList();
        int width = subChunk.tiles.GetLength(0);
        int height = tilesList.Count / width;
        int count = tilesList.Count - height * 2 - width * 2 + 4;
        int[] indices = new int[count];
        int index = 0;
        int border = fixTileRuleBorder - 1;
        for (int y = border; y < height - border; y += 1)
        {
            for (int x = border; x < width - border; x += 1)
            {
                indices[index] = width * y + x;
                index += 1;
            }
        }
        subChunk.hasInvalid = !Utility.CheckTileRules(subChunk, tileRules);
        Fix(0, indices, tilesList, width, subChunk, tileRules);
    }

        private void Fix(int index, int[] indices, List<bool> tiles, int width, SubChunk subChunk, TileRules tileRules)
        {
            if (!fixTileRules) return;
            if (subChunk.hasInvalid)
            {
                subChunk.hasInvalid = !Utility.CheckTileRules(tiles, width, tileRules);

                if (!subChunk.hasInvalid)
                {
                    Debug.Log("SubChunk fixed");
                    for (int x = 0; x < width; x += 1)
                    {
                        for (int y = 0; y < tiles.Count / width; y += 1)
                        {
                            subChunk.tiles[x, y] = tiles[x + width * y];
                        }
                    }
                }
                else
                {
                    //Debug.Log("SubChunk not fixed");
                    if (index < indices.Length)
                    {
                        if (subChunk.hasInvalid)
                        {
                            List<bool> tilesClone = new List<bool>(tiles);
                            tilesClone[indices[index]] = !tilesClone[indices[index]];
                            Fix(index += 1, indices, tilesClone, width, subChunk, tileRules);
                        }

                        if (subChunk.hasInvalid && index < indices.Length)
                        {
                            List<bool> tilesClone = new List<bool>(tiles);
                            //tilesClone[indices[index + 1]] = !tilesClone[indices[index]];
                            Fix(index += 1, indices, tilesClone, width, subChunk, tileRules);
                        }
                    }

                }
            }

        }
    
}
