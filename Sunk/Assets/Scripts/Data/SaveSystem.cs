using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem
{
    public struct SavedData
    {
        public int HighScore;
    }

    private static SavedData savedData = new SavedData();

    public static string GetSaveFileName()
    {
        string saveFileName = Application.persistentDataPath + "/save" + ".save";
        return saveFileName;
    }

    public static void SaveData()
    {
        HandleSaveData();
    }

    private static void HandleSaveData()
    {
        int score = GameManager.Instance.CurrentRound;
        if (savedData.HighScore > 0 && savedData.HighScore > score)
            savedData.HighScore = score;

        File.WriteAllText(GetSaveFileName(), JsonUtility.ToJson(savedData, true));
    }

    public static SavedData LoadData()
    {
        return HandleLoadData();
    }

    private static SavedData HandleLoadData()
    {
        if (!File.Exists(GetSaveFileName()))
        {
            File.Create(GetSaveFileName());
            return savedData;
        }

        string savedDataString = File.ReadAllText(GetSaveFileName());

        if(savedDataString != "")
            savedData = JsonUtility.FromJson<SavedData>(savedDataString);

        return savedData;
    }
}
