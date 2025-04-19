using System;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Actions/BattlePhaseStartCheck")]
    public class BattlePhaseStartCheck : Condition
    {
        public override bool IsValid()
        {
            GameManager gm = GameManager.singleton;

            if(gm.currentPlayer.cardsDown.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}
