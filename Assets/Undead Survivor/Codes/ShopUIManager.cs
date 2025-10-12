using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    //public PlayerData playerData;
    //public Text playerGoldText;

    [Header("Card Pack Buttons")]
    public Button pack1Button;
    public Button pack2Button;
    public Button pack4Button;

    [Header("Card Pack Prices")]
    public int pack1Price = 100;
    public int pack2Price = 200;
    public int pack4Price = 400;

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        pack1Button.onClick.AddListener(() => BuyPack(1));
        pack2Button.onClick.AddListener(() => BuyPack(2));
        pack4Button.onClick.AddListener(() => BuyPack(4));

        //UpdateUI();
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
        Debug.Log($"카드팩 {packCount}구매");
        /*
        int price = GetPrice(packCount);

        if (playerData.SpendGold(price))
        {
            // 카드팩 지급
            CardPackManager.Instance.GivePack(playerData, packCount);

            // 재화 사용 정보 전달 가능 (다른 스크립트에서 PlayerData 참조)
            Debug.Log($"✅ 카드팩 {packCount}개 구매 완료! 소비 골드: {price}");

            UpdateUI();
        }
        else
        {
            Debug.Log("❌ 골드가 부족합니다.");
        }
        */
    }
    /*
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

    void UpdateUI()
    {
        playerGoldText.text = $"Gold: {playerData.gold}";
    }
    */
}