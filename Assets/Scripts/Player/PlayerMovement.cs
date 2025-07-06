using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EightDirectionMovement : MonoBehaviour
{
    public float speed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody rb;
    private Animator animator;
    private int lastDirection = 1;

    private bool isDashing = false;
    private float dashTime = 0f;
    private float lastDashTime = -Mathf.Infinity;
    private Vector3 dashDirection;

    public Vector3 CurrentDirection { get; private set; } = Vector3.forward;

    public bool colliding;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, 0f, v).normalized;
        if (direction.sqrMagnitude > 0.01f)
            CurrentDirection = direction;

        // Dash logic
        if (!isDashing && Input.GetKey(KeyCode.Space) && Time.time >= lastDashTime + dashCooldown && direction.sqrMagnitude > 0.01f)
        {
            animator.SetBool("IsRolling", true);
            animator.SetBool("Attacking", false);
            animator.SetBool("Casting", false);
            animator2.SetBool("IsRolling", true);
            animator2.SetBool("Attacking", false);
            animator2.SetBool("Casting", false);
            isDashing = true;
            dashTime = 0f;
            dashDirection = direction;
            lastDashTime = Time.time;

        }

        if (isDashing)
        {

            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
            dashTime += Time.fixedDeltaTime;
            if (dashTime >= dashDuration)
            {
                isDashing = false;
                animator.SetBool("IsRolling", false);
                animator2.SetBool("IsRolling", false);
            }
        }
        else
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }

        bool isMoving = direction.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);
        animator2.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            int dirValue = GetDirection(h, v);
            animator.SetInteger("Direction", dirValue);
            animator2.SetInteger("Direction", dirValue);
            lastDirection = dirValue;
        }
        else
        {
            animator.SetInteger("Direction", lastDirection);
            animator2.SetInteger("Direction", lastDirection);
        }

    }

    int GetDirection(float h, float v)
    {
        if (!isDashing)
        {
            if (v > 0 && h == 0) return 5;     // Up
            if (v > 0 && h > 0) return 6;      // UpRight
            if (v > 0 && h < 0) return 4;      // UpLeft
            if (v < 0 && h == 0) return 1;     // Down
            if (v < 0 && h > 0) return 8;      // DownRight
            if (v < 0 && h < 0) return 2;      // DownLeft
            if (v == 0 && h > 0) return 7;     // Right
            if (v == 0 && h < 0) return 3;     // Left
        }
        return lastDirection;
    }

    private void OnCollisionExit(Collision collision)
    {
        rb.velocity = Vector3.zero; // Stop movement on collision exit
    }    
    
}