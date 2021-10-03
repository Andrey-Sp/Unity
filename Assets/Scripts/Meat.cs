using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null)
        {
            StartCoroutine(player.RestoreEnergy(100));
            Destroy(gameObject);
        }
    }
}
