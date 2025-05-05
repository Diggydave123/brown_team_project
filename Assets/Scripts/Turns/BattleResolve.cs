using System.Collections.Generic;
using UnityEngine;

namespace SA {
    [CreateAssetMenu(menuName = "Turns/BattleResolve")]
    public class BattleResolve : Phase
    {
        // public Element attackElement;
        // public Element defenseElement;

        public override bool IsComplete()
        {

            /* PlayerHolder p = Settings.gameManager.currentPlayer;
            // PlayerHolder e = Settings.gameManager.GetEnemyOf(p);

            // Debug.Log(p == e);

            // if(p.attackingCards.Count == 0) {
            //     return true;
            // }

            // Dictionary<CardInstance, BlockInstance> blockDict = Settings.gameManager.GetBlockInstances();

            // for(int i = 0; i < p.attackingCards.Count; i++) 
            // {
            //     CardInstance inst = p.attackingCards[i];
            //     Card c = inst.viz.card;
            //     CardProperties attack = c.GetProperty(attackElement);
            //     if (attack == null)
            //     {
            //         Debug.LogError("You are attacking with a card that can't attack");
            //         continue;
            //     }

            //     int attackValue = attack.intValue;

            //     BlockInstance bi = GetBlockInstanceOfAttacker(inst, blockDict);
            //     if (bi != null)
            //     {
            //         for (int b = 0; b < bi.blocker.Count; b++) {
            //             CardInstance blockInst = bi.blocker[b];
            //             Card blockingCard = blockInst.viz.card;
            //             CardProperties def = blockingCard.GetProperty(defenseElement);
            //             if (def == null)
            //             {
            //                 Debug.LogWarning("You are trying to block with a card with no defense element!");
            //                 continue;
            //             }

            //             attackValue -= def.intValue;

            //             if(def.intValue <= attack.intValue)
            //             {
            //                 // Card dies
            //                 blockInst.CardInstanceToGraveyard();
            //             }
            //         }
            //     }
            //     Debug.Log("Attack Value:" + attackValue);
            //     if (attackValue <= 0) {
            //         Debug.Log("Attacker dies");
            //         attackValue = 0;
            //         inst.CardInstanceToGraveyard();
            //     }


            //     p.DropCard(inst, false);
            //     p.currentHolder.SetCardDown(inst);
            //     inst.SetFlatfooted(true);
                
            //     e.DoDamage(attackValue);
            //     Debug.Log("Damage done: " + attack.intValue.ToString());
            // }


            // Settings.gameManager.ClearBlockInstances();
            // p.attackingCards.Clear();
            */
            
            if (forceExit)
            {
                forceExit = false;
                return true;
            }
            return false;
        }

        BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker, Dictionary<CardInstance, BlockInstance> blockInstances)
        {
            BlockInstance r = null;
            blockInstances.TryGetValue(attacker, out r);
            return r;
        }

        public override void OnEndPhase()
        {
            isInit = false;
            
        }

        public override void OnStartPhase()
        {
            if (!isInit)
            {
                isInit = true;
                MultiplayerManager.singleton.SetBattleResolvePhase();

            }
        }
    }
}
