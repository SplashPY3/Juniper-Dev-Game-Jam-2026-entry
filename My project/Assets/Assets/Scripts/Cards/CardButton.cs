using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    public CombatManager.CardColor color;

    public int damage = 5;

    public CombatManager combatManager;

    private Button button;

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
