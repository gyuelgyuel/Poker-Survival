using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData data;
    public Image image;
    public Sprite backImage;
    public int index;
    public bool isClicked = false;
    private bool isFliped = false;
    

    Image icon;

    void Awake()
    {

    }

    void OnEnable()
    {

    }

    private void LateUpdate()
    {
        
    }

    public void setCardData(CardData cardData)
    {
        image.sprite = cardData.cardImage;
        backImage = cardData.cardBackImage;
        data = cardData;
        isFliped = true;
    }

    public void unsetCardData()
    {
        image.sprite = backImage;
        data = null;
        isFliped = false;
    }

    public void OnClickItem()
    {
        if (!isFliped)
            return;

        isClicked = !isClicked;
        Color color = isClicked ? new Color(1f, 1f, 1f, 1f) : new Color(0, 0, 0, 1f);
        Color imageColor = isClicked ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 1f, 1f, 1f);
        GetComponentsInChildren<Outline>()[0].effectColor = color;
        GetComponent<Image>().color = imageColor;


    }
}
