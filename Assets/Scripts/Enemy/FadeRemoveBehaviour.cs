using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRemoveBehaviour : StateMachineBehaviour
{
    [SerializeField] private float _fadeDelay; // ����� �������� ����� ��������� �������
    private float _delayElapsed = 0f; // �����, ��������� � ������ ������ �����
    private float _fadeTime = 0.75f; // �����, �� ������� ������ ��������� ��������
    private float _timeElapsed = 0f; // �����, ��������� � ������ ������������
    SpriteRenderer _spriteRenderer;  // SpriteRenderer �����
    GameObject _objectToRemove; // ��������� ������
    Color _startColor; // �������� ���� �������

    private EnemiesElimination enemiesElimination;

    private void Awake()
    {
        enemiesElimination = FindObjectOfType<EnemiesElimination>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeElapsed = 0f; // ���������� ������� �������
        _spriteRenderer = animator.GetComponent<SpriteRenderer>(); // �������� ��������� SpriteRenderer
        _startColor = _spriteRenderer.color; // ��������� �������� ���� �������
        _objectToRemove = animator.gameObject; // ��������� ������ �� ��������� ������
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_fadeDelay > _delayElapsed) _delayElapsed += Time.deltaTime; // ����������� ������� �������
        else
        {
            _timeElapsed += Time.deltaTime; // ����������� ������� �������

            float _newAlpha = _startColor.a * (1 - _timeElapsed / _fadeTime); // ��������� ����� �������� �����-������ ��� ������������ ������������
            _spriteRenderer.color = new Color(_startColor.r, _startColor.g, _startColor.b, _newAlpha); // ������������� ����� ���� �������

            if (_timeElapsed > _fadeTime)
            {
                Destroy(_objectToRemove); // ���� ����� ������������ �������, ������� ������
                enemiesElimination.EnemyKilled();
            }
        }
    }
}
