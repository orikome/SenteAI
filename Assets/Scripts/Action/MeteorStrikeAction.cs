using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;

    ActionReadinessModule actionReadinessModule;

    public override void Initialize(Agent agent)
    {
        actionReadinessModule = agent.GetModule<ActionReadinessModule>();
        Debug.Assert(actionReadinessModule != null, "ActionReadinessModule is not set!");
    }

    public override void ExecuteActionLoop(Transform firePoint, Agent agent)
    {
        DropMeteor(firePoint, agent);
        lastExecutedTime = Time.time;
    }

    public override bool CanExecute(Agent agent)
    {
        return Time.time - lastExecutedTime >= cooldownTime;
    }

    private void DropMeteor(Transform firePoint, Agent agent)
    {
        GameObject meteor = Instantiate(
            meteorPrefab,
            agent.target.position + (Vector3.up * 10),
            Quaternion.identity
        );
        Destroy(meteor, dropDelay + 1f);
    }
}
