using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
    public abstract class CardType : ScriptableObject
    {
    public string typeName;
    public abstract void OnSetType(CardViz viz);
    }
}