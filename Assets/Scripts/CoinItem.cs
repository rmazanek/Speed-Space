using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    [SerializeField] int coinIncrease = 1;
    [SerializeField] AudioClip coinUpSound;
    [SerializeField] float coinUpSoundVolume = 0.05f;
    Wallet wallet;
    Player player;
    private void OnTriggerEnter2D(Collider2D other)
    {
        wallet = FindObjectOfType<Wallet>();
        player = other.gameObject.GetComponent<Player>();
        if(wallet != null && player != null )
        {
            wallet.AddToWallet(coinIncrease);
            AudioSource.PlayClipAtPoint(coinUpSound, Camera.main.transform.position, coinUpSoundVolume);
            Destroy(gameObject);
        }
    }
}
