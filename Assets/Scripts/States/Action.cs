using SA.GameStates;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SA.GameStates
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute(float d);
    }
}
