using UnityEngine;

public class Key : Collectible  {
    [SerializeField] private int keyID; // ”никальный идентификатор ключа
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private PlayerSetup playerSetup;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            PlayerInventory player = collision.GetComponent<PlayerInventory>();
            if (player) {
                player.AddKey(keyID);
                SoundManager.instance.PlaySound(pickupSound);
                isDestroyed = true;
                playerSetup.SaveCollectibleItems();
                Destroy(gameObject);
            }
        }
    }
}