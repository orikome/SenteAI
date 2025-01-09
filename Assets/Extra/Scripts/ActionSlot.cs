using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI text;
    public Image cooldownOverlay;
    private AgentAction _action;

    public void Initialize(AgentAction action, string slotText)
    {
        _action = action;
        text.text = slotText;
        if (action.icon != null)
            icon.sprite = action.icon;
    }

    private void Update()
    {
        if (_action != null)
        {
            cooldownOverlay.fillAmount = 1 - _action.GetCooldownProgress();
        }
    }
}
