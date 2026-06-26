using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Card card;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardDescriptionText;

    [SerializeField] private GameObject playCardButton;

    private Button button;
    private HandSelectionManager selectionManager;
    private bool isSelected;

    public Card Data => card;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
            button.transition = Selectable.Transition.None;

    }

    private void Start()
    {
        SetSelected(false);
        SetCardInfo();
        ShowCardInfo(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowCardInfo(card != null && !isSelected);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowCardInfo(false);
    }

    private void ShowCardInfo(bool show)
    {
        if (cardNameText != null) // check if card is selected
            cardNameText.gameObject.SetActive(show);

        if (cardDescriptionText != null)
            cardDescriptionText.gameObject.SetActive(show);
    }

    private void SetCardInfo()
    {
        if (cardNameText != null)
            cardNameText.text = card != null ? card.cardName : "";

        if (cardDescriptionText != null)
            cardDescriptionText.text = card != null ? card.description : "";
    }

    public void SetPlayable(bool playable)
    {
        if (button != null)
            button.interactable = playable && card != null;

        if (card == null)
            return;

        if (cardImage != null)
        {
            cardImage.color = playable
                ? Color.white
                : Color.gray;
        }
    }

    public void SetCard(Card newCard)
    {
        card = newCard;

        if (card == null )
        {
            if (cardImage != null)
                cardImage.enabled = false;

            if (button != null)
                button.interactable = false;

            SetCardInfo();
            return;
        }

        if (cardImage != null)
        {
            cardImage.enabled = true;
            cardImage.sprite = card.sprite;
        }

        SetCardInfo();

        ShowNeutral();
    }

    public void ShowNeutral()
    {
        if (button != null)
            button.interactable = false;

        if (card != null && cardImage != null)
        {
            cardImage.color = Color.white;
        }
    }

    public void ClearCard()
    {
        SetCard(null);
    }

    public void SetSelectionManager(HandSelectionManager manager)
    {
        selectionManager = manager;
    }

    public void SelectCard()
    {
        if (card != null)
        {
            selectionManager?.SelectCard(this);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (isSelected)
            ShowCardInfo(false);

        if (playCardButton != null)
            playCardButton.SetActive(selected && card != null);
    }

    public void PlayCard()
    {
        combatManager?.PlayCard(this);
    }
}
