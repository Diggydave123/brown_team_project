using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA
{
    public class NetworkManager : MonoBehaviour
    {
        public bool isMaster;
        public static NetworkManager singleton;

        List<MultiplayerHolder> multiplayerHolders= new List<MultiplayerHolder>();
        public MultiplayerHolder GetHolder(int ownerId)
        {
            for(int i = 0; i < multiplayerHolders.Count; i++) {
                if(multiplayerHolders[i].ownerId == ownerId)
                {
                    return multiplayerHolders[i];
                
                }
            }
            return null;
        }

        public Card GetCard(int instId, int ownerId)
        {
            MultiplayerHolder h = GetHolder(ownerId);
            return h.GetCard(instId);
        }

        ResourcesManager rm;
        int cardInstIds;

        private void Awake()
        {
            if (singleton == null)
            {
                rm = Resources.Load("ResourcesManager") as ResourcesManager;
                singleton = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        #region My Calls

        //Master only
        public void PlayerJoined(int ownerId, string[] cards)
        {
            MultiplayerHolder m = new MultiplayerHolder();
            m.ownerId = ownerId;

            for(int i = 0; i < cards.Length-1; i++) {
                Card c = CreateCardMaster(cards[i]);
                if (c == null) {
                    continue;
                }
                m.RegisterCard(c);

                //RPC


            }
        }

        Card CreateCardMaster(string cardId)
        {
            Card card = rm.GetCardInstance(cardId);
            card.instId = cardInstIds;
            cardInstIds++;

            return card;
        }


        void CreateCardClient_call (string cardId, int instId, int ownerId) 
        {
            Card c = CreateCardClient(cardId, instId);
            if (c != null) 
            {
                MultiplayerHolder h = GetHolder(ownerId);
                h.RegisterCard(c);
            }
        }

        Card CreateCardClient(string cardId, int instId)
        {
            Card card = rm.GetCardInstance(cardId);
            card.instId = instId;

            return card;
        }

        #endregion

        #region Photon Callbacks
        #endregion

        #region RPCs

        #endregion

    }

    public class MultiplayerHolder {
        public int ownerId;
        Dictionary<int, Card> cards = new Dictionary<int, Card>();

        public void RegisterCard(Card c)
        {
            cards.Add(c.instId, c);
        }

        public Card GetCard(int instId)
        {
            Card r = null;
            cards.TryGetValue(instId, out r);
            return r;
        }
    }
}