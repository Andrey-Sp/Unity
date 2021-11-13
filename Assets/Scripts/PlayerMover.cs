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
    [SerializeField] private Transform AttackChecker;
    [SerializeField] private int attackDamage;
    [SerializeField] private int maxHp;
    [SerializeField] private bool _faceRight;

    [SerializeField] private Transform _wavePoint;
    [SerializeField] private Rigidbody2D _wave;
    [SerializeField] private float _waveSpeed;
    [SerializeField] private SpriteRenderer waveSpriteRenderer;


    [Header(("Animation"))]
    [SerializeField] private Animator animator;
    [SerializeField] private string runAnimatorKey;
    [SerializeField] private string jumpAnimatorKey;
    [SerializeField] private string fallAnimatorKey;
    [SerializeField] private string crouchAnimatorKey;
    [SerializeField] private string attackAnimatorKey;
    [SerializeField] private string hurtAnimatorKey;
    [SerializeField] private string castAnimatorKey;

    [Header(("UI"))]
    [SerializeField] private TMP_Text coinsAmountText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider energyBar;

    private float direction;
    private bool jump;
    private bool crawl;
    private bool attack;
    private int currentHp;
    private int currentEnergy;
    private float EnergyLossDelay = 0.55f;
    private float lastEnergyLossTime;
    public int coinsAmount;
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

        if (direction > 0 && !_faceRight || direction < 0 && _faceRight)
        {
            _faceRight = !_faceRight;
            transform.Rotate(0, 180, 0);
        }

        crawl = Input.GetKey(KeyCode.LeftControl);

        if (Input.GetKey(KeyCode.E))
        {
            animator.SetBool(attackAnimatorKey, true);
        }
        else
            animator.SetBool(attackAnimatorKey, false);

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!_faceRight)
                waveSpriteRenderer.flipX = true;
            else if (_faceRight)
                waveSpriteRenderer.flipX = false;
            if (CurrentEnergy >= 25)
            {
                animator.SetBool(castAnimatorKey, true);
                if (Time.time - lastEnergyLossTime > EnergyLossDelay)
                {
                    lastEnergyLossTime = Time.time;
                    LoseEnergy();
                }
            }
        }

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
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundChecker.position, groundCheckerRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(headChecker.position, headCheckerRadius);
        Gizmos.DrawWireCube(AttackChecker.position, new Vector3(attackCheckerRadius, attackCheckerRadius, 0));
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

    private void Attack()
    {
        Collider2D[] targets = Physics2D.OverlapBoxAll(AttackChecker.position, new Vector2(attackCheckerRadius, attackCheckerRadius), whatIsEnemy);
        foreach(var target in targets)
        {
            Eye eye = target.GetComponent<Eye>();
            if(eye != null)
            {
                eye.TakeDamage(attackDamage);
            }
            Skeleton skeleton = target.GetComponent<Skeleton>();
            if (skeleton != null)
            {
                skeleton.TakeDamage(attackDamage);
            }
            Mushroom mushroom = target.GetComponent<Mushroom>();
            if (mushroom != null)
            {
                mushroom.TakeDamage(attackDamage);
            }
        }
    }

    private void CastWave()
    {
        Rigidbody2D wave = Instantiate(_wave, _wavePoint.position, Quaternion.identity);
        wave.velocity = _waveSpeed * transform.right;
    }

    private void CastEnd()
    {
        animator.SetBool(castAnimatorKey, false);
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
