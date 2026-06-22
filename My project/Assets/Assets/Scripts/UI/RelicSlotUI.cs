using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject stackBadge;
    [SerializeField] private TMP_Text stackCountText;
    [SerializeField] private TMP_Text tooltipText;

    private RelicInstance boundRelic;

    public void Bind(RelicInstance instance)
    {
        boundRelic = instance;
        Refresh();
    }

    public void Refresh()
    {
        if (boundRelic == null) return;

        if (iconImage != null)
        {
            iconImage.sprite = boundRelic.data.icon;
            iconImage.enabled = boundRelic.data.icon != null;
        }

        bool showBadge = boundRelic.currentStacks > 1;

        if (stackBadge != null)
            stackBadge.SetActive(showBadge);

        if (showBadge && stackCountText != null)
            stackCountText.text = boundRelic.currentStacks.ToString();

        if (tooltipText != null)
            tooltipText.text = $"{boundRelic.data.relicName}\n{boundRelic.data.description}";
    }
}
