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

    /// <summary>
    /// Saves the current game data to the save file
    /// </summary>
    public static void SaveData()
    {
        HandleSaveData();
    }

    public static SavedData LoadData()
    {
        return HandleLoadData();
    }

    private static void HandleSaveData()
    {
        savedData = LoadData();

        int score = GameManager.Instance.CurrentRound;
        if (savedData.HighScore <= 0 || savedData.HighScore > score)
            savedData.HighScore = score;

        File.WriteAllText(GetSaveFileName(), JsonUtility.ToJson(savedData, true));
    }

    private static SavedData HandleLoadData()
    {
        // If the save file doesn't exist, create it and return default saved data
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
