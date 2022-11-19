using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;

namespace NoiseTerrain
{
    public class ProceduralMapGenerator : MapGenerator
    {
        public Vector2Int chunkID;
        public Vector2Int _chunkRadius = new Vector2Int(5, 4);
        public Vector2Int _tileRulesRadius = new Vector2Int(4, 3);
        public Vector2Int _tileRulesFixRadius = new Vector2Int(3, 2);
        public Vector2Int tileRadius = new Vector2Int(2, 1);

        private Vector2Int chunkRadius { get { return _chunkRadius + _tileRulesRadius + _tileRulesFixRadius; } }
        private Vector2Int tileRulesRadius { get { return _tileRulesRadius + _tileRulesFixRadius; } }
        private Vector2Int tileRulesFixRadius { get { return _tileRulesFixRadius; } }

        public List<Vector2Int> visibleChunkIDs = new List<Vector2Int>();
        public List<Vector2Int> toFixChunkIDs = new List<Vector2Int>();
        public List<Vector2Int> toDisplayChunks = new List<Vector2Int>();

        public Transform target;

        
        public TileRules tileRules;

        List<Chunk> chunks = new List<Chunk>();
        //public bool debugFixTileRules;
        public bool fixTileRules;
        Thread handleFixTileRulesThread;
        public FixSubChunk fixSubChunk;
        private void OnDestroy()
        {
            fixTileRules = false;
            fixSubChunk.fixTileRules = fixTileRules;
            handleFixTileRulesThread.Abort();
            Debug.Log("Exit");
        }
        private void Start()
        {
            handleFixTileRulesThread = new Thread(HandleFixTileRulesThread);
            handleFixTileRulesThread.Start();
            //fixTileRules = true;
        }
        private void Update()
        {
            HandleMouseClick();
            Vector2Int chunkID = GetChunkID(target.position);

            for (int i = 0; i < visibleChunkIDs.Count; i += 1)
            {
                Chunk visibleChunk = GetChunk(visibleChunkIDs[i]);
                if (visibleChunk.valueChanged)
                {
                    if (!toDisplayChunks.Contains(visibleChunkIDs[i]))
                    {
                        //toDisplayChunks.Add(visibleChunkIDs[i]);
                        GenerateMap(visibleChunkIDs[i]);
                    }
                        
                    visibleChunk.valueChanged = false;
                }
            }
            //display tiles
            for (int i = toDisplayChunks.Count - 1; i >= 0; i -= 1)
            {
                lock (toFixChunkIDs)
                {
                    if (!toFixChunkIDs.Contains(toDisplayChunks[i]))
                    {
                        //Debug.Log($"Displaying chunk {toDisplayChunks[i]}");
                        GenerateMap(toDisplayChunks[i]);
                        visibleChunkIDs.Add(toDisplayChunks[i]);
                        toDisplayChunks.RemoveAt(i);
                    }
                }

            }
            DisplayMap(chunkID);
        }

        public Chunk GetChunk(Vector2Int chunkID)
        {
            foreach (Chunk chunk in chunks)
            {
                if (chunk.chunkID == chunkID) return chunk;
            }
            return null;
        }

        public Vector2Int GetChunkID(Vector2 pos)
        {
            int xOffset = pos.x < 0 ? -1 : 0;
            int yOffset = pos.y > 0 ? 1 : 0;

            return new Vector2Int(xOffset + (int)pos.x / width, -yOffset - (int)pos.y / height);
        }

        public void DisplayMap(Vector2Int chunkID)
        {
            if (chunkID != this.chunkID)
            {
                
                lock (toFixChunkIDs)
                {
                    this.chunkID = chunkID;
                    //layer 1 largest layer
                    List<Vector2Int> toBuildChunks = new List<Vector2Int>();
                    for (int x = -chunkRadius.x - tileRadius.x; x <= chunkRadius.x + tileRadius.x; x += 1)
                    {
                        for (int y = -chunkRadius.y - tileRadius.y; y <= chunkRadius.y + tileRadius.y; y += 1)
                        {

                            toBuildChunks.Add(chunkID + new Vector2Int(x, y));
                        }
                    }

                    //layer 2 second largest layer
                    List<Vector2Int> toCheckTileRulesChunks = new List<Vector2Int>();
                    for (int x = -tileRulesRadius.x - tileRadius.x; x <= tileRulesRadius.x + tileRadius.x; x += 1)
                    {
                        for (int y = -tileRulesRadius.y - tileRadius.y; y <= tileRulesRadius.y + tileRadius.y; y += 1)
                        {
                            toCheckTileRulesChunks.Add(chunkID + new Vector2Int(x, y));
                        }
                    }

                    //layer 3
                    List<Vector2Int> toFixTileRulesChunks = new List<Vector2Int>();
                    for (int x = 0; x <= tileRulesFixRadius.x + tileRadius.x; x += 1)
                    {
                        for (int y = 0; y <= tileRulesFixRadius.y + tileRadius.y; y += 1)
                        {
                            if (fixTileRules)
                            {
                                toFixTileRulesChunks.Add(chunkID + new Vector2Int(x, y));
                                if(x != 0) toFixTileRulesChunks.Add(chunkID + new Vector2Int(-x, y));
                                if(y != 0) toFixTileRulesChunks.Add(chunkID + new Vector2Int(x, -y));
                                if (x!= 0 && y != 0) toFixTileRulesChunks.Add(chunkID + new Vector2Int(-x, -y));
                            }
                        }
                    }



                    // layer ?? visible layer smallest layer
                    List<Vector2Int> toDisplayChunks = new List<Vector2Int>();
                    for (int x = -tileRadius.x; x <= tileRadius.x; x += 1)
                    {
                        for (int y = -tileRadius.y; y <= tileRadius.y; y += 1)
                        {
                            toDisplayChunks.Add(chunkID + new Vector2Int(x, y));
                        }
                    }

                    //find chunks to be removed
                    for (int i = visibleChunkIDs.Count - 1; i >= 0; i -= 1)
                    {

                        if (!toDisplayChunks.Contains(visibleChunkIDs[i]) && !this.toDisplayChunks.Contains(visibleChunkIDs[i]))
                        {
                            //Debug.LogWarning($"Removing {visibleChunkIDs[i]}");
                            ClearMap(visibleChunkIDs[i]);
                            visibleChunkIDs.RemoveAt(i);
                            
                        }
                        else if (!toDisplayChunks.Contains(visibleChunkIDs[i]))
                        {
                            //Debug.LogWarning($"Removing 2 {visibleChunkIDs[i]} {this.toDisplayChunks.Remove(visibleChunkIDs[i])}"); // must remove next line for debug
                            this.toDisplayChunks.Remove(visibleChunkIDs[i]);
                            visibleChunkIDs.RemoveAt(i);
                            
                        }
                    }

                    //build chunks
                    for (int i = 0; i < toBuildChunks.Count; i += 1)
                    {
                        Chunk chunk = GetChunk(toBuildChunks[i]);
                        if (chunk == null)
                        {
                            int minX = toBuildChunks[i].x * width;
                            int maxX = (toBuildChunks[i].x + 1) * width - 1;
                            int minY = toBuildChunks[i].y * height;
                            int maxY = (toBuildChunks[i].y + 1) * height - 1;

                            chunk = new Chunk(toBuildChunks[i], GenerateBoolMap(minX, maxX, minY, maxY), this);
                            chunks.Add(chunk);
                        }
                    }

                    //check tileRules
                    for (int i = 0; i < toCheckTileRulesChunks.Count; i += 1)
                    {
                        Chunk chunk = GetChunk(toCheckTileRulesChunks[i]);
                        Utility.CheckTileRules(chunk,tileRules);
                        // Debug.Log($"Checking Chunk {chunk.chunkID}");
                    }

                    //fix tileRules
                    for (int i = 0; i < toFixTileRulesChunks.Count; i += 1)
                    {
                        if (!toFixChunkIDs.Contains(toFixTileRulesChunks[i]) && !visibleChunkIDs.Contains(toFixTileRulesChunks[i]) && !this.toDisplayChunks.Contains(toFixTileRulesChunks[i]))
                        {
                            Chunk chunk = GetChunk(toFixTileRulesChunks[i]);
                            Utility.CheckTileRules(chunk,tileRules); // need to check in case invalid were fixed in an overlapping subchunk
                            if (chunk.hasInvalidTile)
                            {

                                toFixChunkIDs.Add(toFixTileRulesChunks[i]);

                            }
                        }

                    }

                    for (int i = 0; i < toDisplayChunks.Count; i += 1)
                    {
                        if (!this.toDisplayChunks.Contains(toDisplayChunks[i]) && !visibleChunkIDs.Contains(toDisplayChunks[i]))
                        {
                            this.toDisplayChunks.Add(toDisplayChunks[i]);
                        }
                    }

                }

                Utility.SortToChunkIDs(chunkID, toFixChunkIDs);

            }


        }

        

        public override void GenerateMap()
        {
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;
            GenerateMap(minX, maxX, minY, maxY);
        }
        public void GenerateMap(Vector2Int chunkID)
        {
            Chunk chunk = GetChunk(chunkID);
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;

            for (int x = 0; x < width; x += 1)
            {
                for (int y = 0; y < height; y += 1)
                {
                    bool[] neighbors = chunk.GetTileNeighbors(x, y);
                    if (neighbors != null)
                    {
                        TileBase tile = tileRules.GetSprite(neighbors);
                        if (tile == null)
                        {
                            tile = fullTile;
                        }
                        fullTilemap.SetTile(new Vector3Int(x + minX, -y - minY, 0), tile);
                    }
                    else
                    {
                        fullTilemap.SetTile(new Vector3Int(x + minX, -y - minY, 0), null);
                    }

                }
            }
            
            chunk.BuildChunk(seed);
        }

        public void ClearMap(Vector2Int chunkID)
        {
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;
            ClearMap(minX, maxX, minY, maxY);
            GetChunk(chunkID).ClearChunk();
        }

        public override void ClearMap()
        {
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;
            ClearMap(minX, maxX, minY, maxY);
        }

        public bool[,] GenerateBoolMap(Vector2Int chunkID)
        {
            int minX = chunkID.x * width;
            int maxX = (chunkID.x + 1) * width - 1;
            int minY = chunkID.y * height;
            int maxY = (chunkID.y + 1) * height - 1;

            return GenerateBoolMap(minX, maxX, minY, maxY);
        }

        Vector2Int lastClickChunkID;
        public GameObject playerPrefab;
        public enum HandleMouseClickFuction {resetChunk,placePlayer,selectPlatform,generatePath}
        public HandleMouseClickFuction clickFuction;
        private void HandleMouseClick()
        {
            if (clickFuction == HandleMouseClickFuction.generatePath)
            {
                if (roomChunk != null && Input.GetMouseButtonUp(0))
                {
                    // Vector2Int clickChunkID = GetChunkID(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    Vector2Int clickTile = new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                    if (roomChunk.GetPlatformID(clickTile) == 0)
                    {
                        roomChunk.PrintPath(clickTile,3);
                    }

                }
            }
            else if (clickFuction == HandleMouseClickFuction.placePlayer)
            {
                if (roomChunk != null && Input.GetMouseButtonUp(0))
                {
                    // Vector2Int clickChunkID = GetChunkID(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    Vector2Int clickTile = new Vector2Int((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                    if (roomChunk.GetPlatformID(clickTile) == 0)
                    {
                        Vector2 spawn = new Vector2(clickTile.x + 0.5f, clickTile.y + 0.5f);
                        Instantiate(playerPrefab, spawn, Quaternion.identity);
                    }
                    
                }
            }else if(clickFuction == HandleMouseClickFuction.resetChunk)
            {
                Vector2Int clickChunkID = GetChunkID(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                if (Input.GetMouseButton(0) && (lastClickChunkID == null || lastClickChunkID != clickChunkID))
                {
                    Chunk clickedChunk = GetChunk(clickChunkID);
                    if (clickedChunk != null)
                    {
                        Debug.Log($"resetting {clickChunkID} chunk");
                        bool[,] resetBoolMap = GenerateBoolMap(clickChunkID);
                        clickedChunk.SetTiles(resetBoolMap);
                        visibleChunkIDs.Remove(clickChunkID);
                        toFixChunkIDs.Add(clickChunkID);
                        toDisplayChunks.Add(clickChunkID);
                    }
                    lastClickChunkID = clickChunkID;
                }
            }else if (clickFuction == HandleMouseClickFuction.selectPlatform)
            {
                if (roomChunk != null && Input.GetMouseButtonUp(0))
                {
                    Vector2Int clickTile = new Vector2Int ((int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), (int)Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y));
                    int platformID = roomChunk.GetPlatformID(clickTile);
                    string id = "";
                    if (platformID > 9)
                        id += (char)((int)'A' + platformID - 10);
                    else
                        id += platformID;
                    Debug.Log($"{id} -> {platformID}");
                    
                }
            }



        }
        RoomChunk roomChunk;
        public void SetRoomChunk()
        {
            List<Chunk> visibleChunks = new List<Chunk>();
            foreach (Vector2Int visibleChunkID in visibleChunkIDs)
            {
                visibleChunks.Add(GetChunk(visibleChunkID));
            }
            roomChunk = new RoomChunk(visibleChunks);
        }
        public void ClearRoomChunk()
        {

        }

        private void HandleFixTileRulesThread()
        {
            while (fixTileRules)
            {
                //if (!fixSubChunk.ready)
                //{
                //    //Debug.LogWarning("Waiting for fixSubChunk.ready");
                //    continue;
                //}
                Vector2Int chunkID = Vector2Int.zero;
                bool chunkIDFound = false;
                lock (toFixChunkIDs)
                {
                    if (toFixChunkIDs.Count > 0)
                    {
                        chunkID = toFixChunkIDs[0];
                        chunkIDFound = true;
                    }

                }
                if (chunkIDFound)
                {
                    Chunk chunk = GetChunk(chunkID);
                    if (chunk != null)
                    {
                        lock (chunk)
                        {
                            Utility.CheckTileRules(chunk,tileRules); // need to check in case invalid were fixed in an overlapping subchunk
                            if (chunk.hasInvalidTile)
                            {
                                HandleFixTileRules(chunk);

                            }
                        }

                        lock (toFixChunkIDs)
                        {
                            toFixChunkIDs.RemoveAt(0);
                            chunk.hasInvalidTile = false;
                        }
                    }

                }

            }

        }

        public int fixTileRuleBorder = 2;
        private void HandleFixTileRules(Chunk chunk)
        {
            Debug.Log($"Fixing Chunk {chunk.chunkID}");
            List<SubChunk> subChunks = chunk.GetInvalidSubChunks(fixTileRuleBorder);
            Debug.Log($"chunkID = {chunk.chunkID} : subChunks.Count = {subChunks.Count}");
            foreach (SubChunk subChunk in subChunks)
            {
                if (Mathf.Abs(chunkID.x - chunk.chunkID.x) <= tileRadius.x && Mathf.Abs(chunkID.y - chunk.chunkID.y) <= tileRadius.y)
                {
                    subChunk.PrintTiles();
                }
            }
            if (!fixTileRules)
            {
                return;
            }
            foreach (SubChunk subChunk in subChunks)
            {
                fixSubChunk.Fix(subChunk, fixTileRuleBorder, tileRules);
                while (!fixSubChunk.ready)
                {/*waiting for fixSubChunk to be done*/ }
                if (!subChunk.hasInvalid)
                {
                    subChunk.PrintTiles();
                    for (int y = 0; y < subChunk.tiles.GetLength(1); y += 1)
                    {
                        for (int x = 0; x < subChunk.tiles.GetLength(0); x += 1)
                        {
                            //Debug.Log($"{x}x{y} = {subChunk.tiles[x, y]}");
                            chunk.SetTile(x + subChunk.minX, y + subChunk.minY, subChunk.tiles[x, y]);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("TileRule not fixed");
                }
            }
        }

        

    }

    
}