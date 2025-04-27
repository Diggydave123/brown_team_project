using SO;
using UnityEngine;

namespace SA
{
    [CreateAssetMenu]
    public class MainDataHolder : ScriptableObject
    {
        public GameObject cardPrefab;
        public GameElements.GE_Logic cardDownLogic;
        public GameElements.GE_Logic handCard;
        
    }

}
