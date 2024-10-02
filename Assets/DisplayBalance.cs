using Solana.Unity.SDK;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DisplayBalance : MonoBehaviour
{
    private TextMeshProUGUI _txtBalance;
    private void Start()
    {
        _txtBalance = GetComponent<TextMeshProUGUI>();
    }
    private void OnEnable()
    {
        Web3.OnBalanceChange += OnBalanceChange;
    }

    private void OnDisable()
    {
        Web3.OnBalanceChange -= OnBalanceChange;
    }

    private void OnBalanceChange(double amount)
    {
        _txtBalance.text = amount.ToString(CultureInfo.InvariantCulture);
    }
}