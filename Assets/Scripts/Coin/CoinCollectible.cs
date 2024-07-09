using UnityEngine;

public class CoinCollectible : Collectible {
    [SerializeField] private int value;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private PlayerSetup playerSetup;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            CoinBalance player = collision.GetComponent<CoinBalance>();
            if (player) {
                player.AddCoin(value);
                SoundManager.instance.PlaySound(pickupSound);
                isDestroyed = true;
                playerSetup.SaveCollectibleItems();
                Destroy(gameObject);
            }
        }
    }
}