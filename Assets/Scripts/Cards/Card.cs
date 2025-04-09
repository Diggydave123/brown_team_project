using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace SA{
    [CreateAssetMenu(menuName = "Card")]
    public class Card : ScriptableObject {
        public string cardName;
        public Sprite art;
        public string cardDetail;
        public string cardFlavor;

    }
}

