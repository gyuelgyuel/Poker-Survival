using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPC : MonoBehaviour
{
    public GameObject shopUI;
    public bool isPlayerNear = false;


    void Start()
    {
        Transform npcKey = transform.Find("NPCKey");
        npcKey.gameObject.SetActive(false);
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
            return;
        Transform npcKey = transform.Find("NPCKey");
        float distance = Vector2.Distance(transform.position, player.transform.position);
        
        if (distance < 2f)
            isPlayerNear = true;
        else
            isPlayerNear = false;

        npcKey.gameObject.SetActive(isPlayerNear);

        if (isPlayerNear && Input.GetKeyDown(KeyCode.G))
        {
            ToggleShop();
        }
        
    }
    
    void ToggleShop()
    {
        shopUI.SetActive(!shopUI.activeSelf);
        Time.timeScale = shopUI.activeSelf ? 0f : 1f;
        if(shopUI.activeSelf)
        {
            ShopUIManager.instance.UpdateUI();
        }
    }
}
