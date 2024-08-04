using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue : MonoBehaviour, IInteractable
{
    public string uniqueKey;
    public int page;

    public void Interact()
    {
        Debug.Log("Clues: " + uniqueKey);
        EventManager.TriggerEvent("ClueCollected", uniqueKey);
        Destroy(gameObject);
    }
}

