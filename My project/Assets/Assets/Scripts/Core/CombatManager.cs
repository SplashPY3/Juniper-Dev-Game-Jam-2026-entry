using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class CombatManager : MonoBehaviour
{
    public int playerHP = 30;
    public int enemyHP = 20;
    public int enemyDamage = 5;
    public bool alreadySpun = false;
    public bool cardPlayed = false;

    public TMP_Text playerHealthText;
    public TMP_Text enemyHealthText;

    public GameObject victoryPanel;
    public GameObject defeatPanel;

    public SpriteRenderer wheelRenderer;

    private enum CombatState
    {
        NotStarted,
        PlayerTurn,
        EnemyTurn,
        Won,
        Lost
    }

    private string[] colors = { "Red", "Green", "Blue", "Yellow" };

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

    public CardButton[] cards;

    public enum CardColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    private CardColor spunColor;

    public void SpinWheel()
    {
        if (currentState != CombatState.PlayerTurn || alreadySpun.Equals(true))
        {
            return;
        }

        alreadySpun = true;

        spunColor = (CardColor)Random.Range(0, 4);

        Debug.Log($"Wheel landed on {spunColor}");

        UpdatePlayableCards();

    }
    private void UpdatePlayableCards()
    {
        foreach (CardButton card in cards)
        {
            wheelRenderer.color = GetColor(spunColor);
            bool playable = card.color == spunColor;
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

            Debug.Log($"Played {card.color} card for {card.damage} damage.");

            enemyHP -= card.damage;

            Debug.Log($"Enemy HP: {enemyHP}");

            if (enemyHP <= 0)
            {
                UpdateHealthUI();
                Debug.Log("Enemy defeated!");
                currentState = CombatState.Won;
                ShowVictory();
                return;
            }

            UpdateHealthUI();
        }
    }

    void EnemyAttack()
    {
        Debug.Log($"Enemy attacks for {enemyDamage} damage.");
        Debug.Log($"Player HP: {playerHP}");
        playerHP -= enemyDamage;

        if (playerHP <= 0)
        {
            UpdateHealthUI();
            Debug.Log("You died!");
            currentState = CombatState.Lost;
            ShowDefeat();
            return;
        }

        Debug.Log("Enemy turn ended.");

        UpdateHealthUI();

        StartPlayerTurn();
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

    private Color GetColor(CardColor cardColor)
    {
        switch (cardColor)
        {
            case CardColor.Red:
                return Color.red;

            case CardColor.Green:
                return Color.green;

            case CardColor.Blue:
                return Color.blue;

            case CardColor.Yellow:
                return Color.yellow;

            default:
                return Color.white;
        }
    }

}
