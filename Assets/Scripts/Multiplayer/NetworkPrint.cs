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
