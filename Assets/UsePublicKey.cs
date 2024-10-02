using TMPro;
using UnityEngine;

public class UsePublicKey : MonoBehaviour
{
    public TextMeshProUGUI PublicKeyText;

    private void Start()
    {
        string publicKey = PlayerPrefs.GetString("PublicKey", "Không có khóa công khai");
        PublicKeyText.text = "Public Key: " + publicKey;
    }
}