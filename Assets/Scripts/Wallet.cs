using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    int maxWalletSize = 99;
    int coinTotal = 0;
    WalletDisplay walletDisplay;
    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public void AddToWallet(int amountToAdd)
    {
        coinTotal = Mathf.Min(coinTotal + amountToAdd, maxWalletSize);
        UpdateWalletDisplay();
    }
    public void SubtractFromWallet(int amountToSubtract)
    {
        coinTotal = Mathf.Max(coinTotal - amountToSubtract, 0);
        UpdateWalletDisplay();
    }
    public void ChangeWalletSize(int newValue)
    {
        maxWalletSize = newValue;
    }
    public int GetCoinTotal()
    {
        return coinTotal;
    }
    private void UpdateWalletDisplay()
    {
        walletDisplay = FindObjectOfType<WalletDisplay>();
        if(walletDisplay != null)
        {
            walletDisplay.UpdateWalletDisplay();
        }
    }
}
