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
        public CardType resourceType;
        public SO.TransformVariable areaGrid;
        public SO.TransformVariable resourceGrid;
        public GameElements.GE_Logic cardDownLogic;
        public override void Execute()
        {
            if (card.value == null)
            {
                return;
            }

            if (card.value.viz.card.cardType == creatureType)
            {
                Debug.Log("Place card down");
                Settings.SetParentForCard(card.value.transform, areaGrid.value.transform);
                card.value.transform.gameObject.SetActive(true);
                card.value.currentLogic = cardDownLogic;
                // Place card down
            }

            else if (card.value.viz.card.cardType == resourceType)
            {
                
                Settings.SetParentForCard(card.value.transform, resourceGrid.value.transform);
                card.value.transform.gameObject.SetActive(true);
                card.value.currentLogic = cardDownLogic;
            }
        }
    }
}