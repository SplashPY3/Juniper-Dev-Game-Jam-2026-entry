using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Image cardImage;

    private Button button;

    public Card Data => card;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.transition = Selectable.Transition.None;
    }

    public void SetPlayable(bool playable)
    {
        button.interactable = playable && card != null;

        if (card == null)
            return;

        Color originalColor = Card.GetDisplayColor(card.color);

        cardImage.color = playable
            ? originalColor
            : Color.Lerp(originalColor, Color.gray, 0.65f);
    }

    public void SetCard(Card newCard)
    {
        card = newCard;

        if (card == null )
        {
            cardImage.enabled = false;
            button.interactable = false;
            return;
        }

        cardImage.enabled = true;
        cardImage.sprite = card.sprite;

        ShowNeutral();
    }

    public void ShowNeutral()
    {
        button.interactable = false;

        if (card != null)
        {
            cardImage.color = Card.GetDisplayColor(card.color);
        }
    }

    public void ClearCard()
    {
        SetCard(null);
    }

    public void PlayCard()
    {
        if (combatManager.PlayCard(this))
            deckManager.DiscardFromHand(this);
    }
}
