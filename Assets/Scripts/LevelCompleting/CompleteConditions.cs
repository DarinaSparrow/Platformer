using UnityEngine;

public class CompleteConditions : MonoBehaviour
{
    [SerializeField] private int enemyCount;
    [SerializeField] private int coinsCount;

    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private CoinBalance coinBalance;
    private UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (playerAttack.numOfKills >= enemyCount && coinBalance.currentBalance >= coinsCount)
            uiManager.LevelComplete(true);
    }
}
