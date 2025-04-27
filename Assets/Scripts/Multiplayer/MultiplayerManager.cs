using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class MultiplayerManager : Photon.MonoBehaviour
    {
        #region Variables
        public static MultiplayerManager singleton;
        Transform multiplayerReferences;
        public MainDataHolder dataHolder;

        int playersReady = 0;
        int totalCardsToCreate = 0;
        int cardsCreated = 0;


        // public PlayerHolder localPlayerHolder;
        // public PlayerHolder clientPlayerHolder;

        bool gameStarted;
        public bool countPlayers;
        GameManager gm
        {
            get
            {
                return GameManager.singleton;
            }
        }


        #endregion

        #region Player Management
        List<NetworkPrint> players = new List<NetworkPrint>();
        NetworkPrint localPlayer;
        NetworkPrint GetPlayer(int photonId)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].photonId == photonId)
                {
                    return players[i];
                }

            }
            return null;
        }
        #endregion

        #region Init
        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            multiplayerReferences = new GameObject("references").transform;
            DontDestroyOnLoad(multiplayerReferences.gameObject);

            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            InstantiateNetworkPrint();
            NetworkManager.singleton.LoadGameScene();
        }

        void InstantiateNetworkPrint()
        {
            PlayerProfile profile = Resources.Load("PlayerProfile") as PlayerProfile;
            object[] data = new object[1];
            data[0] = profile.cardIds;

            PhotonNetwork.Instantiate("NetworkPrint", Vector3.zero, Quaternion.identity, 0, data);
        }
        #endregion

        #region Tick
        private void Update()
        {
            if (!gameStarted && countPlayers)
            {
                if (players.Count > 1)
                {
                    gameStarted = true;
                    StartMatch();
                }
            }
        }
        #endregion



        #region Starting the match
        public void StartMatch()
        {
            ResourcesManager rm = gm.resourcesManager;

            if (NetworkManager.isMaster)
            {
                cardsCreated = 0;
                totalCardsToCreate = 0;
                List<int> playerId = new List<int>();
                List<int> cardInstId = new List<int>();
                List<string> cardName = new List<string>();

                foreach (NetworkPrint p in players)
                {
                    foreach (string id in p.GetStartingCardIds())
                    {
                        Card card = rm.GetCardInstance(id);
                        playerId.Add(p.photonId);
                        cardInstId.Add(card.instId);
                        cardName.Add(id);

                        if (p.isLocal)
                        {
                            p.playerHolder = gm.localPlayer;
                            p.playerHolder.photonId = p.photonId;
                        }
                        else
                        {
                            p.playerHolder = gm.clientPlayer;
                            p.playerHolder.photonId = p.photonId;

                        }
                    }
                }

                totalCardsToCreate = playerId.Count;

                for (int i = 0; i < playerId.Count; i++)
                {
                    Debug.Log("RPC_PLAYERCREATESCARD");
                    photonView.RPC("RPC_PlayerCreatesCard", PhotonTargets.All, playerId[i], cardInstId[i], cardName[i]);
                }

                /* if (p.isLocal)
                // {
                //     localPlayerHolder.photonId = p.photonId;
                //     localPlayerHolder.all_cards.Clear();
                //     localPlayerHolder.all_cards.AddRange(p.GetStartingCardIds());
                // }
                // else
                // {
                //     clientPlayerHolder.photonId = p.photonId;
                //     clientPlayerHolder.all_cards.Clear();
                //     clientPlayerHolder.all_cards.AddRange(p.GetStartingCardIds());

                //     foreach (string id in p.GetStartingCardIds())
                //     {
                //         Card inst = rm.GetCardInstance(id);
                //         clientPlayerHolder.allCardInstances.Add(inst);
                //     }
                */

            }
            else
            {
                foreach (NetworkPrint p in players)
                {
                    if (p.isLocal)
                    {
                        p.playerHolder = gm.localPlayer;
                        p.playerHolder.photonId = p.photonId;
                    }
                    else
                    {
                        p.playerHolder = gm.clientPlayer;
                        p.playerHolder.photonId = p.photonId;
                    }
                }
            }
        }

        [PunRPC]
        public void RPC_PlayerCreatesCard(int photonId, int instId, string cardName)
        {
            Card c = gm.resourcesManager.GetCardInstance(cardName);
            c.instId = instId;

            NetworkPrint p = GetPlayer(photonId);
            p.AddCard(c);

            cardsCreated++;
            if (NetworkManager.isMaster && cardsCreated == totalCardsToCreate)
            {
                Debug.Log("RPC_INITGAME");
                photonView.RPC("RPC_InitGame", PhotonTargets.All, 1);
            }
        }


        [PunRPC]
        public void RPC_InitGame(int startingPlayer)
        {
            gm.InitGame(startingPlayer);
            gm.isMultiplayer = true;
        }

        public void AddPlayer(NetworkPrint n_print)
        {
            if (n_print.isLocal)
            {
                localPlayer = n_print;
            }
            players.Add(n_print);
            n_print.transform.parent = multiplayerReferences;
        }

        #endregion

        #region End Turn
        public void PlayerEndsTurn(int photonId)
        {
            photonView.RPC("RPC_PlayerEndsTurn", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerEndsTurn(int photonId)
        {
            if (photonId == gm.currentPlayer.photonId)
            {
                if (NetworkManager.isMaster)
                {
                    int targetId = gm.GetNextPlayerID();
                    photonView.RPC("RPC_PlayerStartsTurn", PhotonTargets.All, targetId);
                }
            }
        }

        [PunRPC]
        public void RPC_PlayerStartsTurn(int photonId)
        {
            gm.ChangeCurrentTurn(photonId);
        }
        #endregion

        #region Card Checks
        public void PlayerPicksCardFromDeck(PlayerHolder playerHolder)
        {
            NetworkPrint p = GetPlayer(playerHolder.photonId);
            
            Card c = p.deckCards[0];
            p.deckCards.RemoveAt(0);

            PlayerWantsToUseCard(c.instId, p.photonId, CardOperation.pickCardFromDeck);

        }


        public void PlayerWantsToUseCard(int cardInst, int photonId, CardOperation operation)
        {
            photonView.RPC("RPC_PlayerWantsToUseCard", PhotonTargets.MasterClient, cardInst, photonId, operation);
        }

        [PunRPC]
        public void RPC_PlayerWantsToUseCard(int cardInst, int photonId, CardOperation operation)
        {
            if (!NetworkManager.isMaster)
            {
                return;
            }
            bool hasCard = PlayerHasCard(cardInst, photonId);

            if (hasCard)
            {
                photonView.RPC("RPC_PlayerUsesCard", PhotonTargets.All, cardInst, photonId, operation);
            }


        }

        bool PlayerHasCard(int cardInst, int photonId)
        {
            NetworkPrint player = GetPlayer(photonId);
            Card c = player.GetCard(cardInst);
            return (c != null);
        }
        #endregion

        #region Card Operations
        public enum CardOperation
        {
            dropResourcesCard,
            pickCardFromDeck
        }

        [PunRPC]
        public void RPC_PlayerUsesCard(int instId, int photonId, CardOperation operation)
        {
            NetworkPrint p = GetPlayer(photonId);
            Card card = p.GetCard(instId);

            switch (operation)
            {
                case CardOperation.dropResourcesCard:
                    Settings.SetParentForCard(card.cardPhysicalInstance.transform, p.playerHolder.currentHolder.resourcesGrid.value);
                    card.cardPhysicalInstance.currentLogic = dataHolder.cardDownLogic;
                    p.playerHolder.AddResourceCard(card.cardPhysicalInstance.gameObject);
                    card.cardPhysicalInstance.transform.gameObject.SetActive(true);
                    break;
                case CardOperation.pickCardFromDeck:
                    GameObject go = Instantiate(dataHolder.cardPrefab) as GameObject;
                    CardViz v = go.GetComponent<CardViz>();
                    v.LoadCard(card);
                    card.cardPhysicalInstance = go.GetComponent<CardInstance>();
                    card.cardPhysicalInstance.currentLogic = dataHolder.handCard;
                    Settings.SetParentForCard(go.transform, p.playerHolder.currentHolder.handGrid.value);
                    p.playerHolder.handCards.Add(card.cardPhysicalInstance);
                    break;

                default:
                    break;

            }
        }
        #endregion
    }

}
