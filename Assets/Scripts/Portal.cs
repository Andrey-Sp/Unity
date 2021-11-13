using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private int moneyToNextLevel;
    [SerializeField] private int levelToLoad;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite openPortalSprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMover player = other.GetComponent<PlayerMover>();
        if (player != null && player.coinsAmount >= moneyToNextLevel)
        {
            spriteRenderer.sprite = openPortalSprite;
            Invoke("LoadNextScene", 1f);
        }
    }
    private void LoadNextScene()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
