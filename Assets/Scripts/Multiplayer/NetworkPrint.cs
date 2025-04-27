using System.Collections.Generic;
using UnityEngine;


namespace SA
{

    public class NetworkPrint : Photon.MonoBehaviour
    {

        public int photonId;
        public bool isLocal;

        string[] cardIds;
        public string[] GetStartingCardIds()
        {
            return cardIds;
        }

        public PlayerHolder playerHolder;
        Dictionary<int, Card> myCards = new Dictionary<int, Card>();
        public List<Card> deckCards = new List<Card>();

        public void AddCard(Card card)
        {
            myCards.Add(card.instId, card);
            deckCards.Add(card);
        }

        public Card GetCard(int instId)
        {
            Card c = null;
            myCards.TryGetValue(instId, out c);
            return c;
        }

        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            photonId = photonView.ownerId;
            isLocal = photonView.isMine;
            
            object[] data = photonView.instantiationData;
            cardIds = (string[])data[0];

            MultiplayerManager.singleton.AddPlayer(this);
        }
    }
 }
