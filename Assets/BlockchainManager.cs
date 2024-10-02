using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using Solana.Unity.SDK;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Wallet;
using Solana.Unity.Programs;
using Solana.Unity.Metaplex.NFT.Library;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Rpc.Builders;
using Solana.Unity.Metaplex.Utilities;
using Solana.Unity.Rpc.Models;

public class BlockchainManager : MonoBehaviour
{
    public string Address { get; private set; }

    public Button playButton;
    public Button shopButton;

    //Game Shop
    public Button freeNFTButton;
    public Button coin500Button;
    public Button coin5000Button;
    public Button coin10000Button;
    public Button backButton;

    public TextMeshProUGUI coinBoughtText;
    public TextMeshProUGUI buyingStatusText;

    public GameObject gameShopPanel;

    private const long SolLamports = 1000000000;

    public TMP_Text solanaPublicKeyText;
    public TMP_Text solanaBalanceText;

    private void Start()
    {
        playButton.gameObject.SetActive(true);
        shopButton.gameObject.SetActive(true);
        gameShopPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Logo");
    }

    private void HandleResponse(RequestResult<string> result)
    {
        Debug.Log(result.Result == null ? result.Reason : "");
    }

    public async void SpendTokenToBuyCoins(int indexValue)
    {

        Double _ownedSolAmount = await Web3.Instance.WalletBase.GetBalance();

        if (_ownedSolAmount <= 0)
        {
            buyingStatusText.text = "Not Enough SOL";
            return;
        }

        freeNFTButton.interactable = false;
        coin500Button.interactable = false;
        coin5000Button.interactable = false;
        coin10000Button.interactable = false;
        backButton.interactable = false;

        float costValue = 0f;
        buyingStatusText.text = "Buying...";
        buyingStatusText.gameObject.SetActive(true);
        if (indexValue == 0)
        {
            costValue = 0.02f;
        }
        else if (indexValue == 1)
        {
            costValue = 0.1f;
        }
        else if (indexValue == 2)
        {
            costValue = 0.15f;
        }
        try
        {
            // Thực hiện chuyển tiền, nếu thành công thì tiếp tục xử lý giao diện
            RequestResult<string> result = await Web3.Instance.WalletBase.Transfer(
               new PublicKey("Hw1VoYsnB7kX5h4nZiczEndj6mMF3i7DZR5Q2Ng1JiM4"),
               Convert.ToUInt64(costValue * SolLamports));
            HandleResponse(result);

            // Chỉ thực hiện các thay đổi giao diện nếu chuyển tiền thành công

            freeNFTButton.interactable = true;
            coin500Button.interactable = true;
            coin5000Button.interactable = true;
            coin10000Button.interactable = true;
            backButton.interactable = true;


            if (indexValue == 0)
            {
                buyingStatusText.text = "+500 Coins";
                coin500Button.gameObject.SetActive(false);
                ResourceBoost.Instance.coins += 500;
            }
            else if (indexValue == 1)
            {
                buyingStatusText.text = "+5,000 Coins";
                coin5000Button.gameObject.SetActive(false);
                ResourceBoost.Instance.coins += 5000;
            }
            else if (indexValue == 2)
            {
                buyingStatusText.text = "+10,000 Coins";
                coin10000Button.gameObject.SetActive(false);
                ResourceBoost.Instance.coins += 10000;
            }

            coinBoughtText.text = "Coin Bought: " + ResourceBoost.Instance.coins.ToString();

        }
        catch (Exception ex)
        {
            // Xử lý ngoại lệ nếu có lỗi xảy ra
            Debug.LogError($"Lỗi khi thực hiện chuyển tiền: {ex.Message}");
        }
    }

    public async void ClaimFreeNFT()
    {
        // Mint and ATA
        var mint = new Account();
        var associatedTokenAccount = AssociatedTokenAccountProgram
            .DeriveAssociatedTokenAccount(Web3.Account, mint.PublicKey);

        // Define the metadata
        var metadata = new Metadata()
        {
            name = "Solana Unity SDK NFT",
            symbol = "MGCK",
            uri = "https://gateway.pinata.cloud/ipfs/QmQNSeusMHgfZvVjsD5DYsujng2t379kkD5pukGKiWMwzQ",
            //uri = "https://y5fi7acw5f5r4gu6ixcsnxs6bhceujz4ijihcebjly3zv3lcoqkq.arweave.net/x0qPgFbpex4ankXFJt5eCcRKJzxCUHEQKV43mu1idBU",
            sellerFeeBasisPoints = 0,
            creators = new List<Creator> { new(Web3.Account.PublicKey, 100, true) }
        };


        // Bước 1: Lấy các tài khoản token liên kết cho người dùng
        var tokenAccounts = await Web3.Wallet.GetTokenAccounts(Commitment.Processed);

        // Bước 2: Khởi tạo biến đếm cho các NFT phù hợp
        int matchingNftCount = 0;

        // Bước 3: Lặp qua từng tài khoản token
        foreach (var item in tokenAccounts)
        {
            var loadTask = Nft.TryGetNftData(item.Account.Data.Parsed.Info.Mint,
                                Web3.Instance.WalletBase.ActiveRpcClient, commitment: Commitment.Processed);
            // Chờ tác vụ hoàn thành và lấy kết quả
            var nftData = await loadTask;

            // Ghi lại thông tin vào log
            if (nftData != null)
            {
                Debug.Log($"NFT Mint: {nftData}");
                string textValue = nftData.metaplexData?.data?.offchainData?.name;
                Debug.Log("textValue: " + textValue);
                if (textValue == "TestNFT")
                {
                    matchingNftCount += 1;
                    break;
                }
            }
            else
            {
                Debug.Log($"Không tìm thấy dữ liệu NFT cho Mint: {item.Account.Data.Parsed.Info.Mint}");
            }
        }

        if (matchingNftCount >= 1)
        {
            buyingStatusText.text = "You already own this NFT.";
            freeNFTButton.gameObject.SetActive(false);
            coin500Button.interactable = true;
            coin5000Button.interactable = true;
            coin10000Button.interactable = true;
            return;
        }

        freeNFTButton.interactable = false;
        coin500Button.interactable = false;
        coin5000Button.interactable = false;
        coin10000Button.interactable = false;
        backButton.interactable = false;

        buyingStatusText.text = "Claiming...";
        buyingStatusText.gameObject.SetActive(true);

        // Prepare the transaction
        var blockHash = await Web3.Rpc.GetLatestBlockHashAsync();
        var minimumRent = await Web3.Rpc.GetMinimumBalanceForRentExemptionAsync(TokenProgram.MintAccountDataSize);
        var transaction = new TransactionBuilder()
            .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
            .SetFeePayer(Web3.Account)
            .AddInstruction(
                SystemProgram.CreateAccount(
                    Web3.Account,
                    mint.PublicKey,
                    minimumRent.Result,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
            .AddInstruction(
                TokenProgram.InitializeMint(
                    mint.PublicKey,
                    0,
                    Web3.Account,
                    Web3.Account))
            .AddInstruction(
                AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                    Web3.Account,
                    Web3.Account,
                    mint.PublicKey))
            .AddInstruction(
                TokenProgram.MintTo(
                    mint.PublicKey,
                    associatedTokenAccount,
                    1,
                    Web3.Account))
            .AddInstruction(MetadataProgram.CreateMetadataAccount(
                PDALookup.FindMetadataPDA(mint),
                mint.PublicKey,
                Web3.Account,
                Web3.Account,
                Web3.Account.PublicKey,
                metadata,
                TokenStandard.NonFungible,
                true,
                true,
                null,
                metadataVersion: MetadataVersion.V3))
            .AddInstruction(MetadataProgram.CreateMasterEdition(
                    maxSupply: null,
                    masterEditionKey: PDALookup.FindMasterEditionPDA(mint),
                    mintKey: mint,
                    updateAuthorityKey: Web3.Account,
                    mintAuthority: Web3.Account,
                    payer: Web3.Account,
                    metadataKey: PDALookup.FindMetadataPDA(mint),
                    version: CreateMasterEditionVersion.V3
                )
            );
        var tx = Transaction.Deserialize(transaction.Build(new List<Account> { Web3.Account, mint }));

        // Sign and Send the transaction
        try
        {
            var res = await Web3.Wallet.SignAndSendTransaction(tx);
            // Show Confirmation
            if (res?.Result != null)
            {
                await Web3.Rpc.ConfirmTransaction(res.Result, Commitment.Confirmed);
                Debug.Log("Minting succeeded, see transaction at https://explorer.solana.com/tx/"
                          + res.Result + "?cluster=" + Web3.Wallet.RpcCluster.ToString().ToLower());
            }
            buyingStatusText.text = "NFT Claimed";
            freeNFTButton.gameObject.SetActive(false);
            coin500Button.interactable = true;
            coin5000Button.interactable = true;
            coin10000Button.interactable = true;
            backButton.interactable = true;
        }
        catch (Exception ex)
        {
            buyingStatusText.text = $"Failed to claim NFT: {ex.Message}";
            Debug.LogError("Error while claiming NFT: " + ex);
            freeNFTButton.interactable = true;
            coin500Button.interactable = true;
            coin5000Button.interactable = true;
            coin10000Button.interactable = true;
            backButton.interactable = true;
        }
    }
}


