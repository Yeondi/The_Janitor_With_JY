using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    private string worldTimeApi = "https://worldtimeapi.org/api/timezone/Asia/Seoul";

    [SerializeField]
    private TMP_Text text;

    private void Start()
    {
        StartCoroutine(GetWorldTimeAPI());
    }

    IEnumerator GetWorldTimeAPI()
    {
        UnityWebRequest www = UnityWebRequest.Get(worldTimeApi);

        Debug.Log("Initialize When Its done");

        yield return www.SendWebRequest();

        if (www.isDone || www.error == null)
        {
            // it's fine
            string resultText = www.downloadHandler.text;
            Debug.Log("It's done");
            // text.text = temporaryText;
            text.text = www.downloadHandler.text;
            // {"abbreviation":"KST","client_ip":"2001:e60:9146:509b:4cbb:bd31:c4bd:e533","datetime":"2024-05-24T20:18:41.002181+09:00","day_of_week":5,"day_of_year":145,"dst":false,"dst_from":null,"dst_offset":0,"dst_until":null,"raw_offset":32400,"timezone":"Asia/Seoul","unixtime":1716549521,"utc_datetime":"2024-05-24T11:18:41.002181+00:00","utc_offset":"+09:00","week_number":21}
            // resultText.Contains("datetime")
            string startText  = "datetime";
            if (resultText.StartsWith(startText))
            {
                // "datetime":"2024-05-24T20:18:41.002181+09:00"

            }
        }

        yield return null;
    }
}
