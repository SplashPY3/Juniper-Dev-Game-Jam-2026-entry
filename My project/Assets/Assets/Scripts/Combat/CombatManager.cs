using UnityEngine;
using TMPro;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private int playerHP = 30;
    [SerializeField] private int enemyHP = 20;
    [SerializeField] private int enemyDamage = 5;
    [SerializeField] private bool alreadySpun = false;
    [SerializeField] private bool cardPlayed = false;

    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private TMP_Text enemyHealthText;

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    [SerializeField] private SpriteRenderer wheelRenderer;

    private enum CombatState
    {
        NotStarted,
        PlayerTurn,
        EnemyTurn,
        Won,
        Lost
    }

    private CombatState currentState = CombatState.NotStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);

        EnterCombat();
        UpdateHealthUI();
    }

    void EnterCombat()
    {
        Debug.Log("Entered combat");
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        currentState = CombatState.PlayerTurn;
        Debug.Log("Player turn started");
    }

    void StartEnemyTurn()
    {
        currentState = CombatState.EnemyTurn;
        Debug.Log("Enemy turn started");

        Invoke(nameof(EnemyAttack), 1f);
    }

    // ------------------------------------------------------------

    [SerializeField] private CardButton[] cards;

    private Card.CardColor spunColor;

    public void SpinWheel()
    {
        if (currentState != CombatState.PlayerTurn || alreadySpun.Equals(true))
        {
            return;
        }

        alreadySpun = true;

        spunColor = (Card.CardColor)Random.Range(0, 4);

        Debug.Log($"Wheel landed on {spunColor}");

        UpdatePlayableCards();

    }
    private void UpdatePlayableCards()
    {
        wheelRenderer.color = GetColor(spunColor);

        foreach (CardButton card in cards)
        {
            bool playable = card.Data.color == spunColor;
            card.SetPlayable(playable);
        }
    }

    void ResetPlayableCards()
    {
        foreach (CardButton card in cards)
        {
            bool playable = true;

            card.SetPlayable(playable);
        }
    }

    public void PlayCard(CardButton card)
    {
        if (cardPlayed.Equals(false))
        {
            cardPlayed = true;

            if (currentState != CombatState.PlayerTurn)
                return;

            Debug.Log($"Played {card.Data.cardName} for {card.Data.damage} damage.");

            if (DamageEnemy(card.Data.damage))
            {
                return;
            }
        }
    }

    void EnemyAttack()
    {
        Debug.Log($"Enemy attacks for {enemyDamage} damage.");

        if (DamagePlayer(enemyDamage))
        {
            return;
        }

        Debug.Log("Enemy turn ended.");

        StartPlayerTurn();
    }

    private bool DamageEnemy(int damage)
    {
        enemyHP = Mathf.Max(0, enemyHP - damage);
        Debug.Log($"Enemy HP: {enemyHP}");
        UpdateHealthUI();

        if (enemyHP > 0)
        {
            return false;
        }

        WinCombat();
        return true;
    }

    private bool DamagePlayer(int damage)
    {
        playerHP = Mathf.Max(0, playerHP - damage);
        Debug.Log($"Player HP: {playerHP}");
        UpdateHealthUI();

        if (playerHP > 0)
        {
            return false;
        }

        LoseCombat();
        return true;
    }

    private void WinCombat()
    {
        Debug.Log("Enemy defeated!");
        currentState = CombatState.Won;
        ShowVictory();
    }

    private void LoseCombat()
    {
        Debug.Log("You died!");
        currentState = CombatState.Lost;
        ShowDefeat();
    }

    public void EndPlayerTurn()
    {
        if (currentState != CombatState.Won)
        {
            Debug.Log("Player turn ended.");
            ResetPlayableCards();
            StartEnemyTurn();
            alreadySpun = false;
            cardPlayed = false;
        }
    }

    private void UpdateHealthUI()
    {
        playerHealthText.text = playerHP.ToString();
        enemyHealthText.text = enemyHP.ToString();
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

    void ShowDefeat()
    {
        defeatPanel.SetActive(true);
    }

    private Color GetColor(Card.CardColor cardColor)
    {
        switch (cardColor)
        {
            case Card.CardColor.Red:
                return Color.red;

            case Card.CardColor.Green:
                return Color.green;

            case Card.CardColor.Blue:
                return Color.blue;

            case Card.CardColor.Yellow:
                return Color.yellow;

            default:
                return Color.white;
        }
    }

}
