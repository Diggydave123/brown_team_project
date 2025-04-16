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
        public CardVariable currentCard;
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
                    GameElements.Area a = r.gameObject.GetComponentInParent<GameElements.Area>();
                    if (a != null)
                    {
                        a.OnDrop();
                        break;
                    }
                }

                currentCard.value.gameObject.SetActive(true);
                currentCard.value = null;
                
                Settings.gameManager.SetState(playerControlState);
                onPlayerConrtolState.Raise();
                return;
            }
        }
    }
}