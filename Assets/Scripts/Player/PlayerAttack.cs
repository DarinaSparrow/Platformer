using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour {

    [Header("����� ���������")]
    [SerializeField] private LayerMask enemyLayer; // ���� ���������� ����� 
    [SerializeField] private CapsuleCollider2D capsuleCollider;
    [SerializeField] private Text textKills;

    [Header("��������� ������� �����")]
    [SerializeField] private float attackCooldown; // ����� ����������� �����
    [SerializeField] private AudioClip attackSound; // ���� �����
    [SerializeField] private float distanceAttack;
    [SerializeField] private float rangeAttack;
    [SerializeField] private int damageAttack;

    [Header("������ ������� �����")]
    [SerializeField] private Effects BladeEffect;

    [Header("��������� ������� �����")]
    [SerializeField] private float longRangeAttackCooldown; // ����� ����������� �����
    [SerializeField] private AudioClip longRangeAttackSound; // ���� �����
    [SerializeField] private float distanceLongRangeAttack;
    [SerializeField] private float xRangeLongRangeAttack;
    [SerializeField] private float yRangeLongRangeAttack;
    [SerializeField] private int damageLongRangeAttack;
    [SerializeField] private float heightLongRangeAttack;

    [Header("������ ������� �����")]
    [SerializeField] private Effects SplashEffect;

    private Animator anim; // ��������
    private PlayerMovement playerMovement; // ������ �������� ������
    private float cooldownTimerAttack = Mathf.Infinity; // ������ ����������� ������� �����
    private float cooldownTimerLongRangeAttack = Mathf.Infinity; // ������ ����������� ������� �����
    private EnemyHealth enemyHealth;
    public int numOfKills = 0;

    private void Awake() {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start() {
        textKills.text = numOfKills < 10 ? $"x {numOfKills} " : $"x {numOfKills}";
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverSpecificUI() && cooldownTimerAttack > attackCooldown && playerMovement.canAttack()) {
            playerMovement.StopMovementForAttack(); // ������������� ��������
            Attack();
        }

        if (Input.GetMouseButtonDown(1) && !IsPointerOverSpecificUI() && cooldownTimerLongRangeAttack > attackCooldown && playerMovement.canAttack()) {
            playerMovement.StopMovementForAttack(); // ������������� ��������
            LongRangeAttack();
        }

        cooldownTimerAttack += Time.deltaTime;
        cooldownTimerLongRangeAttack += Time.deltaTime;

        textKills.text = numOfKills < 10 ? $"x {numOfKills} " : $"x {numOfKills}";
    }

    private void Attack() {
        playerMovement.ResetCollider();
        SoundManager.instance.PlaySound(attackSound);
        anim.SetBool("grounded", true);
        anim.SetTrigger("attack");
        cooldownTimerAttack = 0;
    }

    public void AttackHit() {
        BladeEffect.ShowEffect();
        if (PlayerInSightForAttack()) {
            bool isDead = enemyHealth.Hit(damageAttack);
            if (isDead) numOfKills++;
        }
        cooldownTimerAttack = 0;
    }

    private void LongRangeAttack() {
        playerMovement.ResetCollider();
        SoundManager.instance.PlaySound(attackSound);
        anim.SetBool("grounded", true);
        anim.SetTrigger("long-range attack");
        cooldownTimerLongRangeAttack = 0;
    }

    public void LongRangeAttackHit() {
        SplashEffect.ShowEffect();
        if (PlayerInSightForLongRangeAttack()) {
            bool isDead = enemyHealth.Hit(damageLongRangeAttack);
            if (isDead) numOfKills++;
        }
        cooldownTimerLongRangeAttack = 0;
    }

    private bool PlayerInSightForAttack() {
        RaycastHit2D hit = Physics2D.BoxCast(capsuleCollider.bounds.center + transform.right * distanceAttack * Mathf.Sign(transform.localScale.x),
            new Vector3(capsuleCollider.bounds.size.x * rangeAttack, capsuleCollider.bounds.size.y, capsuleCollider.bounds.size.z), 0, Vector2.left, 0, enemyLayer);

        if (hit.collider != null)
            enemyHealth = hit.transform.GetComponent<EnemyHealth>();
        return hit.collider != null;
    }

    private bool PlayerInSightForLongRangeAttack() {
        RaycastHit2D hit = Physics2D.BoxCast(capsuleCollider.bounds.center + transform.right * distanceLongRangeAttack * Mathf.Sign(transform.localScale.x) + transform.up * yRangeLongRangeAttack,
            new Vector3(capsuleCollider.bounds.size.x * xRangeLongRangeAttack, capsuleCollider.bounds.size.y * heightLongRangeAttack, capsuleCollider.bounds.size.z), 0, Vector2.left, 0, enemyLayer);

        if (hit.collider != null)
            enemyHealth = hit.transform.GetComponent<EnemyHealth>();
        return hit.collider != null;
    }

    public void EndAttack() {
        playerMovement.ResumeMovementAfterAttack(); // ��������������� ��������
    }

    private bool IsPointerOverSpecificUI() {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results) {
            if (result.gameObject.CompareTag("UIButton")) // �������� �� ��� ���
            {
                return true;
            }
        }
        return false;
    }

    public void OnDrawGizmos_Attack() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(capsuleCollider.bounds.center + transform.right * distanceAttack * Mathf.Sign(transform.localScale.x),
            new Vector3(capsuleCollider.bounds.size.x * rangeAttack, capsuleCollider.bounds.size.y, capsuleCollider.bounds.size.z));

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(capsuleCollider.bounds.center + transform.right * distanceLongRangeAttack * Mathf.Sign(transform.localScale.x) + transform.up * yRangeLongRangeAttack,
            new Vector3(capsuleCollider.bounds.size.x * xRangeLongRangeAttack, capsuleCollider.bounds.size.y * heightLongRangeAttack, capsuleCollider.bounds.size.z));
    }
}