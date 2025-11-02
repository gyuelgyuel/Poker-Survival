using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObject/CardData")]
public class CardData : ScriptableObject
{
    public enum CardType { Spade, Heart, Diamond, Club }

    [Header("# Main Info")]
    public CardType cardType;
    public int cardNumber;
    public Sprite cardImage;
    public Sprite cardBackImage;
}
