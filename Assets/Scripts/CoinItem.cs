using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : MonoBehaviour
{
    [SerializeField] int coinIncrease = 1;
    [SerializeField] AudioClip coinUpSound;
    [SerializeField] float coinUpSoundVolume = 0.05f;
    Wallet wallet;
    private void OnTriggerEnter2D(Collider2D other)
    {
        wallet = FindObjectOfType<Wallet>();
        if(wallet != null)
        {
            wallet.AddToWallet(coinIncrease);
            AudioSource.PlayClipAtPoint(coinUpSound, Camera.main.transform.position, coinUpSoundVolume);
            Destroy(gameObject);
        }
    }
}
