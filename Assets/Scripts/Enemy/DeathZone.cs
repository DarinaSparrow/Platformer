using UnityEngine;

public class DeathZone : MonoBehaviour 
{
    [SerializeField] private bool boxed = true;

    protected void OnTriggerExit2D(Collider2D collision) 
    {
        if (boxed) {
            if (collision.tag == "Player")
            {
                collision.GetComponent<Health>()?.Kill();
                collision.GetComponent<PlayerMovement>()?.StopMovement();
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!boxed) 
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<Health>()?.Kill();
                collision.GetComponent<PlayerMovement>()?.StopMovement();
            }
        }
    }
}