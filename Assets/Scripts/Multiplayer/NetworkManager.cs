using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace SA
{
    public class NetworkManager : Photon.PunBehaviour
    {
        public static bool isMaster;
        public static NetworkManager singleton;

        List<MultiplayerHolder> multiplayerHolders = new List<MultiplayerHolder>();
        public MultiplayerHolder GetHolder(int ownerId)
        {
            for (int i = 0; i < multiplayerHolders.Count; i++)
            {
                if (multiplayerHolders[i].ownerId == ownerId)
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

        public StringVariable logger;
        public GameEvent loggerUpdated;
        public GameEvent failedToConnect;
        public GameEvent onConnected;
        public GameEvent waitingForPlayer;

        private void Awake()
        {
            if (singleton != null && singleton != this)
            {
                Destroy(this.gameObject);
                return;
            }

            singleton = this;
            DontDestroyOnLoad(this.gameObject);

            gameObject.tag = "Persistent";

            if (rm == null)
            {
                rm = Resources.Load("ResourcesManager") as ResourcesManager;
            }
        }

        private void Start()
        {
            rm.Init();
            PhotonNetwork.autoCleanUpPlayerObjects = false;
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = false;
            Init();
        }

        public void Init()
        {
            PhotonNetwork.ConnectUsingSettings("1");
            logger.value = "Connecting";
            loggerUpdated.Raise();

        }

        #region My Calls
        public void OnPlayGame()
        {
            JoinRandomRoom();
        }

        void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        void CreateRoom()
        {
            RoomOptions room = new RoomOptions();
            room.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(RandomString(256), room, TypedLobby.Default);
        }

        private System.Random random = new System.Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNUPQRSTUVWXYZ0123456789abcdefgolkip";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //Master only
        public void PlayerJoined(int ownerId, string[] cards)
        {
            MultiplayerHolder m = new MultiplayerHolder();
            m.ownerId = ownerId;

            for (int i = 0; i < cards.Length - 1; i++)
            {
                Card c = CreateCardMaster(cards[i]);
                if (c == null)
                {
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


        // void CreateCardClient_call (string cardId, int instId, int ownerId) 
        // {
        //     Card c = CreateCardClient(cardId, instId);
        //     if (c != null) 
        //     {
        //         MultiplayerHolder h = GetHolder(ownerId);
        //         h.RegisterCard(c);
        //     }
        // }

        // Card CreateCardClient(string cardId, int instId)
        // {
        //     Card card = rm.GetCardInstance(cardId);
        //     card.instId = instId;

        //     return card;
        // }

        #endregion

        #region Photon Callbacks
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            logger.value = "Connected";
            loggerUpdated.Raise();
            onConnected.Raise();
        }

        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            base.OnFailedToConnectToPhoton(cause);
            logger.value = "Failed To Connect";
            loggerUpdated.Raise();
            failedToConnect.Raise();
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            base.OnPhotonRandomJoinFailed(codeAndMsg);
            CreateRoom();
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            isMaster = true;
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            logger.value = "Waiting for player";
            loggerUpdated.Raise();
            waitingForPlayer.Raise();
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            if (isMaster)
            {
                if (PhotonNetwork.playerList.Length > 1)
                {
                    logger.value = "Ready for match";
                    loggerUpdated.Raise();

                    PhotonNetwork.room.IsOpen = false;
                    PhotonNetwork.Instantiate("MultiplayerManager", Vector3.zero, Quaternion.identity, 0);
                }
            }
        }

        public void LoadGameScene()
        {
            SessionManager.singleton.LoadGameLevel(OnGameSceneLoaded);
        }

        public void LoadMenu()
        {
            if (PhotonNetwork.inRoom)
            {
                PhotonNetwork.LeaveRoom(); // OnLeftRoom handles loading the menu
            }
            else
            {
                SceneManager.LoadScene("Menu");
            }
        }


        void OnGameSceneLoaded()
        {
            MultiplayerManager.singleton.countPlayers = true;
        }

        public override void OnDisconnectedFromPhoton()
        {
            base.OnDisconnectedFromPhoton();
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            // Debug.Log("Left room. Cleaning up and returning to menu.");
            // DestroyPersistentObjects();

            // StartCoroutine(DelayedReturnToMenu());
        }

        private IEnumerator DelayedReturnToMenu()
        {
            yield return new WaitForSeconds(1); // Small delay to ensure Photon is ready
            SceneManager.LoadScene("Menu");
        }


        #endregion

        private void DestroyPersistentObjects()
        {
            GameObject[] persistents = GameObject.FindGameObjectsWithTag("Persistent");

            foreach (GameObject go in persistents)
            {
                Destroy(go);
            }

            // Or destroy known objects manually if tag-based fails:
            // Destroy(MultiplayerManager.singleton?.gameObject);
        }


        #region RPCs

        #endregion

    }

    public class MultiplayerHolder
    {
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