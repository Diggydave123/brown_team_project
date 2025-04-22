using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class BlockInstance
    {
        public CardInstance attacker;
        public List<CardInstance> blocker = new List<CardInstance>();
    }
}
