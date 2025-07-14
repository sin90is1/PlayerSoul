using UnityEngine;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro
using Thirdweb;
using Thirdweb.Unity;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PlayerSoulMinter : MonoBehaviour
{
    public Button connectDeveloperWalletButton;
    public TMP_Text developerAddressText;
    public TMP_Text developerBalanceText;



    [Header("Thirdweb Configuration")]
    [SerializeField] private ulong chainId = 11155111; // Sepolia
    [SerializeField] private string contractAddress = "YOUR_CONTRACT_ADDRESS";
    [SerializeField] private TextAsset contractABI; // Drag your ABI JSON file here in the inspector

    private IThirdwebWallet developerWallet;
    private ThirdwebContract playerSoulContract;

    void Start()
    {
        connectDeveloperWalletButton.onClick.AddListener(() => ConnectDeveloperWalletHandler());
    }
    public async void ConnectDeveloperWalletHandler()
    {
        bool success = await ConnectDeveloperWallet();
        Debug.Log($"Developer wallet connection success: {success}");
    }
    public async Task<bool> ConnectDeveloperWallet()
    {
        if (ThirdwebManager.Instance == null)
        {
            Debug.LogError("ThirdwebManager not found.");
            return false;
        }

        WalletProvider provider = Application.platform == RuntimePlatform.WebGLPlayer
            ? WalletProvider.MetaMaskWallet
            : WalletProvider.WalletConnectWallet;


        developerWallet = await ThirdwebManager.Instance.ConnectWallet(new WalletOptions(provider, chainId));

        if (developerWallet == null)
        {
            Debug.LogError("Wallet connection failed.");
            return false;
        }

        ///////////////for debug
        string address = await developerWallet.GetAddress();
        developerAddressText.text = $"Dev Wallet: {address}";
        await ShowDeveloperBalance();

        //////////////


        playerSoulContract = await ThirdwebContract.Create(
            ThirdwebManager.Instance.Client,
            contractAddress,
            chainId,
            contractABI.text
        );

        return true;
    }

    public bool IsReady()
    {
        return developerWallet != null && playerSoulContract != null;
    }


    // This is the only function other scripts need to call
    public async Task<string> MintAchievement(string playerAddress, string ipfsUri)
    {
        Debug.Log($"MintAchievement function was called");

        if (developerWallet == null || playerSoulContract == null)
        {
            Debug.LogError("Wallet or contract not connected.");
            return null;
        }

        var result = await ThirdwebContract.Write(
            developerWallet,
            playerSoulContract,
            "mintAchievement",
            0,
            new object[] { playerAddress, ipfsUri }
        );

        Debug.Log($"Minted Achievement. Tx: {result.TransactionHash}");
        return result.TransactionHash;
    }

    public async Task<List<(string issuer, string uri)>> GetAchievementsForPlayer(string playerAddress)
    {
        if (developerWallet == null || playerSoulContract == null)
        {
            Debug.LogError("Wallet or contract not connected.");
            return null;
        }

        try
        {
            var result = await ThirdwebContract.Read<object[][]>(
                playerSoulContract,
                "getAchievementsForPlayer",
                new object[] { playerAddress }
            );

            List<(string, string)> achievements = new List<(string, string)>();

            foreach (object[] tuple in result)
            {
                string issuer = tuple[0]?.ToString();
                string uri = tuple[1]?.ToString();
                achievements.Add((issuer, uri));
            }

            return achievements;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error fetching achievements: {e.Message}");
            return null;
        }
    }

    ///////////////for debug
    private async System.Threading.Tasks.Task ShowDeveloperBalance()
    {
        if (developerWallet == null) return;

        var balance = await developerWallet.GetBalance(chainId: chainId);
        string balanceEth = Utils.ToEth(balance.ToString(), 4);
        developerBalanceText.text = $"Dev ETH: {balanceEth}";

    }

}
