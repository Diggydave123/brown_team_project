using SA.GameStates;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA 
{
    public class GameManager : MonoBehaviour
    {
        public State currentState;

        private void Update()
        {
            currentState.Tick(Time.deltaTime);
        }
    }
}
