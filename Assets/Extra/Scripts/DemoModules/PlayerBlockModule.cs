using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBlockModule", menuName = "SenteAI/Modules/PlayerBlockModule")]
public class PlayerBlockModule : Module
{
    [SerializeField]
    private Transform shieldPrefab;

    [SerializeField]
    private float maxChargeTime = 2f;

    [SerializeField]
    private float minSpeedMultiplier = 2f;

    [SerializeField]
    private float maxSpeedMultiplier = 6f;

    [SerializeField]
    private float swingDuration = 0.2f;

    [SerializeField]
    private float cooldownDuration = 1f;

    private SwingState currentState = SwingState.Ready;
    private float chargeStartTime;
    private float currentCharge;
    private float cooldownEndTime;

    private enum SwingState
    {
        Ready,
        Charging,
        Swinging,
        Cooldown,
    }

    public override void Execute()
    {
        switch (currentState)
        {
            case SwingState.Ready:
                if (Input.GetMouseButtonDown(1))
                {
                    currentState = SwingState.Charging;
                    chargeStartTime = Time.time;
                }
                break;

            case SwingState.Charging:
                currentCharge = Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime);

                if (!Input.GetMouseButton(1))
                {
                    currentState = SwingState.Swinging;
                    cooldownEndTime = Time.time + swingDuration;
                }
                break;

            case SwingState.Swinging:
                CheckForProjectiles(GetCurrentSpeedMultiplier());

                if (Time.time >= cooldownEndTime)
                {
                    currentState = SwingState.Cooldown;
                    cooldownEndTime = Time.time + cooldownDuration;
                }
                break;

            case SwingState.Cooldown:
                if (Time.time >= cooldownEndTime)
                {
                    currentState = SwingState.Ready;
                }
                break;
        }
    }

    private float GetCurrentSpeedMultiplier()
    {
        return Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, currentCharge);
    }

    private void CheckForProjectiles(float speedMultiplier)
    {
        int projectileMask = LayerMask.GetMask("EnemyProjectile");

        Collider[] hitColliders = Physics.OverlapSphere(
            _agent.transform.position,
            4f,
            projectileMask
        );

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider && hitCollider.TryGetComponent<Projectile>(out var projectile))
            {
                Vector3 reflectionDir = -projectile.MoveDirection;
                projectile.SetParameters(
                    _agent,
                    reflectionDir,
                    projectile.Speed * speedMultiplier,
                    projectile.Damage
                );

                Helpers.SpawnParticles(
                    hitCollider.transform.position,
                    Helpers.GetFactionColorHex(_agent.Faction)
                );
            }
            else if (hitCollider && hitCollider.TryGetComponent<HomingOrbBehaviour>(out var homing))
            {
                homing.Initialize(_agent);
            }
        }
    }
}
