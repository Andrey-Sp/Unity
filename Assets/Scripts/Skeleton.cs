using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skeleton : MonoBehaviour
{
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private float _walkRange;
    [SerializeField] private bool faceRight = true;
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private float speed;
    [SerializeField] private int enemyDamage;
    [SerializeField] private float damageDelay;
    [SerializeField] private bool _faceRight;
    [SerializeField] private int _maxHp;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _enemySystem;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _walkAnimationKey;

    private float lastDamageTime;
    private PlayerMover _player;
    private int direction = 1;
    private Vector2 startPosition;
    private float lastAttackTime;
    private int _currentHp;

    private int CurrentHp
    {
        get => _currentHp;
        set
        {
            _currentHp = value;
            _slider.value = value;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHp -= damage;
        if (CurrentHp <= 0)
        {
            Destroy(_enemySystem);
        }
    }

    private Vector2 drawPosition
    {
        get
        {
            if (startPosition == Vector2.zero)
                return transform.position;
            else
                return startPosition;
        }
    }

    private void Start()
    {
        _slider.maxValue = _maxHp;
        CurrentHp = _maxHp;
        startPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(drawPosition, new Vector3(_walkRange * 2, 1, 0));
    }

    private void Update()
    {
        if (_player != null && Time.time - lastDamageTime > damageDelay)
        {
            lastDamageTime = Time.time;
            _player.GetHurt(enemyDamage);
        }
    }

    private void FixedUpdate()
    {
        CheckIfPlayerNear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null)
        {
            _player = player;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player == _player)
        {
            _player = null;

        }
    }
    private void CheckIfPlayerNear()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(_walkRange, 1), 0, _whatIsPlayer);
        if (player != null)
        {
            Walk(player.transform.position);
        }
        else
        {
            _animator.SetBool(_walkAnimationKey, false);
            rigidBody2D.velocity = Vector2.right * 0;
        }
    }
    private void Walk(Vector2 playerPosition)
    {
        _animator.SetBool(_walkAnimationKey, true);
        if (transform.position.x > playerPosition.x && !_faceRight)
        {
            direction = -1;
            _faceRight = !_faceRight;
            transform.Rotate(0, 180, 0);
        }
        if (transform.position.x < playerPosition.x && _faceRight)
        {
            direction = 1;
            _faceRight = !_faceRight;
            transform.Rotate(0, 180, 0);
        }
        _animator.SetBool(_walkAnimationKey, true);
        rigidBody2D.velocity = Vector2.right * direction * speed;
    }
}
