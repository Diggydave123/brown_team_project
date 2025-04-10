using UnityEngine;
using System.Collections;

namespace SA.GameElements
{
    public class CurretnSelected : MonoBehaviour
    {
        public CardVariable currentCard;
        public CardViz cardViz;

        Transform mTransform;

        public void LoadCard()
        {
            if(currentCard.value == null)
                return;

            currentCard.value.gameObject.SetActive(false);
            cardViz.LoadCard(currentCard.value.viz.card);
            cardViz.gameObject.SetActive(true);
        }

        public void CloseCard()
        {
            cardViz.gameObject.SetActive(false);
        }
        private void Start()
        {
            mTransform = this.transform;
            CloseCard();
        }
        void Update()
        {
            mTransform.position = Input.mousePosition;
        }
    }
}