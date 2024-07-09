using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetup : MonoBehaviour {
    [SerializeField] private PlayerCharacteristics characteristics;
    [SerializeField] private SoundManager soundManager;

    [SerializeField] private float startingHealth;
    [SerializeField] private Transform spawnpoint;
    [SerializeField] private int startingBalance;
    [SerializeField] private int startingLifes = 3;
    [SerializeField] private int startingKills = 0;

    [SerializeField] private List<Collectible> collectibles;
    [SerializeField] private List<Enemy> enemies;

    private void Start() {
        SaveManager.LoadSoundSettings(soundManager);
        bool isNewGame = PlayerPrefs.GetInt("isNewGame", 1) == 1;
        if (isNewGame) {
            InitializeCharacter();
        } else {
            SaveManager.LoadPlayerData(characteristics);
            SaveManager.LoadCollectibles(collectibles);
            SaveManager.LoadEnemies(enemies);
        }

        PlayerPrefs.SetInt("lastLevel", SceneManager.GetActiveScene().buildIndex);
    }

    private void InitializeCharacter() {
        // ���������� ��������� ��������� ��� ���������
        characteristics.transform.position = spawnpoint.position; // ��������� �������
        characteristics.health.SetHealth(startingHealth); // ��������� ��������
        characteristics.coinBalance.SetBalance(startingBalance); // ��������� ������ �����
        characteristics.inventory.ResetKeys(); // ������ ���������
        characteristics.respawn.Lifes = startingLifes; // ��������� ���������� ������
        characteristics.attack.numOfKills = startingKills; // ��������� ���������� ������
    }

    public void SaveCollectibleItems() {
        SaveManager.SaveCollectibles(collectibles);
    }

    public void SaveEnemy() {
        SaveManager.SaveEnemies(enemies);
    }

    private void OnApplicationQuit() {
        SaveManager.SavePlayerData(characteristics);
        SaveManager.SaveEnemies(enemies);
        SaveManager.SaveSoundSettings();
    }

    private void OnDisable() {
        SaveManager.SavePlayerData(characteristics);
        SaveManager.SaveCollectibles(collectibles);
        SaveManager.SaveEnemies(enemies);
        SaveManager.SaveSoundSettings();
    }
}