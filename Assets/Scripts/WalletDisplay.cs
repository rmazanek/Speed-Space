using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WalletDisplay : MonoBehaviour
{
  Wallet wallet;
  void Start()
  {
    UpdateWalletDisplay();
  }
  public void UpdateWalletDisplay()
  {
    wallet = FindObjectOfType<Wallet>(true);
    GetComponent<TextMeshProUGUI>().text = wallet.GetCoinTotal().ToString();
  }
}