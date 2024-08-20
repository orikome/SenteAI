using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;

    public override void ExecuteActionLoop(Transform firePoint, Agent agent)
    {
        DropMeteor(firePoint);
    }

    public override void Initialize(Agent agent) { }

    private void DropMeteor(Transform firePoint)
    {
        Vector3 targetPosition = Player.Instance.transform.position;
        GameObject meteor = Instantiate(
            meteorPrefab,
            targetPosition + (Vector3.up * 10),
            Quaternion.identity
        );
        Destroy(meteor, dropDelay + 1f);
    }
}
