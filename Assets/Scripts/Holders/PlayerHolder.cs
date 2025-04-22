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

        [System.NonSerialized]
        public int health = 20;
        public PlayerStatsUI statsUI;

        //public string[] startingCards;
        public List<string> startingDeck = new List<string>();

        [System.NonSerialized]
        public List<string> all_cards = new List<string>();

        public int resourcesPerTurn = 1;
        [System.NonSerialized]
        public int resourcesDroppedThisTurn;

        public bool isHumanPlayer;

        public GE_Logic handLogic;
        public GE_Logic downLogic;

        [System.NonSerialized]
        public CardHolders currentHolder;

        [System.NonSerialized]
        public List<CardInstance> handCards = new List<CardInstance>();
        [System.NonSerialized]
        public List<CardInstance> cardsDown = new List<CardInstance>();
        [System.NonSerialized]
        public List<CardInstance> attackingCards = new List<CardInstance>();
        [System.NonSerialized]
        public List<ResourceHolder> resourcesList = new List<ResourceHolder>();

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

        public void CardToGraveyard (CardInstance c) 
        {
            if(attackingCards.Contains(c))
            {
                attackingCards.Remove(c);
            }
            if(handCards.Contains(c))
            {
                handCards.Remove(c);
            }
            if(cardsDown.Contains(c)) {
                cardsDown.Remove(c);
            }
        }

        public void AddResourceCard(GameObject cardObj)
        {
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
                if(!resourcesList[i].isUsed)
                {
                    result++;
                }
            }
            return result;
        }

        public void DropCard(CardInstance inst, bool registerEvent = true)
        {
            if(handCards.Contains(inst))
            {
                handCards.Remove(inst);
            }
            if (!cardsDown.Contains(inst))
            {
                cardsDown.Add(inst);
            }

            if(registerEvent) {
                Settings.RegisterEvent(username + " used " + inst.viz.card.name + " for " + inst.viz.card.cost + " resources", Color.white);
            }
        }

        public bool CanUseCard(Card c)
        {
            bool result = false;
            if(c.cardType is CreatureCard || c.cardType is SpellCard)
            {
                int currentResources = NonUsedCards();

                if (c.cost <= currentResources)
                    result = true;

                return result;
            }
            else
            {
                if(c.cardType is ResourcesCard)
                {
                    if (resourcesPerTurn - resourcesDroppedThisTurn > 0)
                    {
                        result = true;
                    }
                }
            }
            
            return result; 
        }

        public List<ResourceHolder> GetUnusedResources()
        {
            List<ResourceHolder> result = new List<ResourceHolder>();

            for (int i = 0; i < resourcesList.Count; i ++)
            {
                if (!resourcesList[i].isUsed)
                {
                    result.Add(resourcesList[i]);
                }
            }

            return result;
        }

        public void MakeAllResourceCardsUsable()
        {
            for (int i = 0; i < resourcesList.Count; i ++)
            {
                resourcesList[i].isUsed = false;
                resourcesList[i].cardObj.transform.localEulerAngles = Vector3.zero;
            }

            resourcesDroppedThisTurn = 0;
        }

        public void UseResourceCards(int amount)
        {
            Vector3 euler = new Vector3(0, 0, 90);

            List<ResourceHolder> l = GetUnusedResources();

            for (int i = 0; i < amount; i++)
            {
                l[i].isUsed = true;
                l[i].cardObj.transform.localEulerAngles = euler;
            }
        }

        public void LoadPlayerOnStatsUI()
        {
            if(statsUI != null)
            {
                statsUI.player = this;
                statsUI.UpdateAll();
            }
        }
        public void DoDamage(int value)
        {
            health -= value;
            if (statsUI != null)
                statsUI.UpdateHealth();
        }

    }
}