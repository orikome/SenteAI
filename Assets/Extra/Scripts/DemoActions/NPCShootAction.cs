using System;
using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(menuName = "SenteAI/Actions/NPCShootAction")]
    public class NPCShootAction : ShootAction, IFeedbackAction
    {
        // Feedback interface
        public Action OnSuccessCallback { get; set; }
        public Action OnFailureCallback { get; set; }
        public int SuccessCount { get; set; } = 0;
        public int FailureCount { get; set; } = 0;
        public float SuccessRate { get; set; } = 1.0f;
        public float FeedbackModifier { get; set; } = 1.0f;

        public override void Execute(Transform firePoint, Vector3 direction)
        {
            ShootProjectile(firePoint, _agent.Metrics.GetDirectionToTargetPredictedPosition());
            AfterExecution();
        }

        public override bool CanExecute()
        {
            return base.CanExecute() || !_agent.GetModule<SeeingModule>().HasLOS;
        }

        public override void CalculateUtility()
        {
            float utility = new UtilityBuilder()
                .WithDistance(_agent.Metrics.DistanceToTarget, 60f, UtilityType.Linear)
                .WithLOS(_agent.GetModule<SeeingModule>().HasLOS)
                .WithProjectileStats(projectileSpeed)
                .Build();

            SetUtilityWithModifiers(utility);
        }

        public float ApplyFeedbackModifier(float utility, IFeedbackAction feedbackAction)
        {
            if (SuccessRate >= 0.5f)
                // Success rate is good, boost utility
                FeedbackModifier = Mathf.Lerp(1.0f, 1.5f, SuccessRate);
            else
                // Success rate is low, add penalty
                FeedbackModifier = Mathf.Lerp(0.5f, 1.0f, SuccessRate);

            return Mathf.Max(FeedbackModifier, MIN_UTILITY);
        }

        public void HandleFailure(Agent agent)
        {
            if (agent == null)
                return;

            // Decrease utility if projectile misses
            FailureCount++;
            OnFailureCallback?.Invoke();
            UpdateSuccessRate();
            int totalAttempts = SuccessCount + FailureCount;
            AgentLogger.Log(
                $"Miss: {Helpers.Bold(Helpers.CleanName(name))}. Attempts: {totalAttempts}. SuccessRate: {SuccessRate}, FeedbackModifier: {FeedbackModifier}.",
                _agent.gameObject
            );
        }

        public void HandleSuccess(Agent agent)
        {
            if (agent == null)
                return;

            // Increase utility if projectile hits
            SuccessCount++;
            OnSuccessCallback?.Invoke();
            UpdateSuccessRate();
            int totalAttempts = SuccessCount + FailureCount;
            AgentLogger.Log(
                $"Hit: {Helpers.Bold(Helpers.CleanName(name))}. Attempts: {totalAttempts}. SuccessRate: {SuccessRate}, FeedbackModifier: {FeedbackModifier}.",
                _agent.gameObject
            );
        }

        public void UpdateSuccessRate()
        {
            int totalAttempts = SuccessCount + FailureCount;
            if (totalAttempts > 0)
            {
                SuccessRate = (float)SuccessCount / totalAttempts;
            }
        }

        protected override void ShootProjectile(Transform firePoint, Vector3 direction)
        {
            GameObject projectile = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.LookRotation(direction)
            );

            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            projectileComponent.SetParameters(_agent, direction, projectileSpeed, damage);

            if (projectileComponent != null)
            {
                projectileComponent.OnHitCallback = () => HandleSuccess(_agent);
                projectileComponent.OnMissCallback = () => HandleFailure(_agent);
            }
            Destroy(projectile, 4f);
        }
    }
}
