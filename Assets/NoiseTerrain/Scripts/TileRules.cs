using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseTerrain
{
    [CreateAssetMenu(fileName = "TileRules", menuName = "ScriptableObjects/ASPTileRules")]
    public class TileRules : ScriptableObject
    {
        public bool debugging = false;
        [System.Serializable]
        public struct TileRule
        {
            public string name { get { return tileSprite.name; } }
            public TileNeighbors.State[] neighbors;

            public UnityEngine.Tilemaps.TileBase tileSprite;
        }

        public TileRule[] Tiles;

        public virtual UnityEngine.Tilemaps.TileBase GetSprite(bool[] neighbors)
        {
            UnityEngine.Tilemaps.TileBase sprite = null;
            foreach (TileRule tileRule in Tiles)
            {
                if (isMatching(tileRule, neighbors) && !sprite) sprite = tileRule.tileSprite;
                else if (isMatching(tileRule, neighbors)) Debug.LogWarning("Multiple sprites matching.");

                if (sprite) break;
            }

            if (sprite == null) Debug.LogWarning("Tile missing");
            return sprite;
        }

        public virtual bool GetValidTile(bool[] neighbors)
        {
            bool isValid = false;
            foreach (TileRule tileRule in Tiles)
            {
                if (isMatching(tileRule, neighbors) && !isValid) isValid = true;
                else if (isMatching(tileRule, neighbors)) Debug.LogWarning("Multiple sprites matching.");
  
            }

            return isValid;
        }

        bool isMatching(TileRule tile, bool[] neighbors)
        {
            
            bool match = true;
            for (int i = 0; i < 8; i += 1)
            {
                if (tile.neighbors[i] != TileNeighbors.State.none && neighbors[i] != (tile.neighbors[i] == TileNeighbors.State.filled)) match = false;
            }
            return match;
        }

        public virtual string getTileRules()
        {
            List<bool[]> missingRules = getMissingRules(Tiles);

            if (debugging) Debug.Log("missingRules.Count: " + missingRules.Count);
            string aspCode = "";

            foreach (bool[] missingTile in missingRules)
            {
                aspCode += $@"

                :- tile(XX,YY,{getTileType("filled")}),
                {getNot(missingTile[0])} tile(XX-1, YY+1, {getTileType("empty")}),
                {getNot(missingTile[1])} tile(XX, YY+1, {getTileType("empty")}),
                {getNot(missingTile[2])} tile(XX+1, YY+1, {getTileType("empty")}),
                {getNot(missingTile[3])} tile(XX-1, YY, {getTileType("empty")}),
                {getNot(missingTile[4])} tile(XX+1, YY, {getTileType("empty")}),
                {getNot(missingTile[5])} tile(XX-1, YY-1, {getTileType("empty")}),
                {getNot(missingTile[6])} tile(XX, YY-1, {getTileType("empty")}),
                {getNot(missingTile[7])} tile(XX+1, YY-1, {getTileType("empty")}).


            ";
            }

            if (debugging) Debug.Log(aspCode);

            return aspCode;
        }

        protected virtual string getTileType(string tileType)
        {
            return tileType;
        }

        string getNot(bool isEmpty)
        {
            if (isEmpty) return "not";
            else return "";
        }


        protected List<bool[]> getMissingRules(TileRule[] tileRules)
        {
            List<bool[]> missingRules = new List<bool[]>();
            for (int i = 0; i < 256; i += 1)
            {
                bool[] permutation = getPermutation(i);
                //string debug = "";
                //for(int j = 0; j < 8; j += 1)
                //{
                //    debug += permutation[j] + ", ";
                //}
                //Debug.Log(debug);
                bool missing = true;
                foreach (TileRule tileRule in tileRules)
                {
                    bool found = true;
                    for (int j = 0; j < 8; j += 1)
                    {


                        if (tileRule.neighbors[j] != TileNeighbors.State.none && permutation[j] != (tileRule.neighbors[j] == TileNeighbors.State.filled)) found = false;
                    }
                    if (found) missing = false;
                }
                if (missing) missingRules.Add(permutation);
            }

            return missingRules;
        }

        protected bool[] getPermutation(int num)
        {
            bool[] permutation = new bool[8];
            int index = 7;
            while (index >= 0)
            {
                int placeValue = num / (int)Mathf.Pow(2, index);
                if (placeValue == 1) permutation[index] = true;
                num = num % (int)Mathf.Pow(2, index);
                index -= 1;
            }
            return permutation;
        }
    }

    [System.Serializable]
    public class TileNeighbors
    {
        public enum State
        {
            none,
            filled,
            empty
        }
        public State[] neighbors = new State[8];
    }
}

