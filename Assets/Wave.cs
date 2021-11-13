using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _lifeTime;

    void Start()
    {
        Invoke(nameof(Destroy), _lifeTime);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Eye eye = other.GetComponent<Eye>();
        if (eye != null)
        {
            eye.TakeDamage(_damage);
        }
        Skeleton skeleton = other.GetComponent<Skeleton>();
        if (skeleton != null)
        {
            skeleton.TakeDamage(_damage);
        }
        Mushroom mushroom = other.GetComponent<Mushroom>();
        if (mushroom != null)
        {
            mushroom.TakeDamage(_damage);
        }
    }
}
