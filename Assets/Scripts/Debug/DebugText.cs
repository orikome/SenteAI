using System.Collections;
using TMPro;
using UnityEngine;

public class DebugText : MonoBehaviour
{
    [SerializeField]
    float lifetime = 2f;

    [SerializeField]
    float speed = 1f;
    float elapsed;
    TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeAndDeactivate());
    }

    private void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector3.up);
    }

    private IEnumerator FadeAndDeactivate()
    {
        float halfLifetime = lifetime / 2f;
        yield return new WaitForSeconds(halfLifetime);

        float elapsed = 0f;
        Color originalColor = text.color;
        while (elapsed < halfLifetime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsed / halfLifetime);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }

    public void SetText(string message, Color color)
    {
        text.text = message;
        text.color = color;
        transform.localScale = Vector3.one;
    }
}
