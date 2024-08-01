using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/HomingOrbs")]
public class HomingOrbsAction : AgentAction
{
    public GameObject orbPrefab;
    public int numberOfOrbs = 3;
    public float spreadAngle = 45f;

    public override void ExecuteAction(Transform firePoint, Agent agent)
    {
        ShootOrbs(firePoint);
    }

    private void ShootOrbs(Transform firePoint)
    {
        for (int i = 0; i < numberOfOrbs; i++)
        {
            float angle = (i - numberOfOrbs / 2) * spreadAngle / numberOfOrbs;
            Quaternion rotation = Quaternion.Euler(0, angle, 0) * firePoint.rotation;
            GameObject orb = Instantiate(orbPrefab, firePoint.position, rotation);
            Destroy(orb, 8);
        }
    }
}
