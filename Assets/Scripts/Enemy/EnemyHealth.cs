using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth; // ������������ �������� ��������
    private int _health; // ������� ������� ��������

    private bool _isHit = false; // ����, ������������, ������ �� ����� ����
    private bool _isAlive = true; // ����, ������������, ��� �� ����

    private bool _isInvincible = false; // ����, ������������, �������� �� ����
    private float _timeSinceHit = 0; // �����, ��������� � ������� ��������� �����
    [SerializeField] private float _invincibilityTimer; // ����� ������������ ����� ��������� �����

    Animator _animator; // �������� �����

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _health = _maxHealth;
    }

    private void Update()
    {
       if (_isInvincible)
       {
            if (_timeSinceHit > _invincibilityTimer)
            {
                _isInvincible = false; // ���������� ������������
                _isHit = false; // ���������� ���� ��������� �����
                _timeSinceHit = 0; // ���������� ������
            }

            _timeSinceHit += Time.deltaTime; // ��������� ������, ����������� ����� ���������� ��������� �����
        }

        // ��������� ��������� ��������
        _animator.SetBool("isHit", _isHit);
        _animator.SetBool("isAlive", _isAlive);
    }

    public bool Hit(int damage)
    {
        if (_isAlive && !_isInvincible)
        {
            _isHit = true; // ������������� ���� ��������� �����

            _health -= damage; // ��������� �������� �����
            _isInvincible = true; // ������ ����� �������� ����������

            if (_health <= 0) {
                _isAlive = false; // ���� � ����� ����������� ��������, ��������� ��� ������
                return true;
            } 
        }
        return false;
    }

    public EnemyData GetEnemyData() {
        return new EnemyData {
            position = transform.position,
            health = _health,
            isAlive = _isAlive,
            isHit = _isHit
        };
    }

    public void LoadEnemyData(EnemyData data) {
        transform.position = data.position;
        _health = data.health;
        _isAlive = data.isAlive;
        _isHit = data.isHit;

        // ��������� �������� � ���������
        _animator.SetBool("isAlive", _isAlive);
        _animator.SetBool("isHit", _isHit);
    }
}
