
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectRange = 5f;

    private Transform player;
    private Transform companion;
    private Transform target;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 lastDirection = Vector2.down;

    [Header("Combat")]
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    private float attackTimer;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float knockbackDuration = 0.15f;

    private bool isKnockbacked = false;
    private Coroutine knockbackCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;

        GameObject companionObj =
            GameObject.FindGameObjectWithTag("Companion");

        if (companionObj != null)
            companion = companionObj.transform;
    }

    private void FixedUpdate()
    {
        // Không di chuyển AI khi đang bật lùi
        if (isKnockbacked)
            return;

        target = GetNearestTarget();

        if (target == null)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimator(Vector2.zero);
            return;
        }

        float distance = Vector2.Distance(
            rb.position,
            target.position
        );

        Vector2 direction =
            ((Vector2)target.position - rb.position).normalized;

        if (distance > attackRange && distance <= detectRange)
        {
            rb.linearVelocity = direction * moveSpeed;
            lastDirection = direction;
        }
        else if (distance <= attackRange)
        {
            rb.linearVelocity = Vector2.zero;
            Attack();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }

        UpdateAnimator(rb.linearVelocity);
    }

    // Giữ tương thích nếu đạn đang gọi EnemyAI.TakeDamage
    public void TakeDamage(int damage, Vector2 damageSourcePos)
    {
        EnemyHealth health = GetComponent<EnemyHealth>();

        if (health != null)
            health.TakeDamage(damage, damageSourcePos);
    }

    // Được EnemyHealth gọi sau khi nhận sát thương
    public void ApplyKnockback(Vector2 damageSourcePos)
    {
        Vector2 direction = rb.position - damageSourcePos;

        // Trường hợp viên đạn trùng vị trí Enemy
        if (direction.sqrMagnitude < 0.0001f)
            direction = -lastDirection;

        direction.Normalize();

        // Nếu đang bật lùi thì bắt đầu lại
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine =
            StartCoroutine(KnockbackRoutine(direction));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction)
    {
        isKnockbacked = true;
        attackTimer = 0f;

        float timer = 0f;

        UpdateAnimator(Vector2.zero);

        while (timer < knockbackDuration)
        {
            rb.linearVelocity = direction * knockbackForce;

            yield return new WaitForFixedUpdate();

            timer += Time.fixedDeltaTime;
        }

        rb.linearVelocity = Vector2.zero;

        isKnockbacked = false;
        knockbackCoroutine = null;
    }

    private void UpdateAnimator(Vector2 currentVelocity)
    {
        if (animator == null)
            return;

        animator.SetFloat("Speed", currentVelocity.magnitude);
        animator.SetFloat("MoveX", lastDirection.x);
        animator.SetFloat("MoveY", lastDirection.y);
    }

    private void Attack()
    {
        attackTimer += Time.fixedDeltaTime;

        if (attackTimer < attackCooldown)
            return;

        attackTimer = 0f;

        if (target.CompareTag("Player"))
        {
            PlayerMovement playerHealth =
                target.GetComponent<PlayerMovement>();

            if (playerHealth != null)
                playerHealth.TakeDamage(1);
        }
        else if (target.CompareTag("Companion"))
        {
            CompanionAI companionScript =
                target.GetComponent<CompanionAI>();

            if (companionScript != null)
                companionScript.TakeDamage(1);
        }
    }

    private Transform GetNearestTarget()
    {
        Transform nearest = null;
        float nearestDistance = detectRange;

        if (player != null)
        {
            float d = Vector2.Distance(
                transform.position,
                player.position
            );

            if (d <= detectRange)
            {
                nearest = player;
                nearestDistance = d;
            }
        }

        if (companion != null)
        {
            float d = Vector2.Distance(
                transform.position,
                companion.position
            );

            if (d <= detectRange && d < nearestDistance)
            {
                nearest = companion;
                nearestDistance = d;
            }
        }

        return nearest;
    }

    // Dừng AI khi Enemy chết
    public void StopAI()
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }

        isKnockbacked = false;
        rb.linearVelocity = Vector2.zero;

        enabled = false;
    }
}
