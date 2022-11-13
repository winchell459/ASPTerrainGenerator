using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Clingo_02
{
    public class ClingoOnline : ClingoSolver
    {
        private const string clingoServerURL = "https://ancient-cove-68362.herokuapp.com/clingo"; /*"https://clingo.herokuapp.com/clingo";*/

        public string aspSourceFileName = "queens.txt";
        public string optionsFileName = "options.txt";


        // Start is called before the first frame update
        void Start()
        {

            
        }

        public override void Solve()
        {

            status = Status.READY;
            if (status == Status.READY)
            {
                if(debugging) Debug.Log(Status.RUNNING);
                status = Status.RUNNING;
                StartCoroutine(Upload());
            }
        }

        IEnumerator Upload()
        {
            // Get path - You may want to Application.dataPath to persistant data or where ever you save your files.
            string aspSourcePath = Path.Combine(Application.dataPath, aspSourceFileName);
            string optionsPath = Path.Combine(Application.dataPath, optionsFileName);

            // Get text files data
            //byte[] aspData = File.ReadAllBytes(aspSourcePath);
            if (Application.isEditor)
            {
                aspFilePath = Path.Combine(System.Environment.CurrentDirectory, "Assets", aspFilePath);
            }
            else
            {
                aspFilePath = Path.Combine(System.Environment.CurrentDirectory, aspFilePath);
            }
            byte[] aspData = System.Text.Encoding.ASCII.GetBytes(aspCode);//File.ReadAllBytes(aspFilePath);
            //byte[] optionsData = File.ReadAllBytes(optionsPath);
            byte[] optionsData = System.Text.Encoding.ASCII.GetBytes($" --outf=2 --sign-def=rnd --seed={seed} " + AdditionalArguments);

            // Create upload form
            WWWForm form = new WWWForm();
            form.AddBinaryData("src", aspData, Path.GetFileName(aspSourcePath));
            form.AddBinaryData("options", optionsData, Path.GetFileName(optionsPath));

            // Make Post request
            using (UnityWebRequest req = UnityWebRequest.Post(clingoServerURL, form))
            {
                // Send request
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(req.error);
                    status = Status.ERROR;
                }
                else
                {
                    // Success
                    if (debugging) Debug.Log(req.downloadHandler.text);
                    //answerSet = AnswerSet.GetAnswerSet(req.downloadHandler.text);

                    //status = (ClingoSolver.Status)(System.Enum.Parse(typeof(ClingoSolver.Status), answerSet.Result));
                    

                    answerSetStr = req.downloadHandler.text;
                    Thread answersetThread = new Thread(GetAnswerSetThread);
                    answersetThread.Start();
                }
            }
        }
        private string answerSetStr;
        public void GetAnswerSetThread()
        {
            this.answerSet = AnswerSet.GetAnswerSet(answerSetStr);
            status = (ClingoSolver.Status)(System.Enum.Parse(typeof(ClingoSolver.Status), this.answerSet.Result));
        }
    }
}