using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandDrawer_AlphaCrossfadeFlip : MonoBehaviour
{
    [Header("Deck / Sprites")]
    public List<CardData> deck;
    public Sprite backSprite;

    [Header("Layout Settings")]
    public Vector2 cardSize = new Vector2(200, 280);
    public int handSize = 5;

    [Header("Flip Animation")]
    public float flipDuration = 0.25f;   // 뒤집기 총 시간(초)

    [Header("Runtime")]
    public Sprite[] slots = new Sprite[5];
    public int slotIndex = 0;

    private RectTransform container;

    // 시각 레이어
    private Image[] backImages;
    private Image[] frontImages;
    private CanvasGroup[] backGroups;
    private CanvasGroup[] frontGroups;

    // 3D 회전용 Pivot (슬롯 내부에 생성)
    private RectTransform[] flipPivots;

    // 뽑기 풀/상태
    private List<int> handPool;
    private bool[] isFlipping;

    void Awake()
    {
        container = (RectTransform)transform;

        int slotCount = Mathf.Min(handSize, transform.childCount);
        if (slotCount < handSize)
            Debug.LogWarning($"슬롯 {handSize}개 필요하지만 {slotCount}개만 존재합니다.");

        backImages  = new Image[slotCount];
        frontImages = new Image[slotCount];
        backGroups  = new CanvasGroup[slotCount];
        frontGroups = new CanvasGroup[slotCount];
        flipPivots  = new RectTransform[slotCount];
        isFlipping  = new bool[slotCount];

        EnsureFixedLayoutForChildren(slotCount);
        BuildOrBindVisualLayers(slotCount);
        InitBacks(slotCount);

        if (deck != null && deck.Count > 0)
            BeginHand();
    }

    void Update()
    {
        if (GameManager.instance.isLive && Input.GetKeyDown(KeyCode.Space))
        {
            int idx = DrawRandomCardNoDuplicate_FromPool();
            if (idx >= 0 && idx < frontImages.Length)
            {
                frontImages[idx].sprite = slots[idx];               // 앞면 설정
                StartCoroutine(FlipToFront3D(idx, flipDuration));   // ★ 3D 뒤집기
            }
        }

        if (GameManager.instance.isLive && Input.GetKeyDown(KeyCode.R))
        {
            BeginHand();
            InitBacks(frontImages.Length);
            WeaponSelectUIManager.instance.unsetAllCards();
        }
    }

    // ── 레이아웃 고정: 슬롯 크기를 LayoutElement로 못 박기 ───────────────
    void EnsureFixedLayoutForChildren(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var slot = (RectTransform)transform.GetChild(i);
            var le = slot.GetComponent<LayoutElement>();
            if (!le) le = slot.gameObject.AddComponent<LayoutElement>();
            le.minWidth = le.preferredWidth = cardSize.x;
            le.minHeight = le.preferredHeight = cardSize.y;
            le.flexibleWidth = le.flexibleHeight = 0f;

            var rootImg = slot.GetComponent<Image>();
            if (rootImg) rootImg.enabled = false; // 시각은 하위 레이어가 담당
        }
    }

    // ── FlipPivot + Back/Front 레이어 구축 ──────────────────────────────
    void BuildOrBindVisualLayers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var slot = (RectTransform)transform.GetChild(i);

            // FlipPivot (슬롯 안에서만 회전/애니메이션)
            Transform pivotT = slot.Find("FlipPivot");
            if (!pivotT)
            {
                var go = new GameObject("FlipPivot", typeof(RectTransform));
                pivotT = go.transform;
                SetupFullStretch((RectTransform)pivotT);
                pivotT.SetParent(slot, false);
            }
            var pivot = (RectTransform)pivotT;
            pivot.localRotation = Quaternion.identity;
            pivot.localScale = Vector3.one;
            flipPivots[i] = pivot;

            // Back
            Transform backT = pivot.Find("Back");
            if (!backT)
            {
                var go = new GameObject("Back", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
                backT = go.transform;
                SetupFullStretch((RectTransform)backT);
                backT.SetParent(pivot, false);
            }
            var backImg = backT.GetComponent<Image>();
            var backCg  = backT.GetComponent<CanvasGroup>();
            backImg.preserveAspect = true;
            backImg.sprite = backSprite;
            backCg.alpha = 1f;
            backCg.interactable = backCg.blocksRaycasts = false;

            // Front
            Transform frontT = pivot.Find("Front");
            if (!frontT)
            {
                var go = new GameObject("Front", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
                frontT = go.transform;
                SetupFullStretch((RectTransform)frontT);
                frontT.SetParent(pivot, false);
            }
            var frontImg = frontT.GetComponent<Image>();
            var frontCg  = frontT.GetComponent<CanvasGroup>();
            frontImg.preserveAspect = true;
            frontImg.sprite = null;
            frontCg.alpha = 0f;
            frontCg.interactable = frontCg.blocksRaycasts = false;

            backImages[i]  = backImg;
            frontImages[i] = frontImg;
            backGroups[i]  = backCg;
            frontGroups[i] = frontCg;
        }
    }

    void SetupFullStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void InitBacks(int count)
    {
        slotIndex = 0;
        if (slots == null || slots.Length < handSize) slots = new Sprite[handSize];
        System.Array.Clear(slots, 0, slots.Length);

        for (int i = 0; i < count; i++)
        {
            if (backImages[i])  backImages[i].sprite  = backSprite;
            if (frontImages[i]) frontImages[i].sprite = null;
            if (backGroups[i])  backGroups[i].alpha  = 1f;
            if (frontGroups[i]) frontGroups[i].alpha = 0f;
            if (flipPivots[i])  flipPivots[i].localRotation = Quaternion.identity;
            isFlipping[i] = false;
        }
    }

    // ── 뽑기 로직(중복X) ────────────────────────────────────────────────
    void BeginHand()
    {
        if (deck == null || deck.Count == 0)
        {
            Debug.LogWarning("Deck이 비었습니다.");
            handPool = null;
            return;
        }

        handPool = new List<int>(deck.Count);
        for (int i = 0; i < deck.Count; i++) handPool.Add(i);

        for (int i = handPool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (handPool[i], handPool[j]) = (handPool[j], handPool[i]);
        }
    }

    int DrawRandomCardNoDuplicate_FromPool()
    {
        if (deck == null || deck.Count == 0) { Debug.LogWarning("Deck이 비었습니다."); return -1; }

        if (handPool == null || handPool.Count == 0)
        {
            BeginHand();
            InitBacks(frontImages.Length);
            if (handPool == null || handPool.Count == 0) return -1;
        }

        int maxHand = Mathf.Min(handSize, deck.Count);
        if (slotIndex >= maxHand)
        {
            Debug.Log("손패가 가득 찼습니다. (R로 새 핸드를 시작하세요)");
            return -1;
        }

        int pick = handPool[handPool.Count - 1];
        handPool.RemoveAt(handPool.Count - 1);

        int idx = slotIndex;
        slots[slotIndex++] = deck[pick].cardImage;
        WeaponSelectUIManager.instance.setCard(idx, deck[pick]);
        return idx;
    }

    // ── 뒤집기 애니메이션: Pivot을 Y축으로 0→90°→0 회전 + 알파 교차 ─────────
    IEnumerator FlipToFront3D(int slotIdx, float duration)
    {
        if (slotIdx < 0 || slotIdx >= isFlipping.Length) yield break;
        if (isFlipping[slotIdx]) yield break;

        var pivot = flipPivots[slotIdx];
        var front = frontGroups[slotIdx];
        var back  = backGroups[slotIdx];
        if (!pivot || !front || !back) yield break;

        isFlipping[slotIdx] = true;

        float half = Mathf.Max(0.0001f, duration * 0.5f);
        float t = 0f;

        // 1) 0° → 90° : 뒷면 보이던 면이 얇아지며 사라짐
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / half);
            pivot.localRotation = Quaternion.Euler(0f, Mathf.Lerp(0f, 90f, a), 0f);
            back.alpha  = 1f - a;   // 뒷면 서서히 사라짐
            yield return null;
        }
        pivot.localRotation = Quaternion.Euler(0f, 90f, 0f);
        back.alpha = 0f;

        // 2) 90° → 0° : 앞면이 나타남
        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / half);
            pivot.localRotation = Quaternion.Euler(0f, Mathf.Lerp(90f, 0f, a), 0f);
            front.alpha = a;        // 앞면 서서히 나타남
            yield return null;
        }
        pivot.localRotation = Quaternion.identity;
        front.alpha = 1f;

        isFlipping[slotIdx] = false;
    }
}