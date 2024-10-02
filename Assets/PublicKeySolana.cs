using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePublicKeySolana : MonoBehaviour
{
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
        PlayerPrefs.SetString("PublicKey", account.PublicKey);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Shop");
    }
}