using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Collider2D _searchingCollider; // Collider врага
    public List<Collider2D> _detectedColliders = new List<Collider2D>(); // —писок коллайдеров, наход€щихс€ в зоне обнаружени€

    private bool _hasTarget = false; // ‘лаг, показывающий, находитс€ ли игрок в зоне обнаружени€ врага

    Animator _animator; // јниматор врага
    private void Awake()
    {
        _searchingCollider = GetComponent<Collider2D>();
        _animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        _hasTarget = CheckIfTarget(); // ѕровер€ем, находитс€ ли игрок в зоне обнаружени€

        _animator.SetBool("hasTarget", _hasTarget); // ќбновл€ем параметр анимации
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player")) && (!collision.GetComponent<Health>().IsDead())) _detectedColliders.Add(collision); // ƒобавл€ем обнаруженный коллайдер в список
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedColliders.Remove(collision); // ”дал€ем обнаруженный коллайдер из списка
    }

    private bool CheckIfTarget()
    {
       return _detectedColliders.Count > 0; // ѕровер€ем наличие игрока в зоне обнаружени€
    }
}
