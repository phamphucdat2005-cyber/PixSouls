using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CompanionAI : MonoBehaviour,
ISaveable
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public GameObject arrowPrefab;
    public Animator animator;

    [Header("Movement")]
    public float followRadius = 2f;
    public float moveSpeed = 3f;

    [Header("Combat")]
    public float detectRange = 6f;
    public float attackRange = 3f;
    public float fireRate = 1f;

    private Rigidbody2D rb;

    private float fireTimer;
    private Transform target;

    private Vector2 lastDirection = Vector2.down;
    public float arrowSpeed = 10f;

    public int maxHealth = 10;

    private int currentHealth;

    public HealthBarUI healthBar;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.UpdateHealth(currentHealth, maxHealth);
    }

    void Update()
    {
        FindTarget();

        if (target != null)
        {
            AttackBehaviour();
        }
        else
        {
            FollowPlayer();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.UpdateHealth(currentHealth, maxHealth);


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        foreach (Arrow arrow in GetComponentsInChildren<Arrow>())
        {
            Destroy(arrow.gameObject);
        }


        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        animator.SetTrigger("IsDead");

        Destroy(gameObject, 0.3f);
    }
    void FollowPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > followRadius)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

            rb.linearVelocity = dir * moveSpeed;

            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);

            lastDirection = dir;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }
    }

    void FindTarget()
    {
        EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>();

        float closest = detectRange;
        target = null;

        foreach (EnemyHealth enemy in enemies)
        {
            float d = Vector2.Distance(transform.position, enemy.transform.position);

            if (d < closest)
            {
                closest = d;
                target = enemy.transform;
            }
        }
    }

    void AttackBehaviour()
    {
        if (target == null)
            return;

        float distance = Vector2.Distance(transform.position, target.position);

        Vector2 dir = (target.position - transform.position).normalized;

        lastDirection = dir;

        animator.SetFloat("MoveX", dir.x);
        animator.SetFloat("MoveY", dir.y);

        if (distance > attackRange)
        {
            rb.linearVelocity = dir * moveSpeed;
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            return;
        }

        rb.linearVelocity = Vector2.zero;
        animator.SetFloat("Speed", 0);

        firePoint.right = dir;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0;
            animator.SetTrigger("Shoot");
            
        }
    }

    public void FireArrow()
    {
        Vector2 dir = lastDirection.normalized;

        GameObject arrow = Instantiate(
            arrowPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Collider2D arrowCollider = arrow.GetComponent<Collider2D>();
        Collider2D npcCollider = GetComponent<Collider2D>();

        if (arrowCollider != null && npcCollider != null)
        {
            Physics2D.IgnoreCollision(arrowCollider, npcCollider);
        }

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();

        arrowRb.linearVelocity = dir * arrowSpeed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    public void Save(GameData data)
    {
        data.companion = new CompanionData();

        data.companion.x = transform.position.x;
        data.companion.y = transform.position.y;
        data.companion.health = currentHealth;
    }

    public void Load(GameData data)
    {
        transform.position =
            new Vector3(
                data.companion.x,
                data.companion.y,
                0);

        currentHealth =
            data.companion.health;
    }
}