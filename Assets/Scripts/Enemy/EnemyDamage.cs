using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] protected float damage;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        // ���� ����� ����� � ���� ����������� �����, ������� ��� ����
        if ((collision.CompareTag("Player")) && (!collision.GetComponent<Health>().IsDead()))
            collision.GetComponent<Health>()?.TakeDamage(damage);
    }

    protected void OnTriggerStay2D(Collider2D collision) {
        // ���� ����� ����� � ���� ����������� �����, ������� ��� ����
        if ((collision.CompareTag("Player")) && (!collision.GetComponent<Health>().IsDead()))
            collision.GetComponent<Health>()?.TakeDamage(damage);
    }
}