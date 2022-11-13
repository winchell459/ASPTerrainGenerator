using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Clingo_02.ClingoHelperJSON;
using UnityEngine;

namespace Clingo_02
{
    public class ClingoLocal : ClingoSolver
    {
        // Private
        // The path has to be set in the main thread
        //public Thread clingoThread;
        private static string[] trueArray = { "true" };
        private Process clingoProcess;


        public override void Solve()
        {
            SetUpProcess();

            if (status == Status.READY)
            {
                status = Status.RUNNING;
                clingoThread.Start();
            }
        }
        public override void Solve(string aspCode, string clingoArguments, bool saveFile)
        {
            this.AdditionalArguments = clingoArguments;
            this.aspCode = aspCode;
            aspFilePath = ClingoUtil.CreateFile(aspCode);
            Solve();
        }

        //public override void Solve(string aspfilepath, string clingoArguments)
        //{
        //    this.AdditionalArguments = clingoArguments;
        //    this.aspFilePath = aspfilepath;
        //    Solve();
        //}


        //public override void Solve(string aspfilepath)
        //{
        //    this.aspFilePath = aspfilepath;
        //    this.AdditionalArguments = "";
        //    Solve();
        //}


        private void MyThread()
        {
            SolveHelper();
            if (status == Status.SATISFIABLE)
            {
                solutionOutput = AnswerSetToString();
            }
        }


        


        private bool startedOutPutReading = false;

        private void SolveHelper()
        {
            clingoProcess.Start();

            clingoProcess.PriorityClass = threadPriority;


            if (!startedOutPutReading)
            {
                startedOutPutReading = true;
            }
            clingoProcess.BeginOutputReadLine();



            print("Wating");
            if (clingoProcess.WaitForExit(maxDuration * 1000))
            {
                print("finished in time");
            }
            else
            {
                clingoProcess.Kill();
                status = Status.TIMEDOUT;
                UnityEngine.Debug.LogWarning("Clingo Timedout.");
            }



            //            clingoConsoleOutput = clingoProcess.StandardOutput.ReadToEnd();
            clingoConsoleError = clingoProcess.StandardError.ReadToEnd();



            if (status == Status.TIMEDOUT)
            {
                clingoProcess.CancelOutputRead();
                clingoProcess.Close();
                clingoProcess.OutputDataReceived -= OutputDataReceived;

                return;
            }

            print("clingoConsoleError");
            print(clingoConsoleError);

            print("clingoConsoleOutput");
            print(clingoConsoleOutput);
            clingoProcess.CancelOutputRead();
            clingoProcess.Close();
            clingoProcess.OutputDataReceived -= OutputDataReceived;


            if (clingoConsoleError.Length > 0)
            {
                print("clingoConsoleError");
                print(clingoConsoleError);
                print(clingoConsoleError.Contains("INTERRUPTED by signal!"));

                if (clingoConsoleError.Contains("INTERRUPTED by signal!") || clingoConsoleError.Contains("solving stopped by signal"))
                {
                    status = Status.TIMEDOUT;
                }
                else
                {
                    status = Status.ERROR;
                }

                isSolverRunning = false;
                UnityEngine.Debug.Log("Solver is Done.");
                return;
            }


            ClingoRoot clingoOutput = JsonUtility.FromJson<ClingoRoot>(clingoConsoleOutput);
            //answerSet2 = JsonUtility.FromJson<AnswerSet>(clingoConsoleOutput);
            answerSet = AnswerSet.GetAnswerSet(clingoConsoleOutput);

            if (clingoOutput == null)
            {
                status = Status.ERROR;
            }
            else
            {
                if (clingoOutput.Result == "SATISFIABLE")
                {
                    status = Status.SATISFIABLE;
                }
                else if (clingoOutput.Result == "UNSATISFIABLE")
                {
                    status = Status.UNSATISFIABLE;
                }
                else if (clingoOutput.Result == "UNKNOWN")
                {
                    status = Status.ERROR;
                }

                if (status == Status.SATISFIABLE)
                {
                    var values = clingoOutput.Call[0].Witnesses[0].Value;


                    foreach (string value in values)
                    {
                        int start = value.IndexOf('(');
                        int end = value.IndexOf(')');

                        if (start < 0 || end < 0)
                        {
                            string key = value;
                            if (!answerSetDict.ContainsKey(key))
                            {
                                answerSetDict.Add(key, new List<List<string>>());
                            }

                            answerSetDict[key].Add(new List<string>(trueArray));
                        }
                        else
                        {
                            string key = value.Substring(0, start);
                            string keyValue = value.Substring(start + 1, end - start - 1);

                            if (!answerSetDict.ContainsKey(key))
                            {
                                answerSetDict.Add(key, new List<List<string>>());
                            }

                            string[] body = keyValue.Split(',');
                            answerSetDict[key].Add(new List<string>(body));

                        }
                    }
                }

                totalSolutionsFound = clingoOutput.Models.Number;
                moreSolutions = !clingoOutput.Models.More.Equals("no");
                duration = clingoOutput.Time.Total;
            }
            isSolverRunning = false;
            UnityEngine.Debug.Log("Solver is Done.");
        }

        //public void StopSolver()
        //{
        //    if (clingoThread != null && clingoThread.IsAlive)
        //    {
        //        if (clingoProcess != null)
        //        {
        //            clingoProcess.Kill();
        //        }
        //    }
        //}


        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            clingoConsoleOutput += e.Data + "\n";
            //print(e.Data);
        }


        private bool SetUpProcess()
        {
            if (status == Status.RUNNING)
            {
                UnityEngine.Debug.LogWarning("The Solver is already running.");
                return false;
            }

            status = Status.UNINITIATED;

            //if (useRandomSeed)
            //{
            //    seed = Random.Range(0, 1 << 30);
            //}

            if (aspFilePath == string.Empty)
            {
                UnityEngine.Debug.LogError("No ASP File.");
                status = Status.ASPFILENOTFOUND;
                return false;
            }

            //string path = Path.Combine(Application.dataPath, clingoExecutablePathMacOS);
            string clingopath = "";

            if (Application.isEditor)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    clingopath = Path.Combine(System.Environment.CurrentDirectory, "Assets", clingoExecutablePathWin);
                }
                else if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    clingopath = Path.Combine(System.Environment.CurrentDirectory, "Assets", clingoExecutablePathMacOS);
                }
            }
            else
            {
                if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    clingopath = Path.Combine(System.Environment.CurrentDirectory, clingoExecutablePathWin);
                }
                else if (Application.platform == RuntimePlatform.OSXPlayer)
                {
                    clingopath = Path.Combine(System.Environment.CurrentDirectory, clingoExecutablePathMacOS);
                }
            }

            //outputclingopath = clingopath;

            if (!File.Exists(clingopath))
            {
                UnityEngine.Debug.LogError("Clingo is missing.");
                status = Status.CLINGONOTFOUND;
                print(clingopath);
                return false;
            }

            //aspFilePath = AssetDatabase.GetAssetPath(aspFile);

            if (clingoThread == null) { clingoThread = new Thread(MyThread); }
            else if (clingoThread.IsAlive)
            {
                UnityEngine.Debug.LogWarning("Thread State while Alive: " + clingoThread.ThreadState.ToString());
                clingoThread = new Thread(MyThread);
            }
            else
            {
                clingoThread = new Thread(MyThread);
            }




            if (clingoProcess == null) { clingoProcess = new Process(); }
            clingoProcess.StartInfo.FileName = clingopath;
            UpdateClingoASPArguments();
            clingoProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            clingoProcess.StartInfo.UseShellExecute = false;
            clingoProcess.StartInfo.CreateNoWindow = true;
            clingoProcess.StartInfo.RedirectStandardOutput = true;
            clingoProcess.StartInfo.RedirectStandardError = true;

            //clingoProcess.OutputDataReceived += ((sender, e) =>
            //{
            //    clingoConsoleOutput += e.Data + "\n";
            //});

            clingoProcess.OutputDataReceived += OutputDataReceived;



            if (maxDuration < 1) { maxDuration = 10; } // 10 sec
            if (numOfSolutionsWanted < 0) { numOfSolutionsWanted = 1; }

            solutionOutput = "";
            clingoConsoleOutput = "";
            clingoConsoleError = "";
            duration = 0;
            totalSolutionsFound = -1;
            answerSetDict.Clear();


            status = Status.READY;
            return true;
        }


        private void UpdateClingoASPArguments()
        {
            string arguments = " --outf=2 ";

            string filepath = "";

            if (Application.isEditor)
            {
                filepath = Path.Combine(System.Environment.CurrentDirectory, "Assets", aspFilePath);
            }
            else
            {
                filepath = Path.Combine(System.Environment.CurrentDirectory, aspFilePath);
            }

            //outputasppath = filepath;
            //string clingopath = Path.Combine(System.Environment.CurrentDirectory, "DataFiles/Clingo/clingo");
            //string path = Path.Combine(Application.dataPath, aspFilePath);

            //arguments += aspFilePath + " ";
            arguments += "\"" + filepath + "\" ";

            if (FindMultipleSolutions)
            {
                arguments += numOfSolutionsWanted.ToString() + " "; // 0 to show all answers
            }
            arguments += "--sign-def=rnd --seed=" + seed;
            arguments += " " + AdditionalArguments;

            //outputarguments = arguments;
            clingoProcess.StartInfo.Arguments = arguments;
        }
    }
}
