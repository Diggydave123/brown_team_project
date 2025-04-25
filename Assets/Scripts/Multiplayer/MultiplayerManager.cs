using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class MultiplayerManager : Photon.MonoBehaviour
    {
        #region Variables
        public static MultiplayerManager singleton;
        Transform multiplayerReferences;

        public PlayerHolder localPlayerHolder;
        public PlayerHolder clientPlayerHolder;

        bool gameStarted;
        public bool countPlayers;
        GameManager gm {
            get {
                return GameManager.singleton;
            }
        }


        #endregion

        #region Player Management
        List<NetworkPrint> players = new List<NetworkPrint>();
        NetworkPrint localPlayer;
        NetworkPrint GetPlayer(int photonId)
        {
            for(int i = 0; i < players.Count; i++) {
                if(players[i].photonId == photonId)
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
            if(!gameStarted && countPlayers)
            {
                if(players.Count > 1)
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
            if (NetworkManager.isMaster)
            {
                ResourcesManager rm = gm.resourcesManager;

                List<int> playerId = new List<int>();
                List<int> cardInstId = new List<int>();
                List<string> cardName = new List<string>();

                foreach(NetworkPrint p in players)
                {
                    foreach (string id in p.GetStartingCardIds())
                    {
                        Card card = rm.GetCardInstance(id);
                        playerId.Add(p.photonId);
                        cardInstId.Add(card.instId);
                        cardName.Add(id);

                        if(p.isLocal)
                        {
                            localPlayer.photonId = p.photonId;
                            localPlayerHolder.allCardInstances.Add(card);
                        }
                    }
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
            photonView.RPC("RPC_InitGame", PhotonTargets.All, 1);
        }

        [PunRPC]
        public void RPC_PlayerHasCard(int photonId, int cardId, string name)
        {
            if (NetworkManager.isMaster)
            {
                return;
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
                if(NetworkManager.isMaster)
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
        public void PlayerWantsToUseCard(int cardInst, int photonId) 
        {
            photonView.RPC("RPC_PlayerWantsToUseCard", PhotonTargets.MasterClient, cardInst, photonId);
        }

        [PunRPC]
        public void RPC_PlayerWantsToUseCard(int cardInst, int photonId)
        {
            if (!NetworkManager.isMaster)
            {
                return;
            }
        }

        // bool PlayerHasCard(int cardInst, int photonId)
        // {
        //     NetworkPrint player = GetPlayer(photonId);

        //     if (player.GetSt)
        // }
        #endregion
    }

}
