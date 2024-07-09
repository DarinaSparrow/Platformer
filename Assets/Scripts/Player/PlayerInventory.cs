using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    [SerializeField] private GameObject[] keySprites;

    private HashSet<int> keys = new HashSet<int>();

    public void AddKey(int keyID) {
        keySprites[keyID].SetActive(true);
        keys.Add(keyID);
        SaveManager.SavePlayerData(GetComponent<PlayerCharacteristics>());
    }

    public bool HasKey(int keyID) {
        return keys.Contains(keyID);
    }

    public void UseKey(int keyID) {
        if (keys.Contains(keyID)) {
            keys.Remove(keyID);
            keySprites[keyID].SetActive(false);
            SaveManager.SavePlayerData(GetComponent<PlayerCharacteristics>());
        }
    }

    public int[] GetKeys() {
        return keys.ToArray();
    }

    public void SetKeys(HashSet<int> newKeys) {
        ResetKeys();
        foreach (int key in newKeys) {
            AddKey(key);
        }
    }

    public void ResetKeys() {
        keys.Clear();
        foreach (var sprite in keySprites)
            sprite.SetActive(false);
    }
}