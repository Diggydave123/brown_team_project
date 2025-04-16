using System.Collections;
using System.Collections.Generic;
using SA.GameElements;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Areas/MyCardsDownWhenHoldingCard")]
    public class MyCardsDownAreaLogic : AreaLogic
    {
        public CardVariable card;
        public CardType creatureType;
        public SO.TransformVariable areaGrid;
        public GameElements.GE_Logic cardDownLogic;
        public override void Execute()
        {
            if (card.value == null)
            {
                return;
            }

            if(card.value.viz.card.cardType == creatureType)
            {
                Debug.Log("Place card down");
                // Place card down
                card.value.transform.SetParent(areaGrid.value.transform);
                card.value.transform.localPosition = Vector3.zero;
                card.value.transform.localEulerAngles = Vector3.zero;
                card.value.transform.localScale = Vector3.one;
                card.value.transform.gameObject.SetActive(true);
                card.value.currentLogic = cardDownLogic;
            }
        }
    }
}