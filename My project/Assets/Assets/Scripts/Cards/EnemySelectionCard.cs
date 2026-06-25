using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemySelectionCard : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private Image enemyImage;
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text enemyDifficultyText;
    [SerializeField] private TMP_Text enemyRewardText;
    [SerializeField] private Button selectButton;

    private void Start()
    {
        RefreshUI();
        RefreshPlayableState();
    }

    private void RefreshUI()
    {
        if (enemy == null)
        {
            SetPlayable(false);
            return;
        }

        if (enemyNameText != null)
            enemyNameText.text = enemy.enemyName;

        if (enemyDifficultyText != null)
            enemyDifficultyText.text = enemy.enemyDifficulty.ToString();

        if (enemyRewardText != null)
            enemyRewardText.text = $"Reward: {enemy.goldReward}";

        if (enemyImage != null && enemy.sprite != null)
            enemyImage.sprite = enemy.sprite;
    }

    private void RefreshPlayableState()
    {
        bool playable = enemy != null && !RunManager.Instance.HasDefeated(enemy);
        SetPlayable(playable);
    }

    public void SelectEnemy()
    {
        if (enemy == null || RunManager.Instance.HasDefeated(enemy))
        {
            return;
        }

        RunManager.Instance.SelectEnemy(enemy);
        SceneManager.LoadScene("Combat");
    }

    public void SetPlayable(bool playable)
    {
        if (selectButton != null)
            selectButton.interactable = playable && enemy != null;

        if (enemy == null)
            return;

        if (enemyImage != null)
        {
            enemyImage.color = playable
                ? Color.white
                : Color.gray;
        }

    }
}
