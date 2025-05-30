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

        [System.NonSerialized]
        public PlayerHolder playerHolder;

        public void SetCardOnBattleLine(CardInstance card)
        {
            UnityEngine.Vector3 position = card.viz.gameObject.transform.position;
            Settings.SetParentForCard(card.viz.gameObject.transform, battleLine.value.transform);
            position.z = card.viz.gameObject.transform.position.z;
            position.y = card.viz.gameObject.transform.position.y;
            card.viz.gameObject.transform.position = position;

        }

        public void SetCardDown(CardInstance card)
        {
            Settings.SetParentForCard(card.viz.gameObject.transform, downGrid.value.transform);

        }

        public void LoadPlayer(PlayerHolder holder, PlayerStatsUI statsUI)
        {

            if (holder == null)
            {
                return;
            }
            playerHolder = holder;
            holder.currentHolder = this;

            foreach (CardInstance c in holder.cardsDown)
            {
                if (c != null && c.viz != null && c.viz.gameObject != null)
                {
                    Debug.Log(c + " in cardDown");
                    if (!holder.attackingCards.Contains(c))
                    {
                        Settings.SetParentForCard(c.viz.gameObject.transform, downGrid.value.transform);
                    }
                }
            }

            foreach (CardInstance c in holder.handCards)
            {
                if (c != null && c.viz != null && c.viz.gameObject != null)
                {
                    Settings.SetParentForCard(c.viz.gameObject.transform, handGrid.value.transform);
                }
            }

            foreach (ResourceHolder c in holder.resourcesList)
            {
                if (c != null && c.cardObj != null && c.cardObj.transform != null)
                {
                    Settings.SetParentForCard(c.cardObj.transform, resourcesGrid.value.transform);
                }
            }

            foreach (CardInstance c in holder.attackingCards)
            {
                if (c != null && c.viz != null && c.viz.gameObject != null)
                {
                    SetCardOnBattleLine(c);
                }
            }


            // foreach (CardInstance c in holder.cardsDown)
            // {
            //     Debug.Log(c + " in cardDown");
            //     if (!holder.attackingCards.Contains(c))
            //     {
            //         Settings.SetParentForCard(c.viz.gameObject.transform, downGrid.value.transform);
            //     }
            // }

            // foreach (CardInstance c in holder.handCards)
            // {
            //     if (c.viz != null)
            //     {
            //         Settings.SetParentForCard(c.viz.gameObject.transform, handGrid.value.transform);
            //     }
            // }

            // foreach (ResourceHolder c in holder.resourcesList)
            // {
            //     Settings.SetParentForCard(c.cardObj.transform, resourcesGrid.value.transform);
            // }

            // foreach (CardInstance c in holder.attackingCards)
            // {
            //     SetCardOnBattleLine(c);
            // }

            holder.statsUI = statsUI;
            holder.LoadPlayerOnStatsUI();
        }
        public void ClearAllCards()
        {
            ClearGrid(handGrid.value.transform);
            ClearGrid(resourcesGrid.value.transform);
            ClearGrid(downGrid.value.transform);
            ClearGrid(battleLine.value.transform);
        }

        private void ClearGrid(Transform grid)
        {
            for (int i = grid.childCount - 1; i >= 0; i--)
            {
                Destroy(grid.GetChild(i).gameObject);
            }
        }


    }
}

