using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    public enum ActionSelectionMode
    {
        Sequential,
        Random,
        Reactive
    }

    public enum EnemyDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public string enemyName;
    public Sprite sprite;
    public int maxHP;
    public int goldReward;
    public ActionSelectionMode actionSelectionMode;
    public EnemyDifficulty enemyDifficulty;
    public List<EnemyAction> Actions = new();
}
