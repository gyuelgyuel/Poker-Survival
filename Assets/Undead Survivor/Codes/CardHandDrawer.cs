using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class CardHandDrawer : MonoBehaviour
{
    [Header("Deck Sprites")]
    [Tooltip("인스펙터에 카드 54장을 넣거나 비워두면 Resources/Cards/에서 자동 로드")]
    public List<Sprite> deck = new List<Sprite>();

    [Header("UI Targets (5)")]
    public Image[] slots;

    void Awake()
    {
    }
}
