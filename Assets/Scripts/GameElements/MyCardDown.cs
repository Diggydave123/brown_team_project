using UnityEngine;
using System.Collections;

namespace SA.GameElements
{
    [CreateAssetMenu(menuName = "Game Elements/My Card Down")]
    public class MyCardDown : GE_Logic
    {
        public override void OnClick(CardInstance inst)
        {
            Debug.Log("this card is on mine but its on the table");
        }

        public override void OnHighlight(CardInstance inst)
        {

        }
    }
}