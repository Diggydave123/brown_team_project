using UnityEngine;
using UnityEngine.UI;

namespace SA 
{
    public class PlayerStatsUI : MonoBehaviour
    {
        public PlayerHolder player;
        public Image playerPortrait;
        public Text health;
        public Text userName;

        public void UpdateAll()
        {
            UpdateUsername();
            UpdateHealth();
        }

        public void UpdateUsername()
        {
            userName.text = player.username;
            playerPortrait.sprite = player.portrait;
        }

        public void UpdateHealth()
        {
            health.text = player.health.ToString();

        }

    }
    
}
