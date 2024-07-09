using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SaveManager : MonoBehaviour {
    private const string PlayerDataKey = "PlayerData";
    private const string SoundSettingsKey = "SoundSettings";
    private const string EnemiesDataKey = "EnemiesData";
    private const string CollectiblesDataKey = "CollectiblesData";

    public static void SavePlayerData(PlayerCharacteristics player) {
        if (player == null) {
            Debug.LogWarning("PlayerCharacteristics объект равен null. Сохранение данных игрока не выполнено.");
            return;
        }

        PlayerData data = new PlayerData {
            position = player.transform.position,
            health = player.health.currentHealth,
            coinBalance = player.coinBalance.currentBalance,
            lifes = player.respawn.Lifes,
            inventoryKeys = new List<int>(player.inventory.GetKeys()).ToArray(),
            kills = player.attack.numOfKills
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(PlayerDataKey, json);
        PlayerPrefs.Save();
    }

    public static void LoadPlayerData(PlayerCharacteristics player) {
        if (player == null) {
            Debug.LogWarning("PlayerCharacteristics объект равен null. Загрузка данных игрока не выполнена.");
            return;
        }

        if (PlayerPrefs.HasKey(PlayerDataKey)) {
            string json = PlayerPrefs.GetString(PlayerDataKey);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            player.transform.position = data.position;
            player.health.SetHealth(data.health);
            player.coinBalance.SetBalance(data.coinBalance);
            player.respawn.Lifes = data.lifes;
            player.attack.numOfKills = data.kills;
            player.inventory.SetKeys(new HashSet<int>(data.inventoryKeys));

            if (player.respawn.Lifes > 1 && data.health == 0) {
                player.health.SetHealth(player.health.StartingHealth);
                player.respawn.Lifes--;
            } else if (player.respawn.Lifes > 1 && player.health.currentHealth == 0) {
                PlayerPrefs.DeleteKey("lastLevel");
            }
        }
    }

    public static void ClearPlayerData() {
        PlayerPrefs.DeleteKey(PlayerDataKey);
    }

    public static void SaveSoundSettings() {
        SoundSettingsData data = new SoundSettingsData {
            soundVolume = PlayerPrefs.GetFloat("soundVolume", 1),
            musicVolume = PlayerPrefs.GetFloat("musicVolume", 1)
        };

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SoundSettingsKey, json);
        PlayerPrefs.Save();
    }

    public static void LoadSoundSettings(SoundManager soundManager) {
        if (soundManager == null) {
            Debug.LogWarning("SoundManager объект равен null. Загрузка данных настроек не выполнена.");
            return;
        }
        if (PlayerPrefs.HasKey(SoundSettingsKey)) {
            string json = PlayerPrefs.GetString(SoundSettingsKey);
            SoundSettingsData data = JsonUtility.FromJson<SoundSettingsData>(json);

            soundManager.SetSoundVolume(data.soundVolume);
            soundManager.SetMusicVolume(data.musicVolume);
        }
    }

    // Сохранение данных врагов
    public static void SaveEnemies(List<Enemy> enemies) {
        EnemiesData enemiesData = new EnemiesData();

        foreach (var enemy in enemies) {
            if (enemy != null) {
                EnemyData data = new EnemyData {
                    position = enemy.transform.position,
                    direction = enemy.direction,
                    health = enemy.health,
                    isAlive = enemy.isAlive,
                    canMove = enemy.canMove,
                    isHit = enemy.isHit,
                    isDestroyed = enemy.IsDestroyed()
                };
                enemiesData.enemies.Add(data);
            } else {
                enemiesData.enemies.Add(new EnemyData { isDestroyed = false });
            }
        }

        string json = JsonUtility.ToJson(enemiesData);
        PlayerPrefs.SetString(EnemiesDataKey, json);
        PlayerPrefs.Save();
    }

    // Загрузка данных врагов
    public static void LoadEnemies(List<Enemy> enemies) {
        if (PlayerPrefs.HasKey(EnemiesDataKey)) {
            string json = PlayerPrefs.GetString(EnemiesDataKey);
            EnemiesData enemiesData = JsonUtility.FromJson<EnemiesData>(json);

            for (int i = 0; i < enemiesData.enemies.Count; i++) {
                if (i < enemies.Count && !enemiesData.enemies[i].isDestroyed) {
                    var data = enemiesData.enemies[i];
                    var enemy = enemies[i];
                    enemy.transform.position = data.position;
                    enemy.direction = data.direction;
                    enemy.health = data.health;
                    enemy.isAlive = data.isAlive;
                    enemy.canMove = data.canMove;
                    enemy.isHit = data.isHit;
                } else if (i < enemies.Count && enemiesData.enemies[i].isDestroyed) {
                    Destroy(enemies[i].gameObject);
                }
            }
        }
    }

    // Сохранение данных предметов
    public static void SaveCollectibles(List<Collectible> collectibles) {
        CollectiblesData collectiblesData = new CollectiblesData();
        foreach (var collectible in collectibles) {
            if (collectible != null) {
                collectiblesData.collectibles.Add(new CollectibleData { isDestroyed = collectible.isDestroyed });
            } else {
                collectiblesData.collectibles.Add(new CollectibleData { isDestroyed = true });
            }
        }

        string json = JsonUtility.ToJson(collectiblesData);
        PlayerPrefs.SetString(CollectiblesDataKey, json);
        PlayerPrefs.Save();
    }

    // Загрузка данных предметов
    public static void LoadCollectibles(List<Collectible> collectibles) {
        if (PlayerPrefs.HasKey(CollectiblesDataKey)) {
            string json = PlayerPrefs.GetString(CollectiblesDataKey);
            CollectiblesData collectiblesData = JsonUtility.FromJson<CollectiblesData>(json);
            for (int i = 0; i < collectiblesData.collectibles.Count; i++) {
                if (i < collectibles.Count && collectiblesData.collectibles[i].isDestroyed) {
                    Destroy(collectibles[i].gameObject);
                }
            }
        }
    }
}