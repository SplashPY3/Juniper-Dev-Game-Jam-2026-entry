using UnityEngine;

public class EnemySelectionManger : MonoBehaviour
{
    [SerializeField] private EnemySelectionCard[] enemyCards;
    [SerializeField] private GameObject allEnemiesDefeatedButton;

    private void Start()
    {
        RefreshAllEnemiesDefeatedButton();
    }

    private void RefreshAllEnemiesDefeatedButton()
    {
        if (allEnemiesDefeatedButton == null)
            return;

        allEnemiesDefeatedButton.SetActive(AllEnemiesDefeated());
    }

    private bool AllEnemiesDefeated()
    {
        foreach (EnemySelectionCard enemyCard in enemyCards)
        {
            if (enemyCard == null)
                continue;

            if (!enemyCard.IsDefeated())
                return false;
        }

        return true;
    }
}
