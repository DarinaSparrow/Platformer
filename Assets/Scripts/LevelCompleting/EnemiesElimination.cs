using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesElimination : MonoBehaviour
{
    [SerializeField] private int enemyCount;

    private int remainedEnemies;
    private UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        remainedEnemies = enemyCount;
    }

    public void EnemyKilled()
    {
        remainedEnemies--;
        if (remainedEnemies <= 0)
            uiManager.LevelComplete(true);
    }
}
