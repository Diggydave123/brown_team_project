using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace SA{
    public class CardViz : MonoBehaviour {
        public TextMeshProUGUI title;
        public TextMeshProUGUI detail;
        public TextMeshProUGUI flavor;
        public Image art;

        public Card card;

        private void Start(){
            LoadCard(card);
        }

        public void LoadCard(Card c){

            if (c == null)
                return;

            card = c;
            title.text = c.cardName;
            detail.text = c.cardDetail;

            if(string.IsNullOrEmpty(c.cardFlavor)){
                flavor.gameObject.SetActive(false);
            } else {
                flavor.gameObject.SetActive(true);
                flavor.text = c.cardFlavor; 
            }
            art.sprite = c.art;
        }
    }
}

