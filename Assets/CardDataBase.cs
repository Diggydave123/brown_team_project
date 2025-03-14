using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    void Awake() {
        cardList.Add(new Card (0, "None", 0, 0, "None"));
        cardList.Add(new Card (1, "A", 2, 1000, "It's A"));    
        cardList.Add(new Card (2, "B", 3, 3000, "It's B"));
        cardList.Add(new Card (3, "C", 5, 6000, "It's C"));
        cardList.Add(new Card (4, "D", 1, 1000, "It's D"));
    }
}
