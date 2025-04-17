using UnityEngine;
using System.Collections;

namespace SA
{
    [CreateAssetMenu(menuName = "Variable/Card Variable")]
    public class CardVariable : ScriptableObject
    {
        public CardInstance value;

        public void Set(CardInstance v)
        {
            value = v;
        }
    }

}
