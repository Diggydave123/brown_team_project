using UnityEngine;
using System.Collections;

namespace SA
{
    public class CardInstance : MonoBehaviour, IClickable
    {
        public PlayerHolder owner;
        public CardViz viz;
        public SA.GameElements.GE_Logic currentLogic;
        public bool isFlatfooted;

        public void SetFlatfooted(bool isFlat)
        {
            // Safely check if the object or its transform has been destroyed
            if (this == null || gameObject == null || transform == null) return;

            isFlatfooted = isFlat;

            try
            {
                transform.localEulerAngles = isFlat
                    ? new Vector3(0, 0, 90)
                    : Vector3.zero;
            }
            catch (MissingReferenceException ex)
            {
                Debug.LogWarning("Tried to rotate a destroyed card: " + ex.Message);
            }
        }


        void Start()
        {
            viz = GetComponent<CardViz>();
        }

        public void CardInstanceToGraveyard(PlayerHolder p)
        {
            Debug.Log(this);
            Settings.gameManager.PutCardToGraveyard(this, p);
        }

        public bool CanBeBlocked(CardInstance block, ref int count)
        {
            bool result = owner.attackingCards.Contains(this);

            if (result && viz.card.cardType.canAttack)
            {
                result = true;

                //if a card has flying that can be blocked by non flying, you can check it here
                //or cases like that should be here

                if (result)
                {
                    // Settings.gameManager.AddBlockInstance(this, block, ref count);
                }

                return result;
            }
            else
            {
                return false;
            }
        }

        public bool CanAttack()
        {
            bool result = true;

            if (isFlatfooted)
            {
                result = false;
            }

            if (viz.card.cardType.TypeAllowsForAttack(this))
            {
                result = true;
            }

            return result;
        }

        public void OnClick()
        {
            if (currentLogic == null)
            {
                return;
            }
            currentLogic.OnClick(this);
        }

        public void OnHighlight()
        {
            if (currentLogic == null)
            {
                return;
            }

            currentLogic.OnHighlight(this);
        }
    }

}
