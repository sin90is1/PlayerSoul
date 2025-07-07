using UnityEngine;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro
using Thirdweb; // Required for Thirdweb SDK
using Thirdweb.Unity;


public class SepoliaTestManager : MonoBehaviour
{
    [Header("UI References")]
    public Button connectWalletButton;
    public TMP_Text addressText;
    public TMP_Text balanceText;
    public TMP_Text logText; // For general messages

    // Sepolia Chain ID (11155111)
    private const ulong SEPOLIA_CHAIN_ID = 11155111;

    // Corrected: Use IThirdwebWallet interface
    private IThirdwebWallet _connectedWallet; // Stores the connected wallet instance

    void Start()
    {
        // Assign button listener
        if (connectWalletButton != null)
        {
            connectWalletButton.onClick.AddListener(OnConnectWalletClicked);
        }
        else
        {
            Debug.LogError("Connect Wallet Button not assigned! Please assign it in the Inspector.");
        }

        // Initialize UI text
        addressText.text = "Address: Not Connected";
        balanceText.text = "Balance: N/A";
        logText.text = "";
    }

    // This method is called when the Connect Wallet button is clicked
    public async void OnConnectWalletClicked()
    {
        logText.text = "Connecting wallet...";
        try
        {
            // --- IMPORTANT CHECK ---
            // Ensure ThirdwebManager.Instance is not null before proceeding
            if (ThirdwebManager.Instance == null)
            {
                string errorMessage = "ThirdwebManager.Instance is null. Please ensure you have a GameObject in your scene with the 'ThirdwebManager' component attached and that it is active/enabled.";
                Debug.LogError(errorMessage);
                logText.text = $"Error: {errorMessage}";
                return; // Stop execution if manager is not found
            }
            // --- END IMPORTANT CHECK ---


            WalletProvider providerToUse;

            // Determine wallet provider based on platform
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // For WebGL builds, use MetaMask (browser extension)
                providerToUse = WalletProvider.MetaMaskWallet;
                Debug.Log("Attempting to connect with MetaMask (WebGL).");
            }
            else
            {
                // For Editor and native builds (Windows, Mac, Android, iOS), use WalletConnect
                // This will typically open a QR code for scanning with a mobile wallet like MetaMask Mobile
                providerToUse = WalletProvider.WalletConnectWallet;
                Debug.Log("Attempting to connect with WalletConnect (Editor/Native).");
            }

            // Connect the wallet to the Sepolia testnet
            _connectedWallet = await ThirdwebManager.Instance.ConnectWallet(
                new WalletOptions(providerToUse, SEPOLIA_CHAIN_ID)
            );

            // If connection is successful, update UI
            if (_connectedWallet != null)
            {
                string address = await _connectedWallet.GetAddress();
                addressText.text = $"Address: {address}";
                logText.text = "Wallet connected successfully!";

                // Now, get the balance specifically for the Sepolia chain
                await GetSepoliaBalance();
            }
            else
            {
                logText.text = "Wallet connection failed.";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Wallet connection error: {e.Message}");
            logText.text = $"Connection Error: {e.Message}";
            addressText.text = "Address: Error";
            balanceText.text = "Balance: Error";
        }
    }

    // This method fetches and displays the Sepolia balance
    private async System.Threading.Tasks.Task GetSepoliaBalance()
    {
        if (_connectedWallet == null)
        {
            logText.text = "Wallet not connected. Cannot get balance.";
            return;
        }

        logText.text = "Fetching Sepolia balance...";
        try
        {
            // GetBalance takes an optional chainId parameter.
            // Explicitly requesting balance for SEPOLIA_CHAIN_ID (11155111)
            var balance = await _connectedWallet.GetBalance(chainId: SEPOLIA_CHAIN_ID);

            // Convert wei to ETH and display
            string balanceEth = Utils.ToEth(balance.ToString(), 4); // Display with 4 decimal places
            balanceText.text = $"Sepolia ETH: {balanceEth}";
            logText.text = "Sepolia balance fetched.";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting Sepolia balance: {e.Message}");
            logText.text = $"Balance Error: {e.Message}";
            balanceText.text = "Balance: Error";
        }
    }
}
