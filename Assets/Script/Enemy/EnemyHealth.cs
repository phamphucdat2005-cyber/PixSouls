
using UnityEngine;

public class EnemyHealth : MonoBehaviour, ISaveable
{
    [Header("Health")]
    public int maxHealth = 10;

   
    private bool isDead = false;

    private int currentHealth;

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            UpdateHealthBar();
        }
    }

    private Animator animator;
    private EnemyAI enemyAI;
    private Rigidbody2D rb;

    public HealthBarUI healthBar;

    [Header("Save")]
    public string enemyID;
    public string prefabID;

    private void Awake()
    {
        currentHealth = maxHealth;

        animator = GetComponentInChildren<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();

        // Lấy thanh máu thuộc chính Enemy này
        healthBar = GetComponentInChildren<HealthBarUI>(true);
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // Nhận sát thương từ viên đạn
    public void TakeDamage(int damage, Vector2 damageSourcePos)
    {
        if (isDead || damage <= 0)
            return;

        // Trừ máu
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        // Cập nhật thanh máu
        UpdateHealthBar();

        // Kiểm tra chết
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Bật lùi khi nhận sát thương
        if (enemyAI != null)
        {
            enemyAI.ApplyKnockback(damageSourcePos);
        }
    }

    

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Dừng AI và chuyển động
        if (enemyAI != null)
            enemyAI.StopAI();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Xóa các mũi tên đang gắn trên Enemy
        foreach (Arrow arrow in GetComponentsInChildren<Arrow>())
        {
            Destroy(arrow.gameObject);
        }

        // Ẩn thanh máu
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        // Chạy animation chết
        if (animator != null)
            animator.SetTrigger("Dead");

        Destroy(gameObject, 0.3f);
    }

    public void Save(GameData data)
    {
        EnemyData enemy = new EnemyData();

        enemy.enemyID = enemyID;
        enemy.prefabID = prefabID;

        enemy.posX = transform.position.x;
        enemy.posY = transform.position.y;
        enemy.posZ = transform.position.z;

        enemy.currentHealth = currentHealth;
        enemy.isDead = isDead || currentHealth <= 0;

        data.enemies.Add(enemy);
    }

    public void Load(GameData data)
    {
        EnemyData enemy =
            data.enemies.Find(e => e.enemyID == enemyID);

        if (enemy == null)
            return;

        if (enemy.isDead)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = new Vector3(
            enemy.posX,
            enemy.posY,
            enemy.posZ
        );

        currentHealth = enemy.currentHealth;
        isDead = false;

        UpdateHealthBar();
    }
}
