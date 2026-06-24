using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopCardSlot : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private ShopManager shopManager;

    private Card card;

    public Card GetCard()
    {
        Card cardParam = card;
        return cardParam;
    }

    public void SetCard(Card newCard)
    {
        card = newCard;
        cardImage.sprite = card.sprite;
        cardImage.enabled = true;
        priceText.text = $"{card.cardPrice} gold";

    }

    public void ClearCard(Card oldCard)
    {
        card = oldCard;
        cardImage.sprite = null;
        cardImage.enabled = false;
        priceText.text = "";
    }

    public void BuyCard()
    {
        if (!PlayerManager.Instance.SpendGold(card.cardPrice))
        {
            Debug.Log("Not enough gold.");
            return;
        }

        PlayerManager.Instance.AddCardToDeck(card);

        gameObject.SetActive(false);

        Debug.Log($"Bought {card.cardName}");

        shopManager.UpdateGoldUI();
    }
}
