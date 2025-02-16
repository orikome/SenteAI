using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(fileName = "HearingModule", menuName = "SenteAI/Modules/HearingModule")]
    public class HearingModule : SenseModule
    {
        //[SerializeField]
        //private float hearingRange = 15f;

        public override void Execute()
        {
            // Check if there are any sounds within hearingRange
            // If so, update lastKnownLocation
        }

        public override void Initialize(Agent agent) { }
    }
}
