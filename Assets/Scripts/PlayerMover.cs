using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMover : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    [SerializeField] private float speed;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private float groundCheckerRadius;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Collider2D headCollider;
    [SerializeField] private float headCheckerRadius;
    [SerializeField] private Transform headChecker;

    [Header(("Animation"))]
    [SerializeField] private Animator animator;
    [SerializeField] private string runAnimatorKey;
    [SerializeField] private string jumpAnimatorKey;
    [SerializeField] private string fallAnimatorKey;
    [SerializeField] private string crouchAnimatorKey;
    [SerializeField] private string attackAnimatorKey;
    [SerializeField] private string hurtAnimatorKey;

    private float direction;
    private bool jump;
    private bool crawl;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        direction = Input.GetAxisRaw("Horizontal");

        animator.SetFloat(runAnimatorKey, Mathf.Abs(direction));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

            if (direction > 0 && spriteRenderer.flipX)
            spriteRenderer.flipX = false;
        else if (direction < 0 && !spriteRenderer.flipX)
            spriteRenderer.flipX = true;

        crawl = Input.GetKey(KeyCode.LeftControl);

    }
    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(direction * speed, rigidbody.velocity.y);
        bool canJump = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, whatIsGround);
        bool canStand = !Physics2D.OverlapCircle(headChecker.position, headCheckerRadius, whatIsGround);

        headCollider.enabled = !crawl && canStand;

        if (jump && canJump)
        {
            rigidbody.AddForce(Vector2.up * jumpForce);
            jump = false;
        }

        animator.SetBool(jumpAnimatorKey, !canJump);
        if (rigidbody.velocity.y < -1)
        {
            animator.SetBool(fallAnimatorKey, true);
        }
        else
            animator.SetBool(fallAnimatorKey, false);
        animator.SetBool(crouchAnimatorKey, !headCollider.enabled);

        if (Input.GetKey(KeyCode.E))
        {
            animator.SetBool(attackAnimatorKey, true);
        }
        else
            animator.SetBool(attackAnimatorKey, false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundChecker.position, groundCheckerRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(headChecker.position, headCheckerRadius);
    }
    public void AddHp(int hpPoints)
    {
        Debug.Log("HP increased " + hpPoints);
    }
    public void GetHurt(int enemyDamage)
    {
        animator.SetTrigger("Hurt");
        Debug.Log("HP decreased " + enemyDamage);
    }
    public void LevelPass()
    {
        Debug.Log("You have completed that level!");
    }
}
