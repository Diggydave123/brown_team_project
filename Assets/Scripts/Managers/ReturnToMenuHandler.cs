using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using SA;
using System.Collections;

public class ReturnToMenuHandler : UnityEngine.MonoBehaviour
{
    public void OnReturnToMenuClicked()
    {
        StartCoroutine(ReturnToMenuRoutine());
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        Debug.Log("Initiating return to menu...");

        // Prevent any further RPCs by stopping Photon networking
        PhotonNetwork.isMessageQueueRunning = false;

        // Leave the Photon room
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();

            // float timeout = 5f;
            // while (PhotonNetwork.inRoom && timeout > 0f)
            // {
            //     timeout -= Time.deltaTime;
            //     yield return null;
            // }

            Debug.Log("Left Photon room or timeout occurred.");
        }

        ClearGameState();

        // Now safe to kill persistent objects
        DestroyPersistentObjects();

        // Optional short delay to ensure cleanup finishes
        yield return null;

        // Re-enable message queue (optional if rejoining later)
        PhotonNetwork.isMessageQueueRunning = true;

        Debug.Log("Loading Menu scene...");
        SceneManager.LoadScene("Menu");
    }


    private void DestroyPersistentObjects()
    {
        GameObject[] persistentObjects = GameObject.FindGameObjectsWithTag("Persistent");
        foreach (GameObject obj in persistentObjects)
        {
            Destroy(obj);
        }
    }

    void ClearGameState()
    {
        GameManager.singleton.playerOneHolder.ClearAllCards();
        GameManager.singleton.otherPlayersHolder.ClearAllCards();
        MultiplayerManager.singleton = null;
        GameManager.singleton = null;
        Resources.UnloadUnusedAssets();
        
    }

}
