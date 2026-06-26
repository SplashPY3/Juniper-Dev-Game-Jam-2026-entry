using UnityEngine;
using System.Collections.Generic;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public Enemy selectedEnemy;

    private readonly List<Enemy> defeatedEnemies = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartNewRun()
    {
        selectedEnemy = null;

        defeatedEnemies.Clear();
    }

    public void SelectEnemy(Enemy enemy)
    {
        selectedEnemy = enemy;
    }

    public bool HasDefeated(Enemy enemy)
    {
        return enemy != null && defeatedEnemies.Contains(enemy);
    }

    public void AddDefeated(Enemy enemy)
    {
        if (enemy == null || defeatedEnemies.Contains(enemy))
        {
            return;
        }

        defeatedEnemies.Add(enemy);
    }
}
