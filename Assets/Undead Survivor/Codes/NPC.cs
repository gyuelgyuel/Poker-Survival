using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    public GameObject shopUI;
    private bool isPlayerNear = false;

    
    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.G))
        {
            ToggleShop();
        }
        
    }
    
    void ToggleShop()
    {
        shopUI.SetActive(!shopUI.activeSelf);
        Time.timeScale = shopUI.activeSelf ? 0f : 1f;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}
