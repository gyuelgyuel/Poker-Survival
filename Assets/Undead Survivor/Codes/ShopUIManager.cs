﻿using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    //public PlayerData playerData;
    //public Text playerGoldText;
    public static ShopUIManager instance;
    public GameObject shopUI;
    public GameObject notEnoughChipPanel;
    public GameObject packOpenUI;
    public Text ChipCounts, cardpack1text, cardpack2text,cardpack4text;
    private List<string> tempOptions = new List<string>() {"111","222","333","444","555","666","777","888","999"};
    private int optionAmount = 4;
    private int remainPack = 0;
    private int maxPack = 0;
    public List<string> selectedOptions;

    [Header("Card Pack Buttons")]
    public Button pack1Button;
    public Button pack2Button;
    public Button pack4Button;

    [Header("Card Pack Prices")]
    public int pack1Price = 100;
    public int pack2Price = 200;
    public int pack4Price = 4000;

    [Header("Pack Option Select Buttons")]
    public GameObject packOptionSelectButton1;
    public GameObject packOptionSelectButton2;
    public GameObject packOptionSelectButton3;
    public GameObject packOptionSelectButton4;
    public GameObject skipButton;
    private GameObject[] packOptionSelectButtons;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        pack1Button.onClick.AddListener(() => BuyPack(1));
        pack2Button.onClick.AddListener(() => BuyPack(2));
        pack4Button.onClick.AddListener(() => BuyPack(4));

        packOptionSelectButtons = new GameObject[] { packOptionSelectButton1, packOptionSelectButton2, packOptionSelectButton3, packOptionSelectButton4 };
        foreach (var option in packOptionSelectButtons)
        {
            var capturedOption = option; 
            capturedOption.GetComponent<Button>().onClick.AddListener(() => SelectOption(capturedOption));
        }
        skipButton.GetComponent<Button>().onClick.AddListener(() => SelectOption(skipButton));
    }

    void Update()
    {
        // 숫자키 입력 처리
        if (Input.GetKeyDown(KeyCode.Alpha1))
            BuyPack(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            BuyPack(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            BuyPack(4);

    }
    
    // 카드팩 구매 처리
    void BuyPack(int packCount)
    {
        if (!shopUI.activeSelf)
            return;

        int ItemPrice = GetPrice(packCount);

        if (GameManager.instance.SpendChip(ItemPrice)) {
            remainPack = packCount;
            maxPack = packCount;
            UpdateUI();
            PackOpen(remainPack);
        }
        
    }
    
    int GetPrice(int packCount)
    {
        switch (packCount)
        {
            case 1: return pack1Price;
            case 2: return pack2Price;
            case 4: return pack4Price;
            default: return 0;
        }
    }

    public void UpdateUI()
    {
        // 칩개수 변경
        GameObject chipObj = GameObject.Find("ChipCounts");
        ChipCounts = chipObj.GetComponent<Text>();
        ChipCounts.text = $"Chips : {GameManager.instance.chipCount}";

        // 카드팩 설명 변경
        GameObject cardpack1 = GameObject.Find("cardpack1text");
        GameObject cardpack2 = GameObject.Find("cardpack2text");
        GameObject cardpack4 = GameObject.Find("cardpack4text");
        GameObject[] cardpacklists = { cardpack1, cardpack2, cardpack4 };
        for (int i = 0; i < cardpacklists.Length; i++)
        {
            Text textobj = cardpacklists[i].GetComponent<Text>();
            int packCount = (i == 0) ? 1 : (i == 1) ? 2 : 4;
            textobj.text = $"카드팩\n{packCount}개\n필요칩 : {GetPrice(packCount)}";
        }
    }
    

    public void ShowNotEnoughChips()
    {
        if (!notEnoughChipPanel)
            return;

        notEnoughChipPanel.SetActive(true);
        StopAllCoroutines(); // 중복 호출 방지
        StartCoroutine(HideAfterDelay());
    }

    public void HideNotEnoughChip()
    {
        notEnoughChipPanel.SetActive(false);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        notEnoughChipPanel.SetActive(false);
    }

    private void PackOpen(int packCount) {
        if (remainPack == 0) {
            packOpenUI.SetActive(false);
            shopUI.SetActive(true);
            return;
        }
        
        if (!packOpenUI.activeSelf) { 
            packOpenUI.SetActive(true); 
            shopUI.SetActive(false);
        }
        packOpenUI.transform.GetChild(0).GetComponent<Text>().text = $"카드팩 개봉 ({maxPack-remainPack+1}/{maxPack})";
        ShowPackOptions();
    
    }

    private void ShowPackOptions() { 
        List<int> options = new List<int>();
        
        for (int i = 0; i < optionAmount; i++) { 
            int randomInt = Random.Range(0, tempOptions.Count - 1);
            while (options.Contains(randomInt)) {
                randomInt = Random.Range(0, tempOptions.Count - 1);
            }
            options.Add(randomInt);
        }
        for (int i = 0; i < packOptionSelectButtons.Length; i++) {
            Text textobj = packOptionSelectButtons[i].transform.GetChild(1).GetComponent<Text>();
            textobj.text = tempOptions[options[i]];
        }

    }

    private void SelectOption(GameObject btn) {
        if (btn.name != "SkipChoice") { selectedOptions.Add(btn.transform.GetChild(1).GetComponent<Text>().text); }
        remainPack -= 1;
        PackOpen(remainPack);

    }





}