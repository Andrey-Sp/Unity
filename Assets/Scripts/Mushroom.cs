using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mushroom : MonoBehaviour
{
    [SerializeField] private float walkRange;
    [SerializeField] private bool faceRight = true;
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private float speed;
    [SerializeField] private int enemyDamage;
    [SerializeField] private float damageDelay;
    [SerializeField] private int _maxHp;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _enemySystem;

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
        Gizmos.DrawWireCube(drawPosition, new Vector3(walkRange * 2, 1, 0));
    }

    private void Update()
    {
        if (faceRight && transform.position.x > startPosition.x + walkRange)
        {
            Flip();
        }
        else if (!faceRight && transform.position.x < startPosition.x - walkRange)
        {
            Flip();
        }
        if (_player != null && Time.time - lastDamageTime > damageDelay)
        {
            lastDamageTime = Time.time;
            _player.GetHurt(enemyDamage);
        }
    }

    private void FixedUpdate()
    {
        rigidBody2D.velocity = Vector2.right * direction * speed;
    }

    private void Flip()
    {
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
        direction *= -1;
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
}
