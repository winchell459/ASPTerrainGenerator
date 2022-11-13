using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Clingo_02
{
    [System.Serializable]
    public class AnswerSet
    {
        public string Solver;
        public string[] Input;
        public Call Call;
        public string Result;
        public Time Time;
        public Value Value { get { return Call.Witnesses.Value; } }

        public static AnswerSet GetAnswerSet(string json)
        {
            AnswerSet answerSet = JsonUtility.FromJson<AnswerSet>(json);
            if(answerSet.Result == ClingoSolver.Status.SATISFIABLE.ToString())
            {
                answerSet.Call.Witnesses = new Witnesses(json);
            }
            
            return answerSet;
        }
        protected virtual void getAnswerSet(string json)
        {

        }

        protected virtual void getAnswerSet()
        {

        }

        protected List<List<string>> getValues()
        {
            List<List<string>> values = new List<List<string>>();
            foreach (string value in Call.Witnesses.Value.RawValue)
            {
                List<string> sequenceList = new List<string>();
                string sequence = "";
                foreach (char symbol in value)
                {
                    if (symbol == ',' || symbol == '(' || symbol == ')')
                    {
                        sequenceList.Add(sequence);
                        sequence = "";
                    }
                    else if (symbol != '.')
                    {
                        sequence += symbol;
                    }
                }
                values.Add(sequenceList);
            }
            return values;
        }
    }

    //[System.Serializable]
    //public class Solver
    //{

    //}

    //[System.Serializable]
    //public class Input
    //{

    //}

    [System.Serializable]
    public class Call
    {
        public Witnesses Witnesses;
        //public string[] Witnesses;


    }

    [System.Serializable]
    public class Witnesses
    {
        public Witnesses(string json)
        {
            json = findWitnessesJson(json);
            Debug.Log(json);

            //Value = JsonUtility.FromJson<string[]>(json);
            //Debug.Log(Value.Length);
            Value = new Value(json);
        }
        public Value Value;

        private string findWitnessesJson(string json)
        {
            string witnessesJson = "";
            int start = json.IndexOf("Witnesses");
            //Debug.Log($"Witnesses: {start}");
            bool started = false;
            int openCount = 0;
            while (start < json.Length)
            {
                if (!started)
                {
                    if (json[start] == '{')
                    {
                        started = true;
                        witnessesJson += json[start];
                        openCount += 1;
                    }
                }
                else
                {
                    if (json[start] != ' ') witnessesJson += json[start];

                    if (json[start] == '{') openCount += 1;
                    if (json[start] == '}') openCount -= 1;
                }
                json = json.Replace("\n", "").Replace("\r", "");
                if (started && openCount <= 0) break;
                start += 1;
            }

            return witnessesJson;
        }
    }

    [System.Serializable]
    public class Value
    {
        public List<string> RawValue = new List<string>();
        public Value(string json)
        {
            findValueStrings(json);
        }

        public List<List<string>> this[string key]
        {
            get => FindValue(key);
        }

        List<List<string>> FindValue(string key)
        {
            List<List<string>> values = new List<List<string>>();
            foreach (List<string> item in getValues(RawValue))
            {
                if (item[0] == key)
                {
                    List<string> value = new List<string>();
                    for (int i = 1; i < item.Count; i += 1)
                    {
                        value.Add(item[i]);
                    }
                    values.Add(value);
                }
            }
            return values;
        }
        protected List<List<string>> getValues(List<string> rawValues)
        {
            List<List<string>> values = new List<List<string>>();
            foreach (string value in rawValues)
            {
                List<string> sequenceList = new List<string>();
                string sequence = "";
                foreach (char symbol in value)
                {
                    if (symbol == ',' || symbol == '(' || symbol == ')')
                    {
                        sequenceList.Add(sequence);
                        sequence = "";
                    }
                    else if (symbol != '.')
                    {
                        sequence += symbol;
                    }
                }
                values.Add(sequenceList);
            }
            return values;
        }

        private void findValueStrings(string json)
        {
            int start = json.IndexOf("Value");
            int quoteCount = -1;
            bool started = false;
            string nextValue = "";
            while (start < json.Length)
            {
                if (quoteCount > 0 && json[start] != '"')
                {
                    nextValue += json[start];
                }
                else if (quoteCount > 0 && json[start] == '"')
                {
                    quoteCount -= 1;
                    RawValue.Add(nextValue);
                    nextValue = "";
                }
                else if (quoteCount <= 0 && json[start] == '"')
                {
                    quoteCount += 1;
                    if (!started)
                    {
                        started = true;
                    }
                }


                start += 1;
            }
        }
    }

    [System.Serializable]
    public class Result
    {

    }

    [System.Serializable]
    public class Time
    {
        public double Total;
        public double Solve;
        public double Model;
        public double Unsat;
        public double CPU;

    }
}