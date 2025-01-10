using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBlockModule", menuName = "SenteAI/Modules/PlayerBlockModule")]
public class PlayerBlockModule : Module
{
    private Shield shieldPrefab;
    private float maxChargeTime = 1.5f;
    private float minSpeedMultiplier = 2f;
    private float maxSpeedMultiplier = 6f;
    private float swingDuration = 0.05f;
    private float cooldownDuration = 1f;
    private float returnSpeed = 8f;
    private SwingState currentState = SwingState.Ready;
    private float chargeStartTime;
    private float currentCharge;
    private float cooldownEndTime;

    // Animation
    private float shieldOffset = 2f;
    private float minShieldScale = 1f;
    private float maxShieldScale = 2f;
    private Vector3 initialShieldScale;
    private Vector3 initialPosition;
    public GameObject reflectParticles;

    private enum SwingState
    {
        Ready,
        Charging,
        Swinging,
        Cooldown,
    }

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        shieldPrefab = _agent.GetComponentInChildren<Shield>();
        initialPosition = shieldPrefab.transform.localPosition;
        initialShieldScale = shieldPrefab.transform.localScale;
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
                    initialPosition = shieldPrefab.transform.localPosition;
                    initialShieldScale = shieldPrefab.transform.localScale;
                }
                break;

            case SwingState.Charging:
                currentCharge = Mathf.Clamp01((Time.time - chargeStartTime) / maxChargeTime);
                UpdateShieldAnimation(true);

                if (!Input.GetMouseButton(1))
                {
                    SwingShieldForward();
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
                shieldPrefab.transform.localPosition = Vector3.Lerp(
                    shieldPrefab.transform.localPosition,
                    initialPosition,
                    returnSpeed * Time.deltaTime
                );

                shieldPrefab.transform.localScale = Vector3.Lerp(
                    shieldPrefab.transform.localScale,
                    initialShieldScale,
                    returnSpeed * Time.deltaTime
                );

                if (Time.time >= cooldownEndTime)
                {
                    currentState = SwingState.Ready;
                    shieldPrefab.transform.localPosition = initialPosition;
                    shieldPrefab.transform.localScale = initialShieldScale;
                }
                break;
        }
    }

    private void UpdateShieldAnimation(bool isCharging)
    {
        float scaleMultiplier = Mathf.Lerp(minShieldScale, maxShieldScale, currentCharge);
        shieldPrefab.transform.localScale = initialShieldScale * scaleMultiplier;

        float targetOffset = isCharging ? shieldOffset : 0f;
        Vector3 targetPos = initialPosition + Vector3.forward * targetOffset * 0.4f;
        shieldPrefab.transform.localPosition = Vector3.Lerp(
            shieldPrefab.transform.localPosition,
            targetPos,
            returnSpeed * Time.deltaTime
        );
    }

    private void SwingShieldForward()
    {
        Vector3 targetPos = initialPosition + Vector3.forward * (shieldOffset * currentCharge);
        shieldPrefab.transform.localPosition = targetPos;
    }

    private float GetCurrentSpeedMultiplier()
    {
        return Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, currentCharge);
    }

    private void CheckForProjectiles(float speedMultiplier)
    {
        int projectileMask = LayerMask.GetMask("EnemyProjectile");
        int reflectCounter = 0;
        Projectile firstProjectile = null;

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

                reflectCounter++;
                firstProjectile = firstProjectile ?? projectile;

                Instantiate(reflectParticles, projectile.transform.position, Quaternion.identity);
            }
            else if (hitCollider && hitCollider.TryGetComponent<HomingOrbBehaviour>(out var homing))
            {
                homing.Initialize(_agent);
            }
        }

        if (reflectCounter > 2)
        {
            string comboText = $"Nice + {reflectCounter}!";
            CanvasManager.Instance.SpawnDamageText(_agent.transform, comboText, Color.cyan);
            Camera
                .main.GetComponent<CameraManager>()
                ?.TemporarilyTrackTarget(firstProjectile.transform);
        }
    }
}
