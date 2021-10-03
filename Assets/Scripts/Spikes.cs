using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float damageDelay;
    private float lastDamageTime;
    private PlayerMover _player;

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
    private void Update()
    {
        if (_player != null && Time.time - lastDamageTime > damageDelay)
        {
            lastDamageTime = Time.time;
            _player.GetHurt(damage);
        }
    }
}
