using System.Collections;
using TMPro;
using UnityEngine;

namespace SenteAI.Extra
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField]
        float lifetime = 2f;

        [SerializeField]
        float speed = 1f;

        [SerializeField]
        AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        float maxScale = 1.5f;

        TextMeshPro text;

        private void Awake()
        {
            text = GetComponent<TextMeshPro>();
        }

        private void OnEnable()
        {
            StartCoroutine(AnimateAndDeactivate());
            transform.SetParent(null);
        }

        private void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
        }

        private IEnumerator AnimateAndDeactivate()
        {
            float elapsed = 0f;
            Color originalColor = text.color;
            Vector3 baseScale = Vector3.one;

            while (elapsed < lifetime)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / lifetime;

                // Scale animation
                float scaleMultiplier = 1f + (maxScale - 1f) * scaleCurve.Evaluate(normalizedTime);
                transform.localScale = baseScale * scaleMultiplier;

                // Fade animation
                float alpha =
                    normalizedTime <= 0.5f ? 1f : Mathf.Lerp(1f, 0f, (normalizedTime - 0.5f) * 2f);
                text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                yield return null;
            }

            Destroy(gameObject);
        }

        public void SetText(string message, Color color)
        {
            text.text = message;
            text.color = color;
            transform.localScale = Vector3.zero;
        }
    }
}
