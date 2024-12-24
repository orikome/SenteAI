using System.Collections;
using UnityEngine;

public class AgentExtra : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;

    [SerializeField]
    private float flashDuration = 0.1f;

    [SerializeField]
    private Color flashColor = Color.white;

    [SerializeField]
    private AnimationCurve flashCurve;

    [SerializeField]
    private float maxFlashStrength = 1f;

    private Material material;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private bool isFlashing;

    private void Awake()
    {
        material = meshRenderer.material;
        material.EnableKeyword("_EMISSION");
    }

    public void TriggerFlash(float strength = 1f)
    {
        StartCoroutine(FlashCoroutine(strength));
    }

    private IEnumerator FlashCoroutine(float strength)
    {
        float elapsed = 0f;
        float intensity = strength * maxFlashStrength;
        Color targetFlash = flashColor * intensity;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;
            float curveValue = flashCurve.Evaluate(t);

            Color currentFlash = Color.Lerp(Color.black, targetFlash, 1 - curveValue);
            material.SetColor(EmissionColor, currentFlash);

            yield return null;
        }

        material.SetColor(EmissionColor, Color.black);
    }
}
