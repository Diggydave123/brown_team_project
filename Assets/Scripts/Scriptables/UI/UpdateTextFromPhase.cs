using UnityEngine;
using UnityEngine.UI;
using SO.UI;
using TMPro; 

namespace SO
{
    public class UpdateTextFromPhase : UIPropertyUpdater
    {
        
        public PhaseVariable currentPhase;
        public TextMeshProUGUI targetText; 
            
        /// <summary>
        /// Use this to update a TextMesh Pro UI element based on the target string variable
        /// </summary>
        public override void Raise()
        {
            targetText.text = currentPhase.value.phaseName;
        }
    }
}
