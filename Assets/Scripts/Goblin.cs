using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    [SerializeField] private float walkRange;
    [SerializeField] private bool faceRight = true;
    [SerializeField] private Rigidbody2D rigidBody2D;
    [SerializeField] private float speed;
    [SerializeField] private int enemyDamage;
    [SerializeField] private float damageDelay;
    [SerializeField] private float jumpForce;
    [SerializeField] private string jumpAnimatorKey;
    private bool jump;
    private float lastDamageTime;
    private PlayerMover _player;
    private int direction = 1;
    private Vector2 startPosition;
    private float lastAttackTime;

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
        startPosition = transform.position;
        rigidbody = GetComponent<Rigidbody2D>();
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
        rigidbody.velocity = new Vector2(direction * speed, rigidbody.velocity.y);
    }

    private void Flip()
    {
        faceRight = !faceRight;
        transform.Rotate(0, 180, 0);
        direction *= -1;
    }

    private void Jump()
    {
        rigidbody.AddForce(Vector2.up * jumpForce);
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
