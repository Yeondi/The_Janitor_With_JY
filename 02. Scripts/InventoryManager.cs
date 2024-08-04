using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform content; // Content ������Ʈ
    public GameObject buttonPrefab; // ��ư ������
    public GameObject rightSide;
    public TMP_Text textArea;
    public Button prev;
    public Button next;
    public GameObject canvas;

    [SerializeField]
    //private List<List<ClueData>> currentClues = new List<List<ClueData>>();
    private Dictionary<int, List<ClueData>> currentClues = new Dictionary<int, List<ClueData>>();
    private int currentPageIndex = 0;
    [Tooltip("버튼 누른 후 불러온 데이터")]
    private List<ClueData> searchedClueData = new List<ClueData>();

    /*
    데이터 형식
    
    inventory(List<List<ClueData>>)
    
    */

    private void Start()
    {
        // Load clues from ClueManager
        // clues = ClueManager.Instance.LoadJsonData().Result;
        // Debug.Log("Clues ? : ");
        // AddCluesToInventory(clues);
        StartCoroutine(GetLoadedClueData());

        // ��ư Ŭ�� �̺�Ʈ ����
        prev.onClick.AddListener(ShowPreviousPage);
        next.onClick.AddListener(ShowNextPage);
    }

    private IEnumerator GetLoadedClueData()
    {
        yield return new WaitUntil(() => ClueManager.Instance.GetLoadedClue() != null);
    }

    public void AddCluesToInventory(List<ClueData> clues)
    {
        // foreach(var clue in )

        // foreach (var clue in clues)
        // {
        //     AddClue(clue);
        // }


        /*
        id = 1 / page = 5

        id = 2 / page = 1
        id = 2 / page = 2
        id = 2 / page = 4

        id = 4 / page = 1
        */
    }

    public void SwitchInventory()
    {
        if (canvas.activeSelf)
            canvas.SetActive(false);
        else
            canvas.SetActive(true);
    }

    public void AddClue(ClueData clue)
    {
        currentClues[clue.Id].Add(clue);

        CreateContent(clue.Id);
    }

    public void AddClue(int uniqueKey)
    {
        int clueId = uniqueKey / 1000;
        int pageNumber = uniqueKey % 1000;
        bool isNew = false;
        if (!IndexExists(clueId))
        {
            isNew = true;
            currentClues[clueId] = new List<ClueData>();
        }

        ClueData data = ClueManager.Instance.GetSpecificData(clueId,pageNumber);
        //ClueData data = ClueManager.Instance.GetClueByUniqueKey(uniqueKey);

        currentClues[clueId].Add(data);

        if (isNew)
           CreateContent(clueId);
    }

    public bool IndexExists(int index)
    {
        return currentClues.ContainsKey(index);
    }

    private void CreateContent(int clueId)
    {
        GameObject newButton = Instantiate(buttonPrefab, content);
        TMP_Text text = newButton.GetComponentInChildren<TMP_Text>();
        text.text = clueId.ToString();

        // Image image = newButton.GetComponentInChildren<Image>();
        // image.sprite = clueImage;

        newButton.GetComponent<Button>().onClick.AddListener(() => ShowCluePages(clueId));
    }

    void ShowCluePages(int clueId)
    {
        searchedClueData = currentClues[clueId].FindAll(clue => clue.Id == clueId);
        if (searchedClueData.Count < 1)
        {
            Debug.Log("There is no data");
            return;
        }
        searchedClueData.Sort((a, b) => a.Page.CompareTo(b.Page));
        currentPageIndex = 0;
        DisplayCurrentPage();
        rightSide.SetActive(true);
    }

    void ShowPreviousPage()
    {
        if (searchedClueData.Count == 0) return;
        //currentPageIndex = (currentPageIndex - 1 + searchedClueData.Count) % searchedClueData.Count;
        currentPageIndex = (currentPageIndex - 1 < 0) ? currentPageIndex : currentPageIndex - 1;
        DisplayCurrentPage();
    }

    void ShowNextPage()
    {
        if (searchedClueData.Count == 0) return;
        //currentPageIndex = (currentPageIndex + 1) % searchedClueData.Count;
        currentPageIndex = (currentPageIndex + 1 >= searchedClueData.Count) ? currentPageIndex : currentPageIndex + 1;
        DisplayCurrentPage();
    }

    void DisplayCurrentPage()
    {
        if (searchedClueData.Count == 0) return;
        ClueData currentClue = searchedClueData[currentPageIndex];
        textArea.text = $"{currentClue.Id}\n{currentClue.Content}\n\n\n\n\n{currentClue.Page}";
    }
}
