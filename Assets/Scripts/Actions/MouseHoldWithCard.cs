using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using SO;

namespace SA.GameStates
{
    [CreateAssetMenu(menuName = "Actions/MouseHoldWithCard")]
    public class MouseHoldWithCard : Action
    {
        public State playerControlState;
        public SO.GameEvent onPlayerConrtolState;
        public override void Execute(float d)
        {
            bool mouseIsDown = Input.GetMouseButton(0);

            if (!mouseIsDown)
            {
                List<RaycastResult> results = Settings.GetUIObjs();
                
                foreach (RaycastResult r in results)
                {
                    //Check for dropable areas
                }

                Settings.gameManager.SetState(playerControlState);
                onPlayerConrtolState.Raise();
                return;
            }
        }
    }

}