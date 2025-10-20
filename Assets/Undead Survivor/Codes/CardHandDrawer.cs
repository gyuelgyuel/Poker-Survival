using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandDrawer : MonoBehaviour
{
    [Header("Deck Sprites (원본)")]
    public List<Sprite> deck;                 // deck에 스프라이트들을 담아두세요.

    [Header("현재 손패(뽑힌 5장) - runtime")]
    public Sprite[] slots = new Sprite[5];    // G키로 랜덤 뽑을 때 채워집니다.

    // CardContainer의 자식(Card1~Card5)의 Image 캐시
    private Image[] childImages;

    void Awake()
    {
        AutoBindDirectChildImages(); 
        if (slots == null || slots.Length < 5)
            slots = new Sprite[5];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
                DrawRandomCardsNoDuplicate();                // deck에서 5장 랜덤(기존 구조 유지)
                ApplySlotsToChildren();           // slots -> Card1~Card5 적용
        }
    }

    /// Card1~Card5를 이름으로 찾아 Image 캐싱
    void AutoBindDirectChildImages()
    {
        var list = new List<Image>();

        // CardContainer의 "직계 자식"만 순서대로 수집
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var img = child.GetComponent<Image>(); // 자식 오브젝트에 직접 붙은 Image만
            if (img != null) list.Add(img);
        }

        childImages = list.ToArray();
    }

    /// deck에서 5장 랜덤 뽑아 slots에 채우기 (중복 비허용)
    public void DrawRandomCardsNoDuplicate()
    {
        if (deck == null || deck.Count == 0) { Debug.LogWarning("Deck이 비었습니다."); return; }
        if (slots == null || slots.Length < 5) slots = new Sprite[5];

        var pool = new List<Sprite>(deck);
        int drawCount = Mathf.Min(5, Mathf.Min(slots.Length, pool.Count));

        for (int i = 0; i < drawCount; i++)
        {
            int idx = Random.Range(0, pool.Count);
            slots[i] = pool[idx];
            pool.RemoveAt(idx); // 뽑은 카드는 제거
        }
    }

    /// slots의 내용을 Card1~Card5 Image에 반영
    public void ApplySlotsToChildren()
    {
        if (childImages == null || childImages.Length == 0) return;

        int count = Mathf.Min(5, Mathf.Min(slots?.Length ?? 0, childImages.Length));
        for (int i = 0; i < count; i++)
        {
            var img = childImages[i];
            if (img == null) continue;

            img.sprite = slots[i];
            img.enabled = slots[i] != null;
        }
    }
}