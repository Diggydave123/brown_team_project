using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Cards/Creature")]
    public class CreatureCard : CardType
    {
        public override void OnSetType(CardViz viz)
        {
            viz.statsHolder.SetActive(true);
        }
    }   
}