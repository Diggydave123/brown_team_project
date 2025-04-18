using System.Collections;
using System.Collections.Generic;
using SA.GameElements;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Holder/Card Holder")]
    public class CardHolders : ScriptableObject
    {
        public SO.TransformVariable handGrid;
        public SO.TransformVariable resourcesGrid;
        public SO.TransformVariable downGrid;

        public void LoadPlayer(PlayerHolder holder)
        {
            foreach (CardInstance c in holder.cardsDown)
            {
                Settings.SetParentForCard(c.viz.gameObject.transform, downGrid.value.transform);
            }

            foreach (CardInstance c in holder.handCards)
            {
                Settings.SetParentForCard(c.viz.gameObject.transform, handGrid.value.transform);
            }

            foreach (ResourceHolder c in holder.resourcesList)
            {
                Settings.SetParentForCard(c.cardObj.transform, resourcesGrid.value.transform);
            }
        }
    }
}

