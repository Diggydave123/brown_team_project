using SA.GameStates;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA 
{
    public class GameManager : MonoBehaviour
    {
        public PlayerHolder currentPlayer;
        public State currentState;
        public GameObject cardPrefab;
        public SO.GameEvent onTurnChanged;
        public SO.GameEvent onPhaseChanged;
        public SO.StringVariable turnText;

        public int turnIndex;
        public Turn[] turns;


        private void Start()
        {
            Settings.gameManager = this;
            CreateStartingCards();
            turnText.value = turns[turnIndex].player.username;
            onTurnChanged.Raise();
        }

        void CreateStartingCards()
        {
            ResourcesManager rm = Settings.GetResourcesManager();

            for (int i = 0; i < currentPlayer.startingCards.Length; i++)
            {
                GameObject go = Instantiate(cardPrefab) as GameObject;
                CardViz v = go.GetComponent<CardViz>();
                v.LoadCard(rm.GetCardInstance(currentPlayer.startingCards[i]));
                CardInstance inst = go.GetComponent<CardInstance>();
                inst.currentLogic = currentPlayer.handLogic;
                Settings.SetParentForCard(go.transform, currentPlayer.handGrid.value);
        
            }
        }

        private void Update()
        {
            bool IsComplete = turns[turnIndex].Execute();

            if (IsComplete)
            {
                turnIndex++;
                if (turnIndex > turns.Length - 1)
                {
                    turnIndex = 0;
                }

                turnText.value = turns[turnIndex].player.username;
                onTurnChanged.Raise();
            }

            if (currentState != null)
            {
                currentState.Tick(Time.deltaTime);
            }
        }

        public void SetState(State state)
        {
            currentState = state;
        }

    }
}
