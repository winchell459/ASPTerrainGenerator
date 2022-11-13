using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Clingo_02.ClingoHelperJSON;
using UnityEngine;

//public delegate void solverCallback(string clingoOutput);

namespace Clingo_02
{
    public class ClingoSolver : MonoBehaviour
    {
        public bool debugging = false;
        public enum Status { ERROR, SATISFIABLE, UNSATISFIABLE, TIMEDOUT, RUNNING, UNINITIATED, READY, CLINGONOTFOUND, ASPFILENOTFOUND, UNKNOWN }

        //public Fil aspFile = new File();
        public ProcessPriorityClass threadPriority = ProcessPriorityClass.Normal;
        public string aspFilePath = "DataFiles/ASPFiles/queens.txt";
        public string clingoExecutablePathMacOS = "DataFiles/Clingo/clingo";
        public string clingoExecutablePathWin = "DataFiles/Clingo/clingo.exe";
        public string AdditionalArguments = "";
        public int maxDuration = 10; // in seconds
        public bool FindMultipleSolutions = false;
        public int numOfSolutionsWanted = 1; // set to 0 for all possible solution
        public int _seed = 42;
        public int seed { get { return useRandomSeed ? Random.Range(0, 1 << 30) : _seed; } }
        public bool useRandomSeed;
        public bool saveToFile = true;
        protected Dictionary<string, List<List<string>>> answerSetDict = new Dictionary<string, List<List<string>>>();
        public AnswerSet answerSet;

        // Read Only
        protected string aspCode;
        protected int totalSolutionsFound = -1;
        protected bool moreSolutions = false; // Clingo's way to tell us there might be more solutions
        protected double duration; // How long to run clingo
        protected bool isSolverRunning = false;
        protected string solutionOutput;
        protected string clingoConsoleOutput;
        protected string clingoConsoleError;
        protected Status status = Status.UNINITIATED;


        public int Seed { get { return seed; } }
        public bool MoreSolutions { get { return moreSolutions; } }
        public int SolutionsFound { get { return totalSolutionsFound; } }
        public double Duration { get { return duration; } }
        public bool IsSolverRunning { get { return isSolverRunning; } }
        public string SolutionOutput { get { return solutionOutput; } }
        public string ClingoConsoleOutput { get { return clingoConsoleOutput; } }
        public string ClingoConsoleError { get { return clingoConsoleError; } }
        public Status SolverStatus { get { return status; } }

        public Thread clingoThread;

        public virtual void Solve()
        {

        }

        public virtual void Solve(string aspCode, string clingoArguments, bool saveFile)
        {
            this.AdditionalArguments = clingoArguments;
            this.aspCode = aspCode;
            if (saveFile)
            {
                //save aspCode to json file with clingoArguments
                aspFilePath = ClingoUtil.CreateFile(aspCode);
            }
            Solve();
        }

        public  void Solve(string aspfilepath, string clingoArguments)
        {
            this.AdditionalArguments = clingoArguments;
            this.aspFilePath = aspfilepath;
            Solve();
        }


        public void Solve(string aspfilepath)
        {
            this.aspFilePath = aspfilepath;
            this.AdditionalArguments = "";
            Solve();
        }

        public string AnswerSetToString()
        {
            StringBuilder sb = new StringBuilder();

            List<string> keys = new List<string>(answerSetDict.Keys);
            foreach (string key in keys)
            {
                sb.Append(key + ": ");
                foreach (List<string> l in answerSetDict[key])
                {
                    sb.Append("[");
                    foreach (string s in l)
                    {
                        sb.Append(s);
                        sb.Append(" ");
                    }
                    sb.Append("]");
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}