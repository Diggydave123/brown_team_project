using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace SA.GameStates 
{
    [CreateAssetMenu(menuName = "Actions/MouseOverDetection")]
    public class MouseOverDetection : Action
    {
        public override void Execute(float d)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult r in results)
            {
                IClickable c = r.gameObject.GetComponentInParent<IClickable>();
                if(c != null)
                {
                    c.OnHighlight();
                    break;
                }
            }
            
        }
    }
}