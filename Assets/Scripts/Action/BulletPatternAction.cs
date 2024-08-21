using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/BulletPatternAction")]
public class BulletPatternAction : AgentAction
{
    public GameObject bulletPrefab;
    public int numberOfBullets = 10;
    public float spreadAngle = 30f;
    public float bulletSpeed = 10f;

    SeeingModule seeingModule;
    ActionReadinessModule actionReadinessModule;

    public override void Initialize(Agent agent)
    {
        seeingModule = agent.GetModule<SeeingModule>();
        actionReadinessModule = agent.GetModule<ActionReadinessModule>();
        Debug.Assert(seeingModule != null, "SeeingModule is not set!");
        Debug.Assert(actionReadinessModule != null, "ActionReadinessModule is not set!");
    }

    public override void ExecuteActionLoop(Transform firePoint, Agent agent) { }

    public override void UpdateUtilityLoop(Agent agent) { }

    private float CalculateUtility(float distance, float health, float energy)
    {
        return 0;
    }

    private void GeneratePattern(Transform firePoint) { }

    public override bool CanExecute(Agent agent)
    {
        return seeingModule.canSeeTarget && Time.time - lastExecutedTime >= cooldownTime;
    }
}
