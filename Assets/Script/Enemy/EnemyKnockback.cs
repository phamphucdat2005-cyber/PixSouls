using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Nhớ thêm dòng này nếu dùng NavMesh

public class EnemyKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.15f;

    private Rigidbody rb;
    private NavMeshAgent agent; // Thêm biến NavMeshAgent

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>(); // Lấy component NavMeshAgent
    }

    public void ApplyKnockback(Vector3 damageSourcePosition)
    {
        Vector3 direction = (transform.position - damageSourcePosition).normalized;
        direction.y = 0;

        StartCoroutine(KnockbackRoutine(direction));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction)
    {
        // 1. Tạm tắt NavMeshAgent để tránh xung đột vật lý
        if (agent != null) agent.enabled = false;

        float timer = 0;
        while (timer < knockbackDuration)
        {
            Vector3 moveAmount = direction * knockbackForce * Time.deltaTime;

            if (rb != null)
            {
                rb.MovePosition(transform.position + moveAmount);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // 2. Bật lại NavMeshAgent sau khi đẩy lùi xong
        if (agent != null) agent.enabled = true;
    }
}