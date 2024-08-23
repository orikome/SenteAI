using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/BulletPatternAction")]
public class BulletPatternAction : AgentAction
{
    public GameObject bulletPrefab;
    public int numberOfBullets = 10;
    public float spreadAngle = 30f;
    public float bulletSpeed = 10f;

    public override void Initialize(Agent agent) { }

    public override void ExecuteLoop(Transform firePoint, Agent agent) { }

    private void GeneratePattern(Transform firePoint) { }

    public override bool CanExecute(Agent agent)
    {
        return agent.perceptionModule.CanSenseTarget && GetCooldownTimeRemaining() <= 0;
    }
}
