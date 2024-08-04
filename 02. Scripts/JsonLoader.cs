using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonLoader
{
    public static List<ClueData> LoadClueData(string path)
    {
        string json = File.ReadAllText(path);
        ClueList clueList = JsonUtility.FromJson<ClueList>(json);
        return clueList.Clues;
    }

    [System.Serializable]
    private class ClueList
    {
        public List<ClueData> Clues;
    }
}
