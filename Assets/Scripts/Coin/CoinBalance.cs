using UnityEngine;

public class CoinBalance : MonoBehaviour {
    public int currentBalance { get; private set; }

    public void AddCoin(int _value) {
        currentBalance += _value;
        SaveManager.SavePlayerData(GetComponent<PlayerCharacteristics>());
    }

    public void SetBalance(int balance) {
        currentBalance = balance;
    }
}