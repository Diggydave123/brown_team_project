using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Cards/Resource")]
    public class ResourcesCard : CardType
    {
        public override void OnSetType(CardViz viz)
        {
            base.OnSetType(viz);
            viz.statsHolder.SetActive(false);
            viz.resourceHolder.SetActive(false);
        }
    }   
}