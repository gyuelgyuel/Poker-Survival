using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public static NPC instance;
    public GameObject shopUI;
    public GameObject npcKey; // G 키 안내 표시용 오브젝트

    private bool isPlayerNear = false;
    public bool isShopOpen = false;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        // 시작 시 키 UI는 비활성화
        if (npcKey != null)
            npcKey.SetActive(false);
    }

    void Update()
    {
        // F키 → 상호작용: 플레이어가 근처일 때만 동작
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            OpenShop();
        }

        // ESC키 → 열려 있을 때만 닫기, pack 개봉 중에는 shopUI 먼저 닫기 불가
        if (isShopOpen && !ShopUIManager.instance.isOpenPack && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {
        if (shopUI == null) return;

        isShopOpen = true;
        shopUI.SetActive(true);

        GameManager.instance.Stop();
        //Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        if (shopUI == null) return;

        isShopOpen = false;
        shopUI.SetActive(false);

        GameManager.instance.Resume();
        //Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (npcKey != null)
                npcKey.SetActive(true); // "F" 키 안내 표시
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
