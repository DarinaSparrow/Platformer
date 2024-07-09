using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private Collider2D _searchingCollider; // Collider �����
    public List<Collider2D> _detectedColliders = new List<Collider2D>(); // ������ �����������, ����������� � ���� �����������

    private bool _hasTarget = false; // ����, ������������, ��������� �� ����� � ���� ����������� �����

    Animator _animator; // �������� �����
    private void Awake()
    {
        _searchingCollider = GetComponent<Collider2D>();
        _animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        _hasTarget = CheckIfTarget(); // ���������, ��������� �� ����� � ���� �����������

        _animator.SetBool("hasTarget", _hasTarget); // ��������� �������� ��������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Player")) && (!collision.GetComponent<Health>().IsDead())) _detectedColliders.Add(collision); // ��������� ������������ ��������� � ������
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _detectedColliders.Remove(collision); // ������� ������������ ��������� �� ������
    }

    private bool CheckIfTarget()
    {
       return _detectedColliders.Count > 0; // ��������� ������� ������ � ���� �����������
    }
}
