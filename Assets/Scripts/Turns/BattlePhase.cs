using SA;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Turns/Battle Phase Player")]
    public class BattlePhase : Phase
    {
        public override bool IsComplete()
        {
            if (forceExit)
            {
                forceExit = false;
                return true;
            }
            return false;
        }

        public override void OnEndPhase()
        {
            
        }

        public override void OnStartPhase()
        {
           
        }
    }
}