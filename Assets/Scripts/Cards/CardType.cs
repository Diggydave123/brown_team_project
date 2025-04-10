using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu(menuName = "Cards/Creature")]
    public abstract class CardType : ScriptableObject
    {
        public string typeName;
        public virtual void OnSetType(CardViz viz)
        {
            Element t = Settings.GetResourcesManager().typeElement;
            CardVizProperties type = viz.GetProperty(t);
            // Debug.Log("Element: " + t);
            // Debug.Log("CardVizProperty: " + type);
            // Debug.Log("Text Component: " + type?.text);
            type.text.text = typeName;
        }
    }
}