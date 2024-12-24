using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    public TextMeshProUGUI subtitleText;
    public GameObject damageRedBackground;
    public float damageFlashIntensity = 0.5f;
    private Coroutine currentDamageFlash;
    private float currentDamageAlpha = 0f;
    public GameObject debugTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamageFlash()
    {
        if (currentDamageFlash != null)
        {
            StopCoroutine(currentDamageFlash);
        }
        currentDamageFlash = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        // Enable the damage background if it's not already active
        damageRedBackground.SetActive(true);
        Image damageImage = damageRedBackground.GetComponent<Image>();

        // Instantly set to max alpha
        Color c = damageImage.color;
        currentDamageAlpha = Mathf.Max(currentDamageAlpha, damageFlashIntensity);
        damageImage.color = new Color(c.r, c.g, c.b, currentDamageAlpha);

        // Fade out
        float fadeSpeed = 2f;
        while (currentDamageAlpha > 0)
        {
            currentDamageAlpha = Mathf.Max(0, currentDamageAlpha - (fadeSpeed * Time.deltaTime));
            damageImage.color = new Color(c.r, c.g, c.b, currentDamageAlpha);
            yield return null;
        }

        damageRedBackground.SetActive(false);
        currentDamageFlash = null;
    }

    void Start()
    {
        StartCoroutine(ShowIntro());
    }

    public enum TextPosition
    {
        Center,
        CenterTop,
        CenterBottom,
        CenterLeft,
        CenterRight,
    }

    private Vector2 CalculateTextPosition(TextPosition position, RectTransform textRect)
    {
        Vector2 screenSize = new(Screen.width, Screen.height);
        Vector2 textSize = textRect.sizeDelta;
        Vector2 pos = Vector2.zero;

        // Adjustable padding from screen edges
        float padding = 20f;

        switch (position)
        {
            case TextPosition.Center:
                pos = screenSize / 2;
                break;
            case TextPosition.CenterTop:
                pos = new Vector2(screenSize.x / 2, screenSize.y - padding - textSize.y / 2);
                break;
            case TextPosition.CenterBottom:
                pos = new Vector2(screenSize.x / 2, padding + textSize.y / 2);
                break;
            case TextPosition.CenterLeft:
                pos = new Vector2(padding + textSize.x / 2, screenSize.y / 2);
                break;
            case TextPosition.CenterRight:
                pos = new Vector2(screenSize.x - padding - textSize.x / 2, screenSize.y / 2);
                break;
        }

        return pos;
    }

    public IEnumerator ShowText(
        TextMeshProUGUI textComponent,
        string message,
        TextPosition position = TextPosition.Center,
        float fadeInTime = 0.5f,
        float displayTime = 2f,
        float fadeOutTime = 0.5f
    )
    {
        // Setup
        textComponent.text = message;
        textComponent.gameObject.SetActive(true);

        // Position the text
        RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
        Vector2 newPosition = CalculateTextPosition(position, rectTransform);
        rectTransform.position = newPosition;

        // Fade in
        yield return StartCoroutine(
            OrikomeUtils.TransitionUtils.FadeTransition(
                textComponent.transform,
                fadeInTime,
                0.0f,
                1.0f
            )
        );

        // Display duration
        yield return new WaitForSeconds(displayTime);

        // Fade out
        yield return StartCoroutine(
            OrikomeUtils.TransitionUtils.FadeTransition(
                textComponent.transform,
                fadeOutTime,
                1.0f,
                0.0f
            )
        );

        // Cleanup
        textComponent.gameObject.SetActive(false);
    }

    public IEnumerator ShowIntro()
    {
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(
            ShowText(subtitleText, "Hello Player!", TextPosition.CenterBottom)
        );
        yield return new WaitForSeconds(2.0f);
        subtitleText.gameObject.SetActive(false);
    }

    public void SpawnTextLog(Transform agentTransform, string message, Color color)
    {
        Vector3 position = OrikomeUtils.GeneralUtils.GetPositionWithOffset(
            agentTransform,
            Random.Range(-4.0f, 4.0f),
            Random.Range(4.0f, 4.0f),
            Random.Range(-4.0f, 4.0f)
        );

        GameObject debugTextObj = Instantiate(
            debugTextPrefab,
            position,
            Quaternion.identity,
            agentTransform
        );

        DamageText debugText = debugTextObj.GetComponent<DamageText>();
        debugText.SetText(message, color);
    }
}
