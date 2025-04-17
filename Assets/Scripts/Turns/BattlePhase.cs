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
            if(isInit)
            {
                Settings.gameManager.SetState(null);
                isInit = false;
            }
        }

        public override void OnStartPhase()
        {
            if(!isInit)
            {
                Settings.gameManager.SetState(null);
                Settings.gameManager.onPhaseChanged.Raise();
                isInit = true;
            }
        }
    }
}