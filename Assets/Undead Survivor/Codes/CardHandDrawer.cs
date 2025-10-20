using System.Collections;                  // IEnumerator, Coroutine
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandDrawer : MonoBehaviour
{
    [Header("Deck / Sprites")]
    public List<Sprite> deck;               // 덱: 앞면 스프라이트들
    public Sprite backSprite;               // 기본 뒷면 스프라이트

    [Header("Runtime (손패)")]
    public Sprite[] slots = new Sprite[5];  // 현재 뽑힌 앞면들 (최대 5장)
    public int slotIndex = 0;

    [Header("Flip Settings")]
    public float flipDuration = 0.25f;      // 뒤집기 시간(초)

    // CardContainer의 직계 자식 Image들 (Card1~Card5)
    private Image[] childImages;

    // 이번 핸드에서 아직 안 뽑힌 deck 인덱스들(셔플됨)
    private List<int> handPool;

    void Awake()
    {
        AutoBindDirectChildImages();
        if (slots == null || slots.Length < 5) slots = new Sprite[5];

        // 시작 시 화면을 모두 뒷면으로 초기화
        InitBacks();

        if (deck != null && deck.Count > 0)
            BeginHand();
    }

    void Update()
    {
        // R: 한 장 뽑고, 그 슬롯만 뒷면 -> 앞면으로 뒤집기
        if (Input.GetKeyDown(KeyCode.R))
        {
            int justFilled = DrawRandomCardNoDuplicate_FromPool(); // 방금 채워진 슬롯 인덱스
            if (justFilled >= 0 && justFilled < childImages.Length)
            {
                var img = childImages[justFilled];
                var front = slots[justFilled];
                if (img != null && front != null)
                {
                    // 뒷면에서 앞면으로 플립
                    StartCoroutine(FlipToFront(img, front, flipDuration));
                }
            }
        }

        // N: 새 핸드 시작(모두 뒷면으로 되돌리고 풀 셔플)
        if (Input.GetKeyDown(KeyCode.N))
        {
            BeginHand();
            InitBacks();
        }
    }

    /// CardContainer의 "직계 자식" Image만 수집
    void AutoBindDirectChildImages()
    {
        var list = new List<Image>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var img = child.GetComponent<Image>();
            if (img != null) list.Add(img);
        }
        childImages = list.ToArray();
    }

    /// 화면을 전부 뒷면으로 초기화
    void InitBacks()
    {
        if (childImages == null) return;
        foreach (var img in childImages)
        {
            if (img == null) continue;
            img.enabled = true;           // 비활성화로 "사라지는" 문제 방지
            img.sprite = backSprite;      // 기본은 항상 뒷면
            img.rectTransform.localScale = Vector3.one; // 플립 스케일 초기화
        }
    }

    /// 새 핸드 시작: 인덱스 풀 생성/셔플 + 손패/인덱스 초기화
    void BeginHand()
    {
        if (deck == null || deck.Count == 0)
        {
            Debug.LogWarning("Deck이 비었습니다.");
            handPool = null;
            slotIndex = 0;
            System.Array.Clear(slots, 0, slots.Length);
            return;
        }

        // 인덱스 풀 생성
        handPool = new List<int>(deck.Count);
        for (int i = 0; i < deck.Count; i++) handPool.Add(i);

        // Fisher–Yates 셔플
        for (int i = handPool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // [0, i]
            (handPool[i], handPool[j]) = (handPool[j], handPool[i]);
        }

        // 손패 초기화
        slotIndex = 0;
        System.Array.Clear(slots, 0, slots.Length);

        Debug.Log("새 핸드를 시작했습니다. (N 키로 수동 초기화 가능)");
    }

    /// deck에서 한 장을 뽑아 slots에 채움(중복 없음). 성공 시 "방금 채워진 슬롯 인덱스" 반환, 실패 시 -1
    public int DrawRandomCardNoDuplicate_FromPool()
    {
        if (deck == null || deck.Count == 0)
        {
            Debug.LogWarning("Deck이 비었습니다.");
            return -1;
        }
        if (handPool == null || handPool.Count == 0)
        {
            // 풀 없음/소진 → 새 핸드
            BeginHand();
            if (handPool == null || handPool.Count == 0) return -1;
            // 새로 시작했으니 화면도 뒷면으로
            InitBacks();
        }

        int maxHand = Mathf.Min(5, deck.Count);
        if (slotIndex >= maxHand)
        {
            Debug.Log("손패가 가득 찼습니다. (N 키로 새 핸드를 시작하세요)");
            return -1;
        }

        // 셔플된 풀의 마지막을 pop → 랜덤 효과
        int pick = handPool[handPool.Count - 1];
        handPool.RemoveAt(handPool.Count - 1);

        int just = slotIndex;
        slots[slotIndex++] = deck[pick];
        return just;
    }

    /// 특정 Image를 뒷면→앞면으로 뒤집는 간단한 플립 코루틴
    IEnumerator FlipToFront(Image img, Sprite front, float duration)
    {
        if (img == null || front == null) yield break;
        float half = Mathf.Max(0.0001f, duration * 0.5f);
        var rt = img.rectTransform;
        var baseScale = rt.localScale;

        // 1) 가로 스케일을 1 → 0 (뒷면 줄어듦)
        float t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(1f, 0f, t / half);
            rt.localScale = new Vector3(s, baseScale.y, baseScale.z);
            yield return null;
        }

        // 중간 시점에 앞면으로 스왑
        img.sprite = front;

        // 2) 가로 스케일을 0 → 1 (앞면 펼쳐짐)
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(0f, 1f, t / half);
            rt.localScale = new Vector3(s, baseScale.y, baseScale.z);
            yield return null;
        }

        rt.localScale = baseScale;
    }

    /// (선택) 전체 동기화가 필요할 때 사용: 미채워진 칸은 뒷면 유지
    public void ApplySlotsToChildren()
    {
        if (childImages == null || childImages.Length == 0) return;

        int count = Mathf.Min(5, Mathf.Min(slots?.Length ?? 0, childImages.Length));
        for (int i = 0; i < count; i++)
        {
            var img = childImages[i];
            if (img == null) continue;

            img.enabled = true; // 항상 보이게
            img.sprite = (slots[i] != null) ? slots[i] : backSprite;
        }
    }
}