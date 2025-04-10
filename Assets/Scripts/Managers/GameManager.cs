using SA.GameStates;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA 
{
    public class GameManager : MonoBehaviour
    {
        public State currentState;

        private void Start()
        {
            Settings.gameManager = this;
        }

        private void Update()
        {
            currentState.Tick(Time.deltaTime);
        }

        public void SetState(State state)
        {
            currentState = state;
        }

    }
}
