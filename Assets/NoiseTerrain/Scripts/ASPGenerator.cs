using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ASPGenerator : MonoBehaviour
{
    [SerializeField] protected int cpus = 4, timeout = 100;
    [SerializeField] protected Clingo_02.ClingoSolver solver;

    //[SerializeField] protected ASPMap.ASPMap map;
    //[SerializeField] protected ASPMap.ASPMapKey mapKey;
    protected bool waitingOnClingo;

    protected System.Action<Clingo_02.AnswerSet, string> satifiableCallBack;
    protected System.Action<string> unsatifiableCallBack;
    protected System.Action<int, string> timedoutCallBack;
    protected System.Action<string, string> errorCallBack;

    protected string filename;
    protected string jobID = "";

    [SerializeField] bool runOnStart = false;
    private void Start()
    {
        if (runOnStart)
        {
            InitializeGenerator(SATISFIABLE, UNSATISFIABLE, TIMEDOUT, ERROR);
            startGenerator();
        }

    }
    // Update is called once per frame
    void Update()
    {
        WaitingOnClingoUpdate();
    }

    protected void WaitingOnClingoUpdate()
    {
        if (waitingOnClingo)
        {
            if (solver.SolverStatus == Clingo_02.ClingoSolver.Status.SATISFIABLE)
            {
                finalizeGenerator();
                satifiableCallBack(solver.answerSet, jobID);
                //waitingOnClingo = false;

            }
            else if (solver.SolverStatus == Clingo_02.ClingoSolver.Status.UNSATISFIABLE)
            {
                finalizeGenerator();
                unsatifiableCallBack(jobID);
                //waitingOnClingo = false;

            }
            else if (solver.SolverStatus == Clingo_02.ClingoSolver.Status.ERROR || solver.SolverStatus == Clingo_02.ClingoSolver.Status.UNKNOWN)
            {
                finalizeGenerator();
                errorCallBack(solver.ClingoConsoleError, jobID);
                //waitingOnClingo = false;

            }
            else if (solver.SolverStatus == Clingo_02.ClingoSolver.Status.TIMEDOUT)
            {
                finalizeGenerator();
                timedoutCallBack(timeout, jobID);
                //waitingOnClingo = false;

            }

            //if (!waitingOnClingo) finalizeGenerator();
        }
    }


    public void StartGenerator(string jobID)
    {
        this.jobID = jobID;
        initializeGenerator();
        startGenerator();
    }

    private string aspCode { get { return getASPCode(); } }
    protected string aspCodeCoroutine = "";

    //protected virtual void setASPCodePreThread()
    //{

    //}

    virtual protected string getASPCode()
    {
        string aspCode = @"

                #const max_width = 40.
                #const max_height = 40.

                width(1..max_width).
                height(1..max_height).

                tile_type(filled;empty).

                1{tile(XX, YY, Type): tile_type(Type)}1 :- width(XX), height(YY).

                :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2, Y1 != Y2.
                :- tile(X1,Y1, filled), tile(X2,Y2,filled), Y1 == Y2, X1 != X2.

                :- Count = {tile(_,_,filled)}, Count != max_width.
  
                :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2 + Offset, Y1 == Y2 + Offset, width(Offset).
                :- tile(X1,Y1, filled), tile(X2,Y2,filled), X1 == X2 + Offset, Y1 == Y2 - Offset, width(Offset).

            ";

        return aspCode;
    }
    public void InitializeGenerator(System.Action<Clingo_02.AnswerSet, string> satifiableCallBack, System.Action<string> unsatifiableCallBack, System.Action<int, string> timedoutCallBack, System.Action<string, string> errorCallBack)
    {
        this.satifiableCallBack = satifiableCallBack;
        this.unsatifiableCallBack = unsatifiableCallBack;
        this.timedoutCallBack = timedoutCallBack;
        this.errorCallBack = errorCallBack;

    }
    public void InitializeGenerator(int cpus, int timeout)
    {
        this.cpus = cpus;
        this.timeout = timeout;
    }

    virtual protected void initializeGenerator()
    {
        //filename = Clingo.ClingoUtil.CreateFile(aspCode);
        solver.maxDuration = timeout + 10;
    }

    virtual protected void startGenerator()
    {

        //solver.Solve(aspCode, getAdditionalParameters(), false);
        //waitingOnClingo = true;
        StartCoroutine(startGeneratorCoroutine());
    }

    virtual protected IEnumerator startGeneratorCoroutine()
    {
        Debug.Log("unimplemented startGeneratorCoroutine");
        yield return null;

        startGeneratorThreadComplete = false;
        Thread thread = new Thread(startGeneratorThread);
        thread.Start();

        while (!startGeneratorThreadComplete) yield return null;
        solver.Solve(aspCodeThread + aspCodeCoroutine, additionalParametersThread, false);
        waitingOnClingo = true;
    }

    bool startGeneratorThreadComplete = false;
    string additionalParametersThread = "";
    string aspCodeThread = "";

    virtual protected void startGeneratorThread()
    {
        aspCodeThread = aspCode;
        additionalParametersThread = getAdditionalParameters();
        startGeneratorThreadComplete = true;
    }

    virtual protected void finalizeGenerator()
    {
        waitingOnClingo = false;
    }

    virtual protected string getAdditionalParameters()
    {
        return $" --parallel-mode {cpus} --time-limit={timeout}";
    }

    virtual protected void SATISFIABLE(Clingo_02.AnswerSet answerSet, string jobID)
    {

        Debug.LogWarning("SATISFIABLE unimplemented");
    }

    virtual protected void UNSATISFIABLE(string jobID)
    {
        Debug.LogWarning("UNSATISFIABLE unimplemented");
    }

    virtual protected void TIMEDOUT(int time, string jobID)
    {
        Debug.LogWarning("TIMEDOUT unimplemented");
    }

    virtual protected void ERROR(string error, string jobID)
    {
        Debug.LogWarning("ERROR unimplemented");
    }
}