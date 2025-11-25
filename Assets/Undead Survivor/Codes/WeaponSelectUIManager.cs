using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectUIManager : MonoBehaviour
{
    public static WeaponSelectUIManager instance;
    public GameObject selectUI;
    public CardHandDrawer_AlphaCrossfadeFlip cardContainer;

    public Card[] cards = new Card[5];

    public Item[] weapons = new Item[2];

    void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.T))
        {
            selectUI.SetActive(!selectUI.activeSelf);
            if (selectUI.activeSelf)
            {
                unclickAllCards();
                GameManager.instance.Stop();
            }
            else
            {
                unclickAllCards();
                GameManager.instance.Resume();
            }

        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (selectUI.activeSelf)
            {
                // 무기 계산, 손패 초기화, 게임 재개
                calcWeaponWithCheckedCard();
            }
        }

        // 숫자키 입력 처리
        if (selectUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                cards[0].OnClickItem();
            if (Input.GetKeyDown(KeyCode.Alpha2))
                cards[1].OnClickItem();
            if (Input.GetKeyDown(KeyCode.Alpha3))
                cards[2].OnClickItem();
            if (Input.GetKeyDown(KeyCode.Alpha4))
                cards[3].OnClickItem();
            if (Input.GetKeyDown(KeyCode.Alpha5))
                cards[4].OnClickItem();
        }
    }

    public void setCard(int index, CardData cardData)
    {
        // selectUI의 card 변경
        cards[index].setCardData(cardData);
    }

    public void unsetCard(int index)
    {
        // selectUI의 card 변경
        cards[index].unsetCardData();
    }

    public void unsetAllCards()
    {
        // selectUI의 card 변경
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].unsetCardData();
        }
    }

    public void unclickAllCards()
    {
        // selectUI의 card 변경
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].isClicked)
                cards[i].OnClickItem();
        }
    }

    public void calcWeaponWithCheckedCard()
    {
        // 선택한 카드 확인
        List<CardData> clickedCardDatas = new List<CardData>();
        foreach (var card in cards)
        {
            if (card.isClicked)
                clickedCardDatas.Add(card.data);
        }

        // 카드풀 바탕으로 무기 선택 로직
        // 현재는 임시로 검정, 빨강 수에 따라 근거리, 원거리 선택
        int blackCount = 0;
        int redCount = 0;
        for (int i = 0; i < clickedCardDatas.Count; i++)
        {
            if (clickedCardDatas[0].cardType == CardData.CardType.Spade || clickedCardDatas[0].cardType == CardData.CardType.Club) blackCount++;
            else redCount++;
        }

        if (blackCount > redCount) // 근접
        {
            weapons[0].OnClickItem();
        }
        else // 원거리
        {
            weapons[1].OnClickItem();
        }

        // 손패 초기화
        cardContainer.resetHands();
        unsetAllCards();
        // 게임 재개
        selectUI.SetActive(false);
        GameManager.instance.Resume();
    }
}
