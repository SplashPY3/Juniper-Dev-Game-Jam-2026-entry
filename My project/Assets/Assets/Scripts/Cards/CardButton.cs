using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private Image cardImage;

    [SerializeField] private GameObject playCardButton;

    private Button button;
    private HandSelectionManager selectionManager;

    public Card Data => card;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.transition = Selectable.Transition.None;
    }

    private void Start()
    {
        SetSelected(false);
    }

    public void SetPlayable(bool playable)
    {
        button.interactable = playable && card != null;

        if (card == null)
            return;

        cardImage.color = playable
            ? Color.white
            : Color.gray;
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
        playCardButton.SetActive(selected && card != null);
    }

    public void PlayCard()
    {
        combatManager.PlayCard(this);
    }
}
