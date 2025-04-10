using UnityEngine;
using System.Collections;

namespace SA.GameElements
{
    [CreateAssetMenu(menuName = "Game Elements/My Hand Card")]
    public class HandCard : GE_Logic
    {
        public CardVariable currentCard;
        public override void OnClick(CardInstance inst)
        {
            Debug.Log("this card is on my hand");
            currentCard.Set(inst);
        }

        public override void OnHighlight(CardInstance inst)
        {

        }
    }
}

