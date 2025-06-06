using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SA.GameStates;
using UnityEngine.EventSystems;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/SelectCardsToAttack")]
    public class SelectCardsToAttack : Action
    {
        public override void Execute(float d) {
            if(Input.GetMouseButtonDown(0))
            {
                List<RaycastResult> results = Settings.GetUIObjs();

                foreach (RaycastResult r in results)
                {
                    CardInstance inst = r.gameObject.GetComponentInParent<CardInstance>();
                    PlayerHolder p = Settings.gameManager.currentPlayer;

                    if (!p.cardsDown.Contains(inst))
                        return;

                    MultiplayerManager.singleton.PlayerWantsToUseCard(inst.viz.card.instId, p.photonId, MultiplayerManager.CardOperation.setCardForBattle);
                    
                    // if (inst.CanAttack() && !p.attackingCards.Contains(inst))
                    // {
                    //     p.attackingCards.Add(inst);
                    //     // Debug.Log("Card added to attacking cards, number of attacking cards: " +  p.attackingCards.Count.ToString());
                    //     p.currentHolder.SetCardOnBattleLine(inst);
                    // }
                }
                
            }
        }
    }
}