using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private CapsuleCollider2D capsuleCollider;

    [Header("Параметры движения")]
    [SerializeField] private float speed; // Скорость
    [SerializeField] private float jumpPower; // Сила прыжка
    [SerializeField] private float rollSpeed; // Скорость перекатывания
    [SerializeField] private float rollDuration; // Длительность перекатывания
    [SerializeField] private float timeToRoll; // Длительность перекатывания
    [SerializeField] private float checkRangeToGround; // Размер луча до земли
    [SerializeField] private float checkRangeToDeath; // Размер луча до запретной зоны
    [SerializeField] private float lowGravityScale; // Гравитация установленная в инспекторе


    [Header("Эффект бега")]
    [SerializeField] private Effects dustEffect;

    [Header("Эффект приземления")]
    [SerializeField] private Effects fallEffect;

    [Header("Эффект перекатывания")]
    [SerializeField] private Effects rollEffect;


    [Header("Время койота")]
    [SerializeField] private float coyoteTime; // Время, в течение которого игрок может висеть в воздухе перед прыжком
    private float coyoteCounter; // Время, прошедшее с момента, когда игрок сошел с края

    [Header("Множественные прыжки")]
    [SerializeField] private int extraJumps; // Дополнительные прыжки
    private int jumpCounter; // Счетчик прыжков

    [Header("Прыжки от стен")]
    [SerializeField] private float wallJumpX; // Горизонтальная сила прыжка от стены
    [SerializeField] private float wallJumpY; // Вертикальная сила прыжка от стены

    [Header("Слои")]
    [SerializeField] private LayerMask groundLayer; // Слой земли
    [SerializeField] private LayerMask wallLayer; // Слой стен
    [SerializeField] private LayerMask deathLayer; // Слой стен

    [Header("Звуки")]
    [SerializeField] private AudioClip jumpSound; // Звук прыжка

    [Header("Звуки")]
    [SerializeField] private AudioClip rollSound; // Звук перекатывания

    private float additionalSpeed;
    private float additionalDurationSpeed;
    private bool isAdditionalSpeed;
    private float currentAdditionalDurationSpeed;

    private float additionalJump;
    private float additionalDurationJump;
    private bool isAdditionalJump;
    private float currentAdditionalDurationJump;

    private float currentDurationRoll;

    private bool isRolling = false; // Флаг перекатывания
    private bool isCollisionWithEnemy = false; // Флаг перекатывания
    private bool isAttacking = false; // Флаг атаки
    private bool wasGroundedLastFrame = false; // Новый флаг для отслеживания состояния приземления

    private Vector2 enemyPos = Vector2.zero; // Кооридинаты моба
    private Rigidbody2D body; // Тело Rigidbody2D
    private Animator anim; // Аниматор
    private float horizontalInput; // Горизонтальный ввод
    private Vector3 normalScale; // Нормальный масштаб
    private float normalGravityScale; // Гравитация установленная в инспекторе
    private Vector3 lastPosition;

    private void Awake() {
        // Получаем ссылки на Rigidbody и аниматор от объекта
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        normalScale = transform.localScale;
        normalGravityScale = body.gravityScale;
        currentAdditionalDurationSpeed = 0f;
        isAdditionalSpeed = false;
    }

    private void Update() {
        if (isAttacking || isRolling) return; // Если атака, остановить движение

        lastPosition = transform.position;
        horizontalInput = Input.GetAxis("Horizontal");

        // Отразить игрока при движении влево-вправо
        if (horizontalInput > 0f)
            transform.localScale = new Vector3(normalScale.x, normalScale.y, normalScale.z);
        else if (horizontalInput < 0f)
            transform.localScale = new Vector3(-normalScale.x, normalScale.y, normalScale.z);

        bool isRoll = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && currentDurationRoll >= timeToRoll;
        currentDurationRoll += Time.deltaTime;

        // Установить параметры аниматора
        anim.SetBool("run", horizontalInput != 0 && !isRoll && isGrounded() && !isBlockedByGround() && !onWall());
        anim.SetBool("grounded", isGrounded() && !(onWall() && isBlockedByGround()));
        anim.SetBool("fall", body.velocity.y <= 0 && !onWall());
        anim.SetBool("roll", horizontalInput != 0 && isRoll && !IsBlockedByGroundToRoll());

        // Перекатывание
        if (isRoll && horizontalInput != 0 && isGrounded() && !isRolling && !IsBlockedByGroundToRoll()) {
            StartCoroutine(Roll());
            return;
        }

        if (!isComingGround() && !isStayOnGroundOneFoot())
            SetColliderOnWall();
        else
            ResetCollider();

        if (isComingDeathZone())
            ResetCollider();

        // Включение эффекта приземления, если только что приземлились
        if (isGrounded() && !wasGroundedLastFrame && !isBlockedByGround()) {
            fallEffect.ShowEffect();
        }
        wasGroundedLastFrame = isGrounded();

        if (isGrounded() && horizontalInput != 0 && !onWall() && !isBlockedByGround()) {
            dustEffect.ShowEffect();
        } else {
            dustEffect.HideEffect();
        }

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        // Регулируемая высота прыжка
        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        // Обнуление скорости
        if (horizontalInput == 0 && !onWall()) {
            body.velocity = new Vector2(0, body.velocity.y);
            body.gravityScale = normalGravityScale;
        } else if (!isStayOnGround() && isBlockedByGround()) {
            body.velocity = new Vector2(0, body.velocity.y);
            body.gravityScale = normalGravityScale;
        } else if (onWall()) {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
        } else {
            body.velocity = new Vector2(horizontalInput * speed * (1f / Time.timeScale), body.velocity.y);
            body.gravityScale = normalGravityScale;
        }

        if (isCollisionWithEnemy &&
            ((enemyPos.x <= body.position.x && Mathf.Sign(body.velocity.x) < 0) ||
            (enemyPos.x >= body.position.x && Mathf.Sign(body.velocity.x) > 0))) {
            body.velocity = new Vector2(0, body.velocity.y);
            body.gravityScale = normalGravityScale;
        } else {
            isCollisionWithEnemy = false;
        }

        // Парение
        Slide();

        if (isGrounded() || onWall()) {
            coyoteCounter = coyoteTime; // Сброс счетчика, когда на земле
            jumpCounter = extraJumps; // Сброс счетчика прыжков на значение дополнительных прыжков
        } else
            coyoteCounter -= Time.deltaTime; // Начинаем уменьшать счетчик, когда не на земле

        if (isAdditionalSpeed) {
            if (additionalDurationSpeed >= currentAdditionalDurationSpeed) {
                body.velocity = new Vector2(body.velocity.x * additionalSpeed, body.velocity.y);
                currentAdditionalDurationSpeed += Time.deltaTime;
            } else {
                isAdditionalSpeed = false;
                currentAdditionalDurationSpeed = 0;
            }
        }

        if (isAdditionalJump) {
            if (additionalDurationJump >= currentAdditionalDurationJump) {
                currentAdditionalDurationJump += Time.deltaTime;
            } else {
                isAdditionalJump = false;
                currentAdditionalDurationJump = 0;
            }
        }

    }

    private void FixedUpdate() {
        if (lastPosition != transform.position)
            SaveManager.SavePlayerData(GetComponent<PlayerCharacteristics>());
    }

    private void Jump() {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0) return;
        // Если счетчик койота равен 0 или меньше, не на стене и нет дополнительных прыжков, ничего не делать

        SoundManager.instance.PlaySound(jumpSound);

        if (onWall() || (isBlockedByGround() && isStayOnGroundOneFoot()))
            WallJump();
        else {
            if (isGrounded())
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            else {
                // Если не на земле и счетчик койота больше 0, делаем обычный прыжок
                if (coyoteCounter > 0)
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                else {
                    if (jumpCounter > 0) // Если у нас есть дополнительные прыжки, прыгаем и уменьшаем счетчик прыжков
                    {
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }

            // Сброс счетчика койота до 0, чтобы избежать двойных прыжков
            coyoteCounter = 0;
        }

        if (isAdditionalJump) {
            if (additionalDurationJump >= currentAdditionalDurationJump) {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y * additionalJump);
            }
        }
    }

    private void Slide() {
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (!isGrounded() && !onWall() && body.velocity.y < 0) {
                body.gravityScale = lowGravityScale;
            } else {
                if (!onWall())
                    body.gravityScale = normalGravityScale;
            }
        }
    }

    private IEnumerator Roll() {
        isRolling = true;
        SoundManager.instance.PlaySound(rollSound);
        rollEffect.ShowEffect();
        SetColliderRoll();

        Vector2 rollDirection = new Vector2(horizontalInput, 0).normalized;
        float rollTime = rollDuration;

        // Пока идет перекат
        while (rollTime > 0) {
            if (isBlockedByGround(rollDirection)) break;
            body.velocity = rollDirection * rollSpeed;
            rollTime -= Time.deltaTime;

            if (isAdditionalSpeed) {
                if (additionalDurationSpeed >= currentAdditionalDurationSpeed) {
                    body.velocity = new Vector2(body.velocity.x * additionalSpeed, body.velocity.y);
                    currentAdditionalDurationSpeed += Time.deltaTime;
                } else {
                    isAdditionalSpeed = false;
                    currentAdditionalDurationSpeed = 0;
                }
            }

            yield return null;
        }

        ResetCollider();
        isRolling = false;
        currentDurationRoll = 0;
    }

    public void SetAdditionalSpeed(float _speed, float _duration) {
        additionalSpeed = _speed;
        additionalDurationSpeed = _duration;
        isAdditionalSpeed = true;
        currentAdditionalDurationSpeed = 0;
    }

    public void SetAdditionalJump(float _speed, float _duration) {
        additionalJump = _speed;
        additionalDurationJump = _duration;
        isAdditionalJump = true;
        currentAdditionalDurationJump = 0;
    }

    private void WallJump() {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
    }

    private bool isGrounded() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y), capsuleCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool isStayOnGround() {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y), Vector2.down, capsuleCollider.bounds.size.y, groundLayer);
        return hit.collider != null;
    }

    private bool isStayOnGroundOneFoot() {
        RaycastHit2D hitLeft = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.min.x, capsuleCollider.bounds.min.y), Vector2.down, capsuleCollider.bounds.size.y, groundLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.max.x, capsuleCollider.bounds.min.y), Vector2.down, capsuleCollider.bounds.size.y, groundLayer);
        return hitLeft.collider != null || hitRight.collider != null;
    }

    private bool isComingGround() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y), capsuleCollider.bounds.size, 0, Vector2.down, checkRangeToGround, groundLayer);
        return raycastHit.collider != null;
    }

    private bool isBlockedByGround() {
        RaycastHit2D hitLow = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y), new Vector2(transform.localScale.x, 0), capsuleCollider.bounds.size.x, groundLayer);
        RaycastHit2D hitMidle = Physics2D.Raycast(capsuleCollider.bounds.center, new Vector2(transform.localScale.x, 0), capsuleCollider.bounds.size.x, groundLayer);
        RaycastHit2D hitHight = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y), new Vector2(transform.localScale.x, 0), capsuleCollider.bounds.size.x, groundLayer);
        return hitMidle.collider != null || hitHight.collider != null || (hitLow.collider != null && !hitLow.collider.CompareTag("Lift"));
    }

    private bool isBlockedByGround(Vector2 direction) {
        RaycastHit2D hitLow = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y), direction, capsuleCollider.bounds.size.x * 2, groundLayer);
        RaycastHit2D hitMidle = Physics2D.Raycast(capsuleCollider.bounds.center, direction, capsuleCollider.bounds.size.x * 2, groundLayer);
        RaycastHit2D hitHight = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y), direction, capsuleCollider.bounds.size.x * 2, groundLayer);
        return hitMidle.collider != null || hitHight.collider != null || (hitLow.collider != null && !hitLow.collider.CompareTag("Lift"));
    }

    private bool IsBlockedByGroundToRoll() {
        RaycastHit2D hitLow = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y), new Vector2(transform.localScale.x, 0), capsuleCollider.bounds.size.x * 3, groundLayer);
        RaycastHit2D hitMidle = Physics2D.Raycast(capsuleCollider.bounds.center, new Vector2(transform.localScale.x, 0), capsuleCollider.bounds.size.x * 3, groundLayer);
        RaycastHit2D hitHight = Physics2D.Raycast(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.max.y), new Vector2(transform.localScale.x, 0), capsuleCollider.bounds.size.x * 3, groundLayer);
        return hitMidle.collider != null || hitHight.collider != null || (hitLow.collider != null && !hitLow.collider.CompareTag("Lift"));
    }

    private bool isComingDeathZone() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, new Vector2(capsuleCollider.bounds.size.x, capsuleCollider.bounds.size.y / 2), 0, Vector2.down, checkRangeToDeath, deathLayer);
        return raycastHit.collider != null;
    }

    private bool onWall() {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack() {
        return !onWall() && !isAttacking && !isRolling;
    }

    public void StopMovementForAttack() {
        isAttacking = true; // Устанавливаем флаг атаки
        body.gravityScale = 0;
        body.velocity = Vector2.zero; // Останавливаем движение
    }

    public void ResumeMovementAfterAttack() {
        isAttacking = false; // Снимаем флаг атаки
        body.gravityScale = normalGravityScale;
    }

    public void StopMovement() {
        body.gravityScale = 0;
        body.velocity = Vector2.zero; // Останавливаем движение
    }

    public void ResumeMovement() {
        body.gravityScale = normalGravityScale;
    }

    public void ResetCollider() {
        capsuleCollider.offset = new Vector2(0.008241236f, -0.4448698f);
        capsuleCollider.size = new Vector2(0.1273168f, 0.3745507f);
    }

    public void SetColliderOnWall() {
        capsuleCollider.offset = new Vector2(0.008241236f, -0.4279791f);
        capsuleCollider.size = new Vector2(0.1273168f, 0.1606014f);
    }

    public void SetColliderRoll() {
        capsuleCollider.offset = new Vector2(0.008241236f, -0.5457208f);
        capsuleCollider.size = new Vector2(0.1273168f, 0.1728487f);
    }

    public void OnTriggerEnter2D_Movement(Collider2D collision) {
        if (collision.tag == "EnemyHitBox") {
            isCollisionWithEnemy = true;
            enemyPos = collision.bounds.min;
        }
    }

    public void OnTriggerExit2D_Movement(Collider2D collision) {
        if (collision.tag == "EnemyHitBox") {
            isCollisionWithEnemy = false;
            enemyPos = Vector2.zero;
        }
    }

    public void OnDrawGizmos_Movement() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y),
            new Vector3(capsuleCollider.bounds.size.x + checkRangeToGround, capsuleCollider.bounds.size.y + checkRangeToGround, capsuleCollider.bounds.size.z + checkRangeToGround));
    }
}