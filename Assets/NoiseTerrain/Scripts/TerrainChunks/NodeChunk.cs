using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainChunk
{
    public abstract class NodeChunk 
    {
        public int nodeID;
        protected List<Vector2Int> movementTiles = new List<Vector2Int>();
        protected List<Edge> edges = new List<Edge>();
        public virtual bool AddEdge(Edge edge)
        {
            if (edges.Contains(edge))
            {
                return false;
            }
            else
            {
                edges.Add(edge);
                return true;
            }
        }

        public bool ContainsMovementTile(Vector2Int movementTile)
        {
            return movementTiles.Contains(movementTile);
        }

        public void AddMovementTile(Vector2Int movementTile)
        {
            if (!ContainsMovementTile(movementTile)) movementTiles.Add(movementTile);
        }

        public void SetMovementTiles(List<Vector2Int> movementTiles)
        {
            this.movementTiles = movementTiles;
        }
        public List<Vector2Int> GetMovementTiles()
        {
            return movementTiles;
        }
    }
}