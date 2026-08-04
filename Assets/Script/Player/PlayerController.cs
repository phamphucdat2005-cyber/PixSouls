﻿using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.down;
  
    private PlayerInputActions inputActions;

    private bool isPunching = false;
    private bool isAiming = false;

    // Health
    public int maxHealth = 100;
    public int currentHealth;

    // Dash
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    // Invincibility
    private bool isInvincible = false;

    [SerializeField]
    private GameObject arrowPrefab;

    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private float arrowSpeed = 10f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
       
        inputActions = new PlayerInputActions();

        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Punch.performed += OnPunch;
        inputActions.Player.Aim.started += OnAimStarted;
        inputActions.Player.Aim.canceled += OnAimCanceled;
        inputActions.Player.Dash.performed += OnDash;
    }

    private void OnDisable()
    {
        inputActions.Player.Punch.performed -= OnPunch;
        inputActions.Player.Aim.started -= OnAimStarted;
        inputActions.Player.Aim.canceled -= OnAimCanceled;
        inputActions.Player.Dash.performed -= OnDash;

        inputActions.Disable();
    }

    private void Update()
    {
        if (PauseManager.IsPaused || isDashing)
        {
            moveInput = Vector2.zero;
            return;
        }
        if (!isPunching && !isAiming)
        {
            moveInput = inputActions.Player.Move.ReadValue<Vector2>();

            if (moveInput != Vector2.zero)
            {
                lastDirection = moveInput.normalized;
            }
        }
        else
        {
            moveInput = Vector2.zero;
        }

        animator.SetFloat("MoveX", lastDirection.x);
        animator.SetFloat("MoveY", lastDirection.y);
        animator.SetFloat("Speed", moveInput.magnitude);
        
        if (isAiming)
        {
            UpdateAimDirection();
        }
    }
    private void FixedUpdate()
    {
        if (isPunching || isDashing)
        {
            if(isPunching)
                rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    private void OnPunch(InputAction.CallbackContext context)
    {
        if (isPunching)
            return;

        isPunching = true;

        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;

        animator.SetTrigger("Punch");
    }

    public void EndPunch()
    {
        Debug.Log("End Punch Called");
        isPunching = false;
    }
    private void OnAimStarted(InputAction.CallbackContext context)
    {
        if (isPunching)
            return;

        isAiming = true;

        animator.SetBool("IsAiming", true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        if (!isAiming)
            return;

        isAiming = false;

        animator.SetBool("IsAiming", false);

        ShootArrow();
    }
    private void UpdateAimDirection()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        Vector2 dir =
            mousePos - transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            lastDirection =
                dir.x > 0
                    ? Vector2.right
                    : Vector2.left;
        }
        else
        {
            lastDirection =
                dir.y > 0
                    ? Vector2.up
                    : Vector2.down;
        }
    }
    private void ShootArrow()
    {
        Vector3 mousePos =
            Camera.main.ScreenToWorldPoint(
                Mouse.current.position.ReadValue());

        mousePos.z = 0;

        Vector2 direction =
            (mousePos - firePoint.position)
            .normalized;
        Vector2 spawnPosition =
    (Vector2)transform.position +
    direction * 0.5f;
        GameObject arrow =
            Instantiate(
                arrowPrefab,
                spawnPosition,
                Quaternion.identity);

        Rigidbody2D rb =
            arrow.GetComponent<Rigidbody2D>();

        rb.linearVelocity =
            direction * arrowSpeed;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        arrow.transform.rotation =
            Quaternion.Euler(0, 0, angle);
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (canDash && !isPunching && !isAiming)
        {
            StartCoroutine(Dash());
        }
    }

    private System.Collections.IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        isInvincible = true;

        rb.linearVelocity = lastDirection.normalized * dashSpeed;
        // Bạn có thể thêm trigger animation cho dash ở đây
        // animator.SetTrigger("Dash");

        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rb.linearVelocity = Vector2.zero;

        // Miễn nhiễm kéo dài 0.4 giây từ lúc bắt đầu dash
        yield return new WaitForSeconds(0.4f - dashDuration);
        isInvincible = false;

        // Thời gian hồi chiêu
        yield return new WaitForSeconds(dashCooldown - 0.4f);
        canDash = true;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;

        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // Lệnh này sẽ vô hiệu hóa đối tượng người chơi. Bạn có thể thay thế bằng logic khác như
        // tải lại màn chơi hoặc hiển thị menu game over.
        gameObject.SetActive(false);
    }
}