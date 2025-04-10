using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace SA.GameStates
{
    [CreateAssetMenu(menuName = "Actions/OnMouseClick")]
    public class OnMouseClick : Action
    {
        public override void Execute(float d)
        {
            if(Input.GetMouseButton(0))
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
                        c.OnClick();
                        break;
                    } 
                }
            }  
        }   
    }
}











/*PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            IClickable c = null;

            foreach (RaycastResult r in results)
            {
                c = r.gameObject.GetComponentInParent<IClickable>();
                //IClickable c = r.gameObject.GetComponentInParent<IClickable>();
                if(c != null)
                {
                    c.OnHighlight();
                    break;
                }
            }

            if(c != null)
            {
                if(Input.GetMouseButton(0))
                {
                    c.OnClick();
                }
            }
            
            } */