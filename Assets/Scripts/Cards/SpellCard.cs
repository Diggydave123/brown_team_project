using UnityEngine;
using System.Collections;

namespace SA 
{
    [CreateAssetMenu(menuName = "Cards/Spell")]
    public class SpellCard : CardType 
    {
        public override void OnSetType(CardViz viz)
        {
            base.OnSetType(viz);
            viz.statsHolder.SetActive(false);
        }
    }
}