using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Mirrors ShopCardSlot but for relics
public class ShopRelicSlot : MonoBehaviour
{
    [SerializeField] private Image relicIcon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private ShopManager shopManager;

    private Relic relic;

    public Relic GetRelic() => relic;

    public void SetRelic(Relic newRelic)
    {
        relic = newRelic;

        relicIcon.sprite  = relic.icon;
        relicIcon.enabled = relic.icon != null;
        nameText.text        = relic.relicName;
        descriptionText.text = relic.description;
        priceText.text       = $"{relic.relicPrice} gold";
    }

    public void ClearRelic()
    {
        relic                = null;
        relicIcon.sprite     = null;
        relicIcon.enabled    = false;
        nameText.text        = "";
        descriptionText.text = "";
        priceText.text       = "";
    }

    public void BuyRelic()
    {
        if (relic == null) return;

        if (!PlayerManager.Instance.SpendGold(relic.relicPrice))
        {
            Debug.Log("[ShopRelicSlot] Not enough gold.");
            return;
        }

        RelicManager.Instance?.AddRelic(relic);
        gameObject.SetActive(false);
        shopManager.UpdateGoldUI();

        Debug.Log($"[ShopRelicSlot] Bought {relic.relicName}");
    }
}
