using UnityEngine;
using System.Collections;

namespace SA
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public CardViz viz;
        public SA.GameElements.GE_Logic currentLogic;

        void Start()
        {
            viz = GetComponent<CardViz>();
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
