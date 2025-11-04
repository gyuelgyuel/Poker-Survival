using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject shopUI;
    public GameObject npcKey; // G 키 안내 표시용 오브젝트

    private bool isPlayerNear = false;
    private bool isShopOpen = false;

    void Start()
    {
        // 시작 시 키 UI는 비활성화
        if (npcKey != null)
            npcKey.SetActive(false);
    }

    void Update()
    {
        // 플레이어가 근처에 있고 G를 눌렀을 때만 상호작용
        if (isPlayerNear && Input.GetKeyDown(KeyCode.G))
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        if (shopUI == null) return;

        isShopOpen = !isShopOpen;
        shopUI.SetActive(isShopOpen);

        // Shop UI가 열리면 시간 정지, 닫으면 시간 재개
        Time.timeScale = isShopOpen ? 0f : 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (npcKey != null)
                npcKey.SetActive(true); // "G" 키 안내 표시
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            if (npcKey != null)
                npcKey.SetActive(false);

            if (shopUI != null)
                shopUI.SetActive(false);
        }
    }
}
