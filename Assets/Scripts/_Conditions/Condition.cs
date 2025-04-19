using UnityEngine;

namespace SA
{
    public abstract class Condition : ScriptableObject
    {
        public abstract bool IsValid();
    }
}