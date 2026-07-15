using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectRange = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 lastDirection = Vector2.down;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance <= detectRange)
        {
            Vector2 direction =
                ((Vector2)player.position - rb.position).normalized;

            rb.linearVelocity = direction * moveSpeed;

            lastDirection = direction;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (animator != null)
        {
            animator.SetFloat("MoveX", lastDirection.x);
            animator.SetFloat("MoveY", lastDirection.y);
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        }
    }
}