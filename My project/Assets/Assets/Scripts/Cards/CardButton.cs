using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private CombatManager combatManager;

    private Button button;

    public Card Data => card;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetPlayable(bool playable)
    {
        button.interactable = playable;
    }

    public void PlayCard()
    {
        combatManager.PlayCard(this);
    }
}
