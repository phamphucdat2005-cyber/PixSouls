using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;

    private int currentHealth;
    private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
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
        animator.SetTrigger("Dead");

        Destroy(gameObject, 0.3f);
    }
}