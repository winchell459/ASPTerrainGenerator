using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainChunk
{
    public class Edge
    {
        public enum EdgeTypes
        {
            singleJump,
            doubleJump,
            dash,
            climb,
            swim
        }
        public List<EdgeTypes> locomotionTypes;
        public NodeChunk parentChunk, childChunk;
    }
}

