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
        
        public override void Execute()
        {
            Debug.Log("My Card Down Area Logic Execute");
            if (card.value == null)
            {
                return;
            }

            Card c = card.value.viz.card;

            if (c.cardType == creatureType)
            {
                MultiplayerManager.singleton.PlayerWantsToUseCard(c.instId, GameManager.singleton.localPlayer.photonId, MultiplayerManager.CardOperation.dropCreatureCard);
            }

            else if (c.cardType == resourceType)
            {
                MultiplayerManager.singleton.PlayerWantsToUseCard(c.instId, GameManager.singleton.localPlayer.photonId, MultiplayerManager.CardOperation.dropResourcesCard);
            }
        }
    }
}