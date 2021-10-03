using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private Collider2D headCollider;
    [SerializeField] private float headCheckerRadius;
    [SerializeField] private Transform headChecker;
    [SerializeField] private float attackCheckerRadius;
    [SerializeField] private Transform leftAttackChecker;
    [SerializeField] private Transform rightAttackChecker;
    [SerializeField] private UnityEngine.Object enemy;
    [SerializeField] private int maxHp;


    [Header(("Animation"))]
    [SerializeField] private Animator animator;
    [SerializeField] private string runAnimatorKey;
    [SerializeField] private string jumpAnimatorKey;
    [SerializeField] private string fallAnimatorKey;
    [SerializeField] private string crouchAnimatorKey;
    [SerializeField] private string attackAnimatorKey;
    [SerializeField] private string hurtAnimatorKey;

    [Header(("UI"))]
    [SerializeField] private TMP_Text coinsAmountText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider energyBar;

    private float direction;
    private bool jump;
    private bool crawl;
    public bool canHit;
    private bool attack;
    private int currentHp;
    private int currentEnergy;
    private float EnergyLossDelay = 0.55f;
    private float lastEnergyLossTime;
    public int kills = 0;

    private int coinsAmount;
    public int CoinsAmount
    {
        get => coinsAmount;
        set
        {
            coinsAmount = value;
            coinsAmountText.text = "Coins: " + value.ToString();
        }
    }
    public int CurrentHp
    {
        get => currentHp;
        set
        {
            currentHp = value;
            hpBar.value = value;
        }
    }

    public int CurrentEnergy
    {
        get => currentEnergy;
        set
        {
            currentEnergy = value;
            energyBar.value = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CoinsAmount = 0;
        hpBar.maxValue = maxHp;
        energyBar.maxValue = maxHp;
        CurrentEnergy = maxHp;
        CurrentHp = maxHp;
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

        if (Input.GetKey(KeyCode.E))
        {
            if (CurrentEnergy >= 25)
            {
                animator.SetBool(attackAnimatorKey, true);
                if (Time.time - lastEnergyLossTime > EnergyLossDelay)
                {
                    lastEnergyLossTime = Time.time;
                    LoseEnergy();
                }
                if (canHit)
                    Invoke("DestroyEnemy", 0.3f);
            }
        }
        else
            animator.SetBool(attackAnimatorKey, false);

    }
    private void FixedUpdate()
    {
        rigidbody.velocity = new Vector2(direction * speed, rigidbody.velocity.y);
        bool canJump = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, whatIsGround);
        bool canStand = !Physics2D.OverlapCircle(headChecker.position, headCheckerRadius, whatIsGround);
        if(spriteRenderer.flipX)
            canHit = Physics2D.OverlapCircle(leftAttackChecker.position, attackCheckerRadius, whatIsEnemy);
        else
            canHit = Physics2D.OverlapCircle(rightAttackChecker.position, attackCheckerRadius, whatIsEnemy);

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
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundChecker.position, groundCheckerRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(headChecker.position, headCheckerRadius);
        Gizmos.DrawWireSphere(leftAttackChecker.position, attackCheckerRadius);
        Gizmos.DrawWireSphere(rightAttackChecker.position, attackCheckerRadius);
    }
    private void DestroyEnemy()
    {
        Destroy(enemy);
        kills++;
    }
    public void AddHp(int hpPoints)
    {
        int missingHp = maxHp - CurrentHp;
        int pointsToAdd = missingHp > hpPoints ? hpPoints : missingHp;
        StartCoroutine(RestoreHp(pointsToAdd));
    }

    private void LoseEnergy()
    {
        CurrentEnergy -= 33;
    }

    private IEnumerator RestoreHp(int pointsToAdd)
    {
        while (pointsToAdd != 0)
        {
            pointsToAdd--;
            CurrentHp++;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator RestoreEnergy(int max)
    {
        while (CurrentEnergy < max)
        {
            CurrentEnergy += 100;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void GetHurt(int enemyDamage)
    {
        animator.SetTrigger("Hurt");
        CurrentHp -= enemyDamage;
        if (currentHp <= 0)
        {
            Debug.Log("You Died");
            gameObject.SetActive(false);
            Invoke(nameof(ReloadScene), 1f);
        }
    }
    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void AddCoins(int coinsAmount)
    {
        CoinsAmount += coinsAmount;
    }
    public void LevelPass()
    {
        Debug.Log("You have completed that level!");
    }
}
