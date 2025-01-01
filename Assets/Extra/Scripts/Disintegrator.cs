using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disintegrator : MonoBehaviour
{
    private float disintegrationTime = 1.5f;
    private List<Material> materials = new List<Material>();
    private static readonly int DissolveThreshold = Shader.PropertyToID("_DissolveThreshold");

    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            materials.Add(mat);
            mat.SetFloat(DissolveThreshold, 0f);
        }
        StartDisintegration();
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            8f,
            Random.Range(-1f, 1f)
        ).normalized;
        rb.AddForce(randomDirection * 6, ForceMode.Impulse);
    }

    public void StartDisintegration()
    {
        StartCoroutine(DisintegrationSequence());
    }

    private IEnumerator DisintegrationSequence()
    {
        float elapsed = 0f;

        while (elapsed < disintegrationTime)
        {
            elapsed += Time.deltaTime;
            float threshold = Mathf.Clamp01(elapsed / disintegrationTime);

            foreach (Material mat in materials)
            {
                mat.SetFloat(DissolveThreshold, threshold);
            }

            yield return null;
        }

        // Ensure final state
        foreach (Material mat in materials)
        {
            mat.SetFloat(DissolveThreshold, 1f);
        }

        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
