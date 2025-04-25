using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SA
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager singleton;
        public delegate void OnSceneLoaded();
        public OnSceneLoaded onSceneLoaded;

        void Awake()
        {
            if (singleton == null)
            {
                singleton = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void LoadGameLevel(OnSceneLoaded callback)
        {
            onSceneLoaded = callback;
            StartCoroutine(LoadLevel("gameScene"));
        }
        
        public void LoadMenu()
        {
            StartCoroutine(LoadLevel("Menu"));

        }

        IEnumerator LoadLevel(string level) 
        {
            yield return SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
            if (onSceneLoaded != null)
            {
                onSceneLoaded();
                onSceneLoaded = null;
            }

        }
    }
}
