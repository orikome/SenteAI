using UnityEngine;

[CreateAssetMenu(fileName = "HearingModule", menuName = "Module/HearingModule")]
public class HearingModule : SenseModule
{
    //[SerializeField]
    //private float hearingRange = 15f;

    public override void Execute(EnemyAgent agent)
    {
        // Check if there are any sounds within hearingRange
        // If so, update lastKnownLocation
    }

    public override void Initialize(EnemyAgent agent) { }
}
