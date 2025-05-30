using System.Collections;
using System.Collections.Generic;
using SA.GameElements;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Holder/Player Holder")]
    public class PlayerHolder : ScriptableObject
    {
        public string username;
        public Sprite portrait;
        public Color playerColor;

        public int photonId = -1;

        [System.NonSerialized] public int health;
        public PlayerStatsUI statsUI;

        public List<string> startingDeck = new List<string>();

        [System.NonSerialized] public List<string> all_cards = new List<string>();
        public int resourcesPerTurn = 1;
        [System.NonSerialized] public int resourcesDroppedThisTurn;

        public bool isHumanPlayer;

        public GE_Logic handLogic;
        public GE_Logic downLogic;

        [System.NonSerialized] public CardHolders currentHolder;
        [System.NonSerialized] public List<CardInstance> handCards = new List<CardInstance>();
        [System.NonSerialized] public List<CardInstance> cardsDown = new List<CardInstance>();
        [System.NonSerialized] public List<CardInstance> attackingCards = new List<CardInstance>();
        [System.NonSerialized] public List<ResourceHolder> resourcesList = new List<ResourceHolder>();
        [System.NonSerialized] public List<int> cardInstIds = new List<int>();
        public List<Card> allCardInstances = new List<Card>();

        public void Init()
        {
            health = 20;
            all_cards.AddRange(startingDeck);
        }

        public int resourcesCount
        {
            get
            {
                return currentHolder.resourcesGrid.value.GetComponentsInChildren<CardViz>().Length;
            }
        }

        public void CardToGraveyard(CardInstance c)
        {
            Debug.Log(c + " Card to grave");
            if (attackingCards.Contains(c)) attackingCards.Remove(c);
            if (handCards.Contains(c)) handCards.Remove(c);
            if (cardsDown.Contains(c)) cardsDown.Remove(c);
        }

        public void AddResourceCard(GameObject cardObj)
        {
            if (cardObj == null) return;

            ResourceHolder resourceHolder = new ResourceHolder
            {
                cardObj = cardObj
            };

            resourcesList.Add(resourceHolder);
            resourcesDroppedThisTurn++;

            Settings.RegisterEvent(username + " drops resources card", Color.white);
        }

        public int NonUsedCards()
        {
            int result = 0;

            for (int i = 0; i < resourcesList.Count; i++)
            {
                if (resourcesList[i] != null && !resourcesList[i].isUsed)
                {
                    result++;
                }
            }
            return result;
        }

        public void DropCard(CardInstance inst, bool registerEvent = true)
        {
            if (handCards.Contains(inst)) handCards.Remove(inst);
            if (!cardsDown.Contains(inst)) cardsDown.Add(inst);

            if (registerEvent)
            {
                Settings.RegisterEvent(username + " used " + inst.viz.card.name + " for " + inst.viz.card.cost + " resources", Color.white);
            }
        }

        public bool CanUseCard(Card c)
        {
            if (c.cardType is CreatureCard || c.cardType is SpellCard)
            {
                return c.cost <= NonUsedCards();
            }
            else if (c.cardType is ResourcesCard)
            {
                return (resourcesPerTurn - resourcesDroppedThisTurn) > 0;
            }

            return false;
        }

        public List<ResourceHolder> GetUnusedResources()
        {
            List<ResourceHolder> result = new List<ResourceHolder>();

            for (int i = 0; i < resourcesList.Count; i++)
            {
                if (resourcesList[i] != null && !resourcesList[i].isUsed)
                {
                    result.Add(resourcesList[i]);
                }
            }

            return result;
        }

        public void MakeAllResourceCardsUsable()
        {
            // Clean up destroyed objects
            resourcesList.RemoveAll(r => r == null || r.cardObj == null);

            foreach (var r in resourcesList)
            {
                r.isUsed = false;

                if (r.cardObj != null && r.cardObj.transform != null)
                {
                    r.cardObj.transform.localEulerAngles = Vector3.zero;
                }
            }

            resourcesDroppedThisTurn = 0;
        }

        public void UseResourceCards(int amount)
        {
            Vector3 euler = new Vector3(0, 0, 90);
            List<ResourceHolder> l = GetUnusedResources();

            for (int i = 0; i < amount && i < l.Count; i++)
            {
                if (l[i].cardObj != null && l[i].cardObj.transform != null)
                {
                    l[i].isUsed = true;
                    l[i].cardObj.transform.localEulerAngles = euler;
                }
            }
        }

        public void LoadPlayerOnStatsUI()
        {
            if (statsUI != null)
            {
                statsUI.player = this;
                statsUI.UpdateAll();
            }
        }

        public bool DoDamage(int value)
        {
            health -= value;

            if (statsUI != null)
                statsUI.UpdateHealth();

            if (health <= 0)
            {
                health = 0;
                Debug.Log("Game over for " + username);
                return true;
            }

            return false;
        }

        public void ClearPlayerState()
        {
            handCards.Clear();
            cardsDown.Clear();
            attackingCards.Clear();
            cardInstIds.Clear();

            // Remove null or destroyed objects
            resourcesList.RemoveAll(r => r == null || r.cardObj == null);

            // Destroy any lingering resource objects
            foreach (var r in resourcesList)
            {
                if (r.cardObj != null)
                {
                    Object.Destroy(r.cardObj);
                }
            }

            resourcesList.Clear();
            resourcesDroppedThisTurn = 0;
        }
    }
}
