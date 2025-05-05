using SA;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Turns/ResetCurrentPlayerCardResources")]


    public class ResetCurrentPlayerCardResources : Phase
    {
        public override bool IsComplete()
        {
            // MultiplayerManager.singleton.PlayerWantsToResetResourcesCards();
            return true;
        }

        public override void OnEndPhase()
        {
            
        }

        public override void OnStartPhase()
        {
            
        }
    }
}
