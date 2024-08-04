using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance { get; private set; }

    public AssetReference jsonFileReference; // Addressable �ý����� JSON ���� ����
    public AssetLabelReference clueLabel; // Addressable �ý����� �ܼ� ���̺�

    private List<ClueData> loadedClues;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private async void Start()
    {
        loadedClues = await LoadJsonData();
        List<ClueData> addressableClues = await LoadAddressableClues();
        //CompareClues(addressableClues);
    }

    public async Task<List<ClueData>> LoadJsonData()
    {
        List<ClueData> clues = new List<ClueData>();
        AsyncOperationHandle<TextAsset> handle = jsonFileReference.LoadAssetAsync<TextAsset>();
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset jsonData = handle.Result;
            ClueList clueList = JsonUtility.FromJson<ClueList>("{\"Clues\":" + jsonData.text + "}");
            clues = clueList.Clues;
        }

        Addressables.Release(handle);
        return clues;
    }

    public async Task<List<ClueData>> LoadAddressableClues()
    {
        List<ClueData> clues = new List<ClueData>();
        // TextAsset�� ����Ͽ� Ŭ�� �����͸� �ε�
        AsyncOperationHandle<IList<TextAsset>> handle = Addressables.LoadAssetsAsync<TextAsset>(clueLabel, null);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var textAsset in handle.Result)
            {
                // JSON �迭�� �Ľ��Ͽ� ClueData ����Ʈ�� ��ȯ
                ClueList clueList = JsonUtility.FromJson<ClueList>("{\"Clues\":" + textAsset.text + "}");
                clues.AddRange(clueList.Clues);
            }
        }

        Addressables.Release(handle);
        return clues;
    }


    public void CompareClues(List<ClueData> addressableClues)
    {
        foreach (var jsonClue in loadedClues)
        {
            foreach (var addressableClue in addressableClues)
            {
                if (jsonClue.Id == addressableClue.Id)
                {
                    Debug.Log($"Match Found: ID = {jsonClue.Id}, Content = {jsonClue.Content}, Page = {jsonClue.Page}");
                }
            }
        }
    }

    public void CompareClues(List<ClueData> addressableClues, int collectedClueId)
    {
        List<ClueData> matchingClues = new List<ClueData>();
        foreach (var clue in addressableClues)
        {
            if (clue.Id == collectedClueId)
            {
                matchingClues.Add(clue);
            }
        }

        //Debug.Log($"Matching Clues for ID {collectedClueId}:");
        foreach (var clue in matchingClues)
        {
            Debug.Log($"ID: {clue.Id}, Type: {clue.Type}, Content: {clue.Content}, Page: {clue.Page}");
        }
    }

    public List<ClueData> GetLoadedClue()
    {
        return loadedClues;
    }

    public ClueData GetSpecificData(int clueId)
    {
        ClueData item = null;

        foreach(var clue in loadedClues)
        {
            if (clue.Id == clueId)
            {
                item = clue;
                break;
            }
        }

        return item;
    }



    [System.Serializable]
    private class ClueList
    {
        public List<ClueData> Clues;
    }
}