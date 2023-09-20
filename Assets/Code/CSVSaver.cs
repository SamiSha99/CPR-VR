using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
public class CSVSaver
{
    static string evaulationDate = "1_1_1970_00_00_00";
    static string newEvaluationText;
    static string fileName;
    static readonly string pathDirectory = Application.persistentDataPath + "/ExamResults/";
    static string fullPathToSave;

    public struct EvaulationData
    {
        public float finalMark;
        public double timetaken;
        public List<string> penalty_names;
        public List<float> penalty_scores;
        public EvaulationData(float finalMark, double timetaken, List<GameManager.ExamPenalty> penalties)
        {
            this.finalMark = finalMark;
            this.timetaken = timetaken;
            penalty_names = new List<string>();
            penalty_scores = new List<float>();

            foreach (GameManager.ExamPenalty p in penalties)
            {
                penalty_names.Add(p.penaltyName);
                penalty_scores.Add(p.penaltyAmount);
            }
        }
    }

    private static void SetPathAndDirectory()
    {
        evaulationDate = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
        fileName = evaulationDate + ".csv";
        fullPathToSave = pathDirectory + fileName;
        if(!Directory.Exists(pathDirectory))
        {
            Directory.CreateDirectory(pathDirectory);
        }
    }

    public static void SaveData(float finalMark, List<GameManager.ExamPenalty> recievedPenalties, double timetaken, bool hasPassed)
    {
        SetPathAndDirectory();

        EvaulationData ed = new EvaulationData(finalMark, timetaken, recievedPenalties);
        
        newEvaluationText = "";

        if(evaulationDate != "1_1_1970_00_00_00")
        {
            newEvaluationText += $"final mark:,{ed.finalMark}\n";
            newEvaluationText += $"has passed?:,{hasPassed}\n";
            newEvaluationText += $"time taken:,{ed.timetaken}\n";
            newEvaluationText += $",\n";
            newEvaluationText += "mistakes, mistakes amount\n";
            for(int i = 0; i < ed.penalty_names.Count && i < ed.penalty_scores.Count; i++)
                newEvaluationText += ed.penalty_names[i] + "," + string.Format("{0:0.##}", ed.penalty_scores[i]) + "\n";
            CreateTextFile();
        }
        else
        {
            Util.Print("FAILED TO SAVE DEFAULT FILE, INVALID DATE FORMAT!", Util.PrintType.Error);
        }
    }

    static void CreateTextFile()
    {
        File.WriteAllLines(fullPathToSave, newEvaluationText.Split("\n"));
        Util.Print($"Saved {fileName} to: <a href='{pathDirectory}'>{pathDirectory}</a>", Util.PrintType.Save);
    }

    /// <summary>Still need to debug this stuff. 😂😂😭😭</summary>
    public static void DEBUG_GenerateDataTesT()
    {
        List<GameManager.ExamPenalty> penalties = new()
        {
            new("Dies from cringe", 5),
            new("Touched his nose (ew!)", 10),
            new("Doesn't like teahcer", 2),
            new("skill issue?!? o.o? 🤨📸", 25)
        };

        SaveData(100, penalties, 330.10, false);
    }
}
