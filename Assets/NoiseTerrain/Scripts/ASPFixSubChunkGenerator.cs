using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASPFixSubChunkGenerator : ASPGenerator
{
    public int width, height, fixTileRuleBorder;
    public NoiseTerrain.TileRules tileRules;
    public TerrainChunk.SubChunk subChunk;
    public bool starting, running;

    private void Update()
    {
        
        if (starting)
        {
            //solver.Solve(aspCode, " --parallel-mode 4 ", true);
            InitializeGenerator(SATISFIABLE, UNSATISFIABLE, TIMEDOUT, ERROR);
            startGenerator();
            starting = false;
            running = true;

            Debug.LogWarning($"{subChunk} {width} {height} started");
        }
        WaitingOnClingoUpdate();
    }
    protected override string getASPCode()
    {
        string aspCode = $@"

            #const max_width = {width - 1}.
            #const max_height = {height - 1}.

            width(0..max_width).
            height(0..max_height).

            check_width({fixTileRuleBorder - 1}..{width - fixTileRuleBorder}).
            check_height({fixTileRuleBorder - 1}..{height - fixTileRuleBorder}).

            tile_type(filled;empty).

            1{{tile(XX, YY, Type): tile_type(Type)}}1 :- check_width(XX), check_height(YY).
            
        ";

        for (int x = 0; x < width; x += 1)
        {
            for (int y = 0; y < height; y += 1)
            {
                if ((x < fixTileRuleBorder - 1 || x > width - fixTileRuleBorder) || (y < fixTileRuleBorder - 1 || y > height - fixTileRuleBorder))
                {
                    string tileType = subChunk.tiles[x, y] ? "filled" : "empty";
                    aspCode += $"tile({x},{y},{tileType}). \n";
                }

            }
        }

        aspCode += tileRules.getTileRules();

        return aspCode;
    }

    override protected void SATISFIABLE(Clingo_02.AnswerSet answerSet, string jobID)
    {
        foreach (List<string> tile in solver.answerSet.Value["tile"])
        {
            int x = int.Parse(tile[0]);
            int y = int.Parse(tile[1]);
            string value = tile[2];
            try
            {
                subChunk.tiles[x, y] = value == "filled" ? true : false;
            }
            catch
            {

                Debug.LogWarning($"{width} {height} {x} {y}");
            }

        }
        Debug.LogWarning($"{subChunk} {width} {height} finished");
        running = false;
        
    }

    override protected void UNSATISFIABLE(string jobID)
    {
        Debug.LogWarning(solver.SolverStatus);
        running = false;
    }

    override protected void TIMEDOUT(int time, string jobID)
    {
        running = false;
    }

    override protected void ERROR(string error, string jobID)
    {
        running = false;
    }
}