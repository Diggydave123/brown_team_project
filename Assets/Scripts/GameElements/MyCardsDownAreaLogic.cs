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

            Card c = card.value.viz.card;

            if (c.cardType == creatureType)
            {
                bool canUse = Settings.gameManager.currentPlayer.CanUseCard(c);

                if(canUse)
                {
                    Debug.Log("Place card down");
                    Settings.DropCreatureCard(card.value.transform, areaGrid.value.transform, card.value);
                    card.value.currentLogic = cardDownLogic;
                }
                else
                {
                    Settings.RegisterEvent("Not enough resources to use card", Color.red);
                }

                card.value.gameObject.SetActive(true);
                // Place card down
            }

            else if (c.cardType == resourceType)
            {
                bool canUse = Settings.gameManager.currentPlayer.CanUseCard(c);

                if(canUse)
                {
                    Settings.SetParentForCard(card.value.transform, resourceGrid.value.transform);
                    card.value.currentLogic = cardDownLogic;
                    Settings.gameManager.currentPlayer.AddResourceCard(card.value.gameObject);
                }
                else
                {
                    Settings.RegisterEvent("Cant drop more than one resource card per turn", Color.red);
                }
                
                card.value.transform.gameObject.SetActive(true);
            }
        }
    }
}