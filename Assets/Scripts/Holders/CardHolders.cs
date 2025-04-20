using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
        public SO.TransformVariable battleLine;

        public void SetCardOnBattleLine(CardInstance card)
        {
            UnityEngine.Vector3 position = card.viz.gameObject.transform.position;
            Settings.SetParentForCard(card.viz.gameObject.transform, battleLine.value.transform);
            position.z = card.viz.gameObject.transform.position.z;
            position.y = card.viz.gameObject.transform.position.y;
            card.viz.gameObject.transform.position = position;

        }

        public void LoadPlayer(PlayerHolder holder, PlayerStatsUI statsUI)
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

            holder.statsUI = statsUI;
            holder.LoadPlayerOnStatsUI();
        }
    }
}

