using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerRespawn playerRespawn;

    private void OnTriggerEnter2D(Collider2D collision) {
        playerMovement.OnTriggerEnter2D_Movement(collision);
        playerRespawn.OnTriggerEnter2D_Respawn(collision);
    }

    private void OnDrawGizmos() {
        playerAttack.OnDrawGizmos_Attack();
        playerMovement.OnDrawGizmos_Movement();
        playerRespawn.OnDrawGizmos_Respawn();
    }

    private void OnTriggerExit2D(Collider2D collision) {
        playerMovement.OnTriggerExit2D_Movement(collision);
    }
}
