using UnityEngine;
using UnityEngine.UI;

public class CoinBar : MonoBehaviour {
    [SerializeField] private CoinBalance playerBalance;
    [SerializeField] private Text[] currentBalance;

    private void Start() {
        foreach (var item in currentBalance) {
            item.text = playerBalance.currentBalance < 10 ? $"x {playerBalance.currentBalance} " : $"x {playerBalance.currentBalance}";
        }
    }
    private void Update() {
        foreach (var item in currentBalance) {
            item.text = playerBalance.currentBalance < 10 ? $"x {playerBalance.currentBalance} " : $"x {playerBalance.currentBalance}";
        }
    }
}