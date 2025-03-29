using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // ADD THIS

public class ThisCard : MonoBehaviour {

    public List<Card> thisCard = new List<Card>();
    public int thisId;

    public int id;
    public string cardName;
    public int cost;
    public int power;
    public string cardDescription;

    public TMP_Text nameText;       // Use TMP_Text instead of Text
    public TMP_Text costText;
    public TMP_Text powerText;
    public TMP_Text descriptionText;

    public Sprite thisSprite; 
    public Image thatImage;
    public Image frame; // This part of the code is to personalize the cards

    public bool cardBack;
    public static bool staticCardBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisCard.Add(null); // Ensure list has a slot
        thisCard [0] = CardDataBase.cardList[thisId];
    }

    // Update is called once per frame
    void Update()
    {
        id = thisCard[0].id;
        cardName = thisCard[0].cardName;
        cost = thisCard[0].cost;
        power = thisCard[0].power;
        cardDescription = thisCard[0].cardDescription;

        thisSprite = thisCard[0].thisImage;

        nameText.text = "" + cardName;
        costText.text = "" + cost;
        powerText.text = "" + power;
        descriptionText.text = " " + cardDescription;

        thatImage.sprite = thisSprite;

        // This part of the code is to personalize the cards
        if(thisCard[0].color == "Red"){
            frame.GetComponent<Image>().color = new Color32(255,0,0,225);
        }
        if(thisCard[0].color == "Blue"){
            frame.GetComponent<Image>().color = new Color32(0,0,225,225);
        }
        if(thisCard[0].color == "Yellow"){
            frame.GetComponent<Image>().color = new Color32(255,225,0,225);
        }
        if(thisCard[0].color == "Green"){
            frame.GetComponent<Image>().color = new Color32(255,0,225,225);
        }

        staticCardBack = cardBack;
    }
}
