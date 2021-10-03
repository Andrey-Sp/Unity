using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] private int coinsAmount;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null)
        {
            player.AddCoins(coinsAmount);
            Destroy(gameObject);
        }
    }
}
