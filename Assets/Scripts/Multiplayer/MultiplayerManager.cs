using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections;

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
        bool isDead = false;


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

        QuizManager qm
        {
            get
            {
                return QuizManager.singleton;
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
            Debug.Log(gameStarted);
            multiplayerReferences = new GameObject("references").transform;
            DontDestroyOnLoad(multiplayerReferences.gameObject);
            multiplayerReferences.gameObject.tag = "Persistent";

            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            gameObject.tag = "Persistent";

            InstantiateNetworkPrint();
            NetworkManager.singleton.LoadGameScene();
        }

        void InstantiateNetworkPrint()
        {
            int actorId = PhotonNetwork.player.ID;
            Debug.Log("Actor Id: " + actorId);

            PlayerProfile profile = CreateProfileForPlayer(actorId); //Resources.Load("PlayerProfile") as PlayerProfile;

            object[] data = new object[1];
            data[0] = profile.cardIds;

            PhotonNetwork.Instantiate("NetworkPrint", Vector3.zero, Quaternion.identity, 0, data);
        }

        PlayerProfile CreateProfileForPlayer(int actorId)
        {
            PlayerProfile profile = ScriptableObject.CreateInstance<PlayerProfile>();

            if (actorId == 1)
            {
                profile.cardIds = new string[] { "RESOURCE", "TAXMAN", "TAXMAN", "SPELL", "REPO" };
            }
            else if (actorId == 2)
            {
                profile.cardIds = new string[] { "RESOURCE", "TAXMAN", "SPELL", "REPO", "RESOURCE" };
            }
            else
            {
                profile.cardIds = new string[] { "RESOURCE", "REPO", "SPELL", "TAXMAN", "REPO" };
            }

            return profile;
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
            pickCardFromDeck,
            dropCreatureCard,
            setCardForBattle,
            cardToGraveyard,
            cardToStandby,
            dropSpellCard
        }

        [PunRPC]
        public void RPC_PlayerUsesCard(int instId, int photonId, CardOperation operation)
        {
            NetworkPrint p = GetPlayer(photonId);
            Card card = p.GetCard(instId);
            PlayerHolder player = Settings.gameManager.currentPlayer;
            PlayerHolder enemy = Settings.gameManager.GetEnemyOf(player);

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
                    card.cardPhysicalInstance.owner = p.playerHolder;
                    Settings.SetParentForCard(go.transform, p.playerHolder.currentHolder.handGrid.value);
                    p.playerHolder.handCards.Add(card.cardPhysicalInstance);
                    break;
                case CardOperation.dropCreatureCard:
                    bool canUse = p.playerHolder.CanUseCard(card);

                    if (canUse)
                    {
                        Settings.DropCreatureCard(card.cardPhysicalInstance.transform, p.playerHolder.currentHolder.downGrid.value.transform, card.cardPhysicalInstance);
                        card.cardPhysicalInstance.currentLogic = dataHolder.cardDownLogic;
                    }
                    else
                    {
                        Settings.RegisterEvent("Not enough resources to use card", Color.red);
                    }
                    card.cardPhysicalInstance.gameObject.SetActive(true);
                    break;
                case CardOperation.dropSpellCard:
                    if (PhotonNetwork.player.ID == photonId)
                    {
                        qm.OnQuestionFinished = (bool correct) =>
                        {
                            if (correct)
                            {
                                Debug.Log("✅ Correct! Trigger spell effect. Damage done to enemy 2");
                                isDead = enemy.DoDamage(2);
                                photonView.RPC("RPC_SyncPlayerHealth", PhotonTargets.All, enemy.photonId, enemy.health, isDead);
                            }
                            else
                            {
                                Debug.Log("❌ Incorrect! Small Backfire Spell effect. Damage done to self 1");
                                isDead = player.DoDamage(1);
                                photonView.RPC("RPC_SyncPlayerHealth", PhotonTargets.All, player.photonId, player.health, isDead);

                            }
                        };
                        qm.TriggerNextQuestionFromGame();
                    }
                    card.cardPhysicalInstance.CardInstanceToGraveyard(p.playerHolder);

                    break;
                case CardOperation.setCardForBattle:
                    if (p.playerHolder.attackingCards.Contains(card.cardPhysicalInstance))
                    {
                        p.playerHolder.attackingCards.Remove(card.cardPhysicalInstance);
                        p.playerHolder.currentHolder.SetCardDown(card.cardPhysicalInstance);
                    }
                    else
                    {
                        if (card.cardPhysicalInstance.CanAttack() && !p.playerHolder.attackingCards.Contains(card.cardPhysicalInstance) && !card.cardPhysicalInstance.isFlatfooted)
                        {
                            p.playerHolder.attackingCards.Add(card.cardPhysicalInstance);
                            // Debug.Log("Card added to attacking cards, number of attacking cards: " +  p.attackingCards.Count.ToString());
                            p.playerHolder.currentHolder.SetCardOnBattleLine(card.cardPhysicalInstance);
                        }
                    }
                    break;
                case CardOperation.cardToStandby:
                    Settings.DropCreatureCardWithoutResources(card.cardPhysicalInstance.transform, p.playerHolder.currentHolder.downGrid.value.transform, card.cardPhysicalInstance);
                    card.cardPhysicalInstance.currentLogic = dataHolder.cardDownLogic;
                    break;
                case CardOperation.cardToGraveyard:
                    card.cardPhysicalInstance.CardInstanceToGraveyard(p.playerHolder);
                    break;
                default:
                    break;

            }
        }

        #endregion

        #region Battle Resolve
        public void SetBattleResolvePhase()
        {
            photonView.RPC("RPC_BattleResolve", PhotonTargets.MasterClient);
        }

        [PunRPC]
        public void RPC_BattleResolve()
        {
            if (!NetworkManager.isMaster)
            {
                return;
            }

            BattleResolveForPlayers();
        }

        void BattleResolveForPlayers()
        {
            PlayerHolder player = Settings.gameManager.currentPlayer;
            PlayerHolder enemy = Settings.gameManager.GetEnemyOf(player);

            if (enemy.attackingCards.Count == 0)
            {
                photonView.RPC("RPC_BattleResolveCallback", PhotonTargets.All, enemy.photonId);
                return;
            }

            Dictionary<CardInstance, BlockInstance> blockDict = Settings.gameManager.GetBlockInstances();

            for (int i = 0; i < enemy.attackingCards.Count; i++)
            {
                CardInstance inst = enemy.attackingCards[i];
                Card c = inst.viz.card;
                CardProperties attack = c.GetProperty(dataHolder.attackElement);
                if (attack == null)
                {
                    Debug.LogError("You are attacking with a card that can't attack");
                    continue;
                }

                int attackValue = attack.intValue;

                BlockInstance bi = GetBlockInstanceOfAttacker(inst, blockDict);
                if (bi != null)
                {
                    Debug.Log("There is block");
                    for (int b = 0; b < bi.blocker.Count; b++)
                    {
                        CardInstance blockInst = bi.blocker[b];
                        Card blockingCard = blockInst.viz.card;
                        CardProperties def = blockingCard.GetProperty(dataHolder.defenseElement);
                        if (def == null)
                        {
                            Debug.LogWarning("You are trying to block with a card with no defense element!");
                            continue;
                        }

                        attackValue -= def.intValue;

                        if (def.intValue <= attack.intValue)
                        {
                            // Card dies
                            Debug.Log("Defensor dies");
                            PlayerWantsToUseCard(blockingCard.instId, player.photonId, CardOperation.cardToGraveyard);
                            // blockInst.CardInstanceToGraveyard();
                        }
                        else
                        {
                            PlayerWantsToUseCard(blockingCard.instId, player.photonId, CardOperation.cardToStandby);
                        }
                    }
                }

                Debug.Log("Attack Value:" + attackValue);
                if (attackValue <= 0)
                {
                    Debug.Log("Attacker dies");
                    attackValue = 0;
                    PlayerWantsToUseCard(inst.viz.card.instId, enemy.photonId, CardOperation.cardToGraveyard);
                }


                // enemy.DropCard(inst, false);
                // p.currentHolder.SetCardDown(inst);
                // inst.SetFlatfooted(true);

                isDead = player.DoDamage(attackValue);
                photonView.RPC("RPC_SyncPlayerHealth", PhotonTargets.All, player.photonId, player.health, isDead);
                Debug.Log("Damage done: " + attack.intValue.ToString());
            }

            photonView.RPC("RPC_BattleResolveCallback", PhotonTargets.All, enemy.photonId);
            return;
        }

        BlockInstance GetBlockInstanceOfAttacker(CardInstance attacker, Dictionary<CardInstance, BlockInstance> blockInstances)
        {
            BlockInstance r = null;
            blockInstances.TryGetValue(attacker, out r);
            return r;
        }

        [PunRPC]
        public void RPC_SyncPlayerHealth(int photonId, int health, bool isDead)
        {
            NetworkPrint p = GetPlayer(photonId);
            p.playerHolder.health = health;
            p.playerHolder.statsUI.UpdateHealth();

            if (isDead)
            {
                photonView.RPC("RPC_OnPlayerDied", PhotonTargets.All, photonId);
            }
        }

        [PunRPC]
        public void RPC_BattleResolveCallback(int photonId)
        {
            foreach (NetworkPrint p in players)
            {
                foreach (CardInstance c in p.playerHolder.attackingCards)
                {
                    p.playerHolder.currentHolder.SetCardDown(c);
                    c.SetFlatfooted(true);

                }

                if (p.photonId == photonId)
                {
                    if (p == localPlayer)
                    {
                        gm.EndCurrentPhase();
                    }
                }

                p.playerHolder.attackingCards.Clear();
            }

            foreach (BlockInstance bi in gm.GetBlockInstances().Values)
            {
                foreach (CardInstance c in bi.blocker)
                {
                    if (gm.graveyardCards.Contains(c))
                    {
                        continue;
                    }
                    Debug.Log("Block Instance set card down.");
                    c.owner.currentHolder.SetCardDown(c);
                    // c.SetFlatfooted(true);
                }
            }

            gm.ClearBlockInstances();
        }

        #endregion

        #region Blocking
        public void PlayerBlocksTargetCard(int cardInst, int photonId, int targetInst, int blocked)
        {
            photonView.RPC("RPC_PlayerBlocksTargetCard_Master", PhotonTargets.MasterClient, cardInst, photonId, targetInst, blocked);
        }

        [PunRPC]
        public void RPC_PlayerBlocksTargetCard_Master(int cardInst, int photonId, int targetInst, int blocked)
        {
            NetworkPrint blockerPlayer = GetPlayer(photonId);
            Card blockerCard = blockerPlayer.GetCard(cardInst);
            NetworkPrint blockedPlayer = GetPlayer(blocked);
            Card blockedCard = blockedPlayer.GetCard(targetInst);

            int count = 0;

            Settings.gameManager.AddBlockInstance(blockedCard.cardPhysicalInstance, blockerCard.cardPhysicalInstance, ref count);

            photonView.RPC("RPC_PlayerBlocksTargetCard_Client", PhotonTargets.All, cardInst, photonId, targetInst, blocked, count);

        }

        [PunRPC]
        public void RPC_PlayerBlocksTargetCard_Client(int cardInst, int photonId, int targetInst, int blocked, int count)
        {
            NetworkPrint blockerPlayer = GetPlayer(photonId);
            Card blockerCard = blockerPlayer.GetCard(cardInst);
            NetworkPrint blockedPlayer = GetPlayer(blocked);
            Card blockedCard = blockedPlayer.GetCard(targetInst);

            Settings.SetCardForBlock(blockerCard.cardPhysicalInstance.transform, blockedCard.cardPhysicalInstance.transform, count);

        }

        #endregion

        #region Multiple Card Operations
        #region Flatfooted Cards
        public void PlayerWantsToResetFlatfootedCards(int photonId)
        {
            photonView.RPC("RPC_ResetFlatfootedCardsForPlayer_Master", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_ResetFlatfootedCardsForPlayer_Master(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            if (gm.turns[gm.turnIndex].player == p.playerHolder)
            {
                photonView.RPC("RPC_ResetFlatfootedCardsForPlayer", PhotonTargets.All, photonId);

            }
        }

        [PunRPC]
        public void RPC_ResetFlatfootedCardsForPlayer(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            foreach (CardInstance c in p.playerHolder.cardsDown)
            {
                if (c.isFlatfooted)
                {
                    c.SetFlatfooted(false);
                }
            }
        }
        #endregion

        #region Resource Cards
        public void PlayerWantsToResetResourcesCards(int photonId)
        {
            photonView.RPC("RPC_PlayerWantsToResetResourcesCards_Master", PhotonTargets.MasterClient, photonId);
        }

        [PunRPC]
        public void RPC_PlayerWantsToResetResourcesCards_Master(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            if (gm.turns[gm.turnIndex].player == p.playerHolder)
            {
                photonView.RPC("RPC_ResetResourcesCardsForPlayer", PhotonTargets.All, photonId);

            }
        }

        [PunRPC]
        public void RPC_ResetResourcesCardsForPlayer(int photonId)
        {
            NetworkPrint p = GetPlayer(photonId);
            p.playerHolder.MakeAllResourceCardsUsable();
        }
        #endregion

        // #region Battle
        // [PunRPC]
        // public void RPC_PlayerSetsCardForBattle(int photonId, int cardInst)
        // {
        //     NetworkPrint p = GetPlayer(photonId);

        // }
        // #endregion

        #region Management
        public void SendPhase(string holder, string phase)
        {
            // photonView.RPC("RPC_MessagePhase", PhotonTargets.All, phase, holder);
        }

        [PunRPC]
        public void RPC_MessagePhase(string phase, string holder)
        {
            // Debug.Log(phase + " " + holder);
        }

        #endregion
        #endregion

        #region Game Over

        [PunRPC]
        void RPC_OnPlayerDied(int deadPlayerId)
        {
            int localId = PhotonNetwork.player.ID;


            if (localId == deadPlayerId)
            {
                ShowGameOver("You Lose");
            }
            else
            {
                ShowGameOver("You Win");
            }
        }

        void ShowGameOver(string message)
        {
            gm.gameOverText.text = message;
            gm.gameOverPanel.SetActive(true);
            Time.timeScale = 0f;

        }

        #endregion
    }

}
