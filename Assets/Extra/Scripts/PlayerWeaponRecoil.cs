using System.Collections;
using UnityEngine;

public class PlayerWeaponRecoil : MonoBehaviour
{
    public Transform handMesh;
    private float recoilDistance = 0.5f;

    [SerializeField]
    private float recoilSpeed = 10f;

    [SerializeField]
    private float returnSpeed = 5f;

    [SerializeField]
    private float fireRate = 0.1f;

    [SerializeField]
    private float recoilRecoveryMultiplier = 1.5f;

    [SerializeField]
    private AnimationCurve recoilCurve;

    [SerializeField]
    private float maxRecoil = 0.5f;

    [SerializeField]
    private float recoilBuild = 0.2f; // How much each shot adds to recoil

    private float currentRecoil = 0f;
    private Vector3 originalLocalPosition;
    private bool isRecoiling = false;

    public void TriggerRecoil()
    {
        if (!isRecoiling)
        {
            //GameObject obj = Instantiate(flashPrefab, firepoint.position, firepoint.rotation);
            //obj.transform.SetParent(firepoint);
            //AudioManager.Instance.PlayAudioFile(splats[Random.Range(0, splats.Length)]);
            StartCoroutine(RecoilCoroutine());
        }
    }

    private IEnumerator RecoilCoroutine()
    {
        isRecoiling = true;
        originalLocalPosition = handMesh.transform.localPosition;

        // Add to current recoil, clamped to max
        currentRecoil = Mathf.Min(currentRecoil + recoilBuild, maxRecoil);
        Vector3 recoilTarget = originalLocalPosition - Vector3.forward * currentRecoil;

        // Recoil phase
        float elapsed = 0f;
        // Use half the fire rate for recoil
        while (elapsed < fireRate * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (fireRate * 0.5f);
            float curveValue = recoilCurve.Evaluate(t);
            handMesh.transform.localPosition = Vector3.Lerp(
                originalLocalPosition,
                recoilTarget,
                curveValue
            );
            yield return null;
        }

        // Recovery phase
        elapsed = 0f;
        while (elapsed < fireRate * recoilRecoveryMultiplier)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (fireRate * recoilRecoveryMultiplier);
            float curveValue = recoilCurve.Evaluate(t);
            handMesh.transform.localPosition = Vector3.Lerp(
                recoilTarget,
                originalLocalPosition,
                curveValue
            );
            yield return null;
        }

        // Gradual recoil recovery
        currentRecoil = Mathf.Max(0, currentRecoil - recoilBuild * 0.5f);
        handMesh.transform.localPosition = originalLocalPosition;
        isRecoiling = false;
    }
}
