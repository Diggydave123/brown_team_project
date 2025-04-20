using UnityEngine;
using System.Collections;

namespace SA
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public CardViz viz;
        public SA.GameElements.GE_Logic currentLogic;
        public bool isFlatfooted;

        void Start()
        {
            viz = GetComponent<CardViz>();
        }

        public bool CanAttack() {
            bool result = true;

            if (isFlatfooted) {
                result = false;
            }

            if (viz.card.cardType.TypeAllowsForAttack(this))
            {  
                result = true;
            }
            
            return result;
        }

        public void OnClick()
        {
            if (currentLogic == null)
            {
                return;
            }
            currentLogic.OnClick(this);
        }

        public void OnHighlight()
        {
            if (currentLogic == null)
            {
                return;
            }
            
            currentLogic.OnHighlight(this);
        }
    }

}
