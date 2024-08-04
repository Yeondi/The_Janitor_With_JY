using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue : MonoBehaviour, IInteractable
{
    public string clueId;
    public int page;

    public void Interact()
    {
        Debug.Log("Clues: " + clueId);
        EventManager.TriggerEvent("ClueCollected", clueId);
        Destroy(gameObject);
    }
}

