using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayPublicKey : MonoBehaviour
{
    private TextMeshProUGUI _txtPublicKey;
    private void Start()
    {
        _txtPublicKey = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        Web3.OnLogin += OnLogin;
    }

    private void OnDisable()
    {
        Web3.OnLogin -= OnLogin;
    }

    private void OnLogin(Account account)
    {
        _txtPublicKey.text = account.PublicKey;
        SceneManager.LoadScene("Shop");
    }
}