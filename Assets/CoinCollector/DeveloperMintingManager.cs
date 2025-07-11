using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Thirdweb;
using Thirdweb.Unity;
using System.Threading.Tasks; // For Task

public class DeveloperMintingManager : MonoBehaviour
{
    [Header("Thirdweb Configuration")]
    [SerializeField] private ulong ActiveChainId = 11155111; // Sepolia Testnet Chain ID
    [SerializeField] private string PlayerSoulContractAddress = "YOUR_SEPOLIA_CONTRACT_ADDRESS_HERE"; // <<< IMPORTANT: PASTE YOUR DEPLOYED SEPOLIA CONTRACT ADDRESS HERE
    [SerializeField] private TextAsset PlayerSoulContractABI; // <<< IMPORTANT: DRAG YOUR PlayerSoulABI.json HERE IN INSPECTOR

    [Header("UI References")]
    public Button connectDeveloperWalletButton;
    public TMP_Text developerAddressText;
    public TMP_Text developerBalanceText;
    public TMP_InputField playerWalletAddressInputField; // Input for player's address
    public TMP_InputField achievementURIInputField; // Input for achievement IPFS URI
    public Button mintAchievementButton; // Button to trigger minting
    public TMP_Text statusLogText; // For all status messages

    private IThirdwebWallet _connectedDeveloperWallet; // Stores the connected developer wallet
    private ThirdwebContract _playerSoulContract; // Stores the deployed contract instance

    void Start()
    {
        // Assign button listeners
        if (connectDeveloperWalletButton != null) connectDeveloperWalletButton.onClick.AddListener(OnConnectDeveloperWalletClicked);
        if (mintAchievementButton != null) mintAchievementButton.onClick.AddListener(OnMintAchievementClicked);

        // Initialize UI text
        developerAddressText.text = "Developer Wallet: Not Connected";
        developerBalanceText.text = "Balance: N/A";
        statusLogText.text = "";
    }

    // Connects the developer's wallet to Sepolia
    public async void OnConnectDeveloperWalletClicked()
    {
        statusLogText.text = "Connecting developer wallet...";
        try
        {
            // Ensure ThirdwebManager.Instance is available
            if (ThirdwebManager.Instance == null)
            {
                string errorMessage = "ThirdwebManager not found. Ensure it's in your scene and active.";
                Debug.LogError(errorMessage);
                statusLogText.text = $"Error: {errorMessage}";
                return;
            }

            // Determine wallet provider based on platform (MetaMask for WebGL, WalletConnect for Editor/Native)
            WalletProvider providerToUse;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                providerToUse = WalletProvider.MetaMaskWallet;
                Debug.Log("Attempting to connect with MetaMask (WebGL).");
            }
            else
            {
                providerToUse = WalletProvider.WalletConnectWallet;
                Debug.Log("Attempting to connect with WalletConnect (Editor/Native).");
            }

            // Connect the wallet
            _connectedDeveloperWallet = await ThirdwebManager.Instance.ConnectWallet(
                new WalletOptions(providerToUse, ActiveChainId)
            );

            // If connection is successful, update UI and initialize contract
            if (_connectedDeveloperWallet != null)
            {
                string address = await _connectedDeveloperWallet.GetAddress();
                developerAddressText.text = $"Developer Wallet: {address}";
                statusLogText.text = "Developer wallet connected successfully!";
                await GetDeveloperSepoliaBalance();
                await InitializePlayerSoulContract();
            }
            else
            {
                statusLogText.text = "Developer wallet connection failed.";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Developer wallet connection error: {e.Message}");
            statusLogText.text = $"Connection Error: {e.Message}";
            developerAddressText.text = "Developer Wallet: Error";
            developerBalanceText.text = "Balance: Error";
        }
    }

    // Fetches and displays the connected developer's Sepolia ETH balance
    private async Task GetDeveloperSepoliaBalance()
    {
        if (_connectedDeveloperWallet == null)
        {
            statusLogText.text = "Developer wallet not connected. Cannot get balance.";
            return;
        }

        statusLogText.text = "Fetching developer Sepolia balance...";
        try
        {
            var balance = await _connectedDeveloperWallet.GetBalance(chainId: ActiveChainId);
            string balanceEth = Utils.ToEth(balance.ToString(), 4);
            developerBalanceText.text = $"Balance: {balanceEth} ETH";
            statusLogText.text = "Developer Sepolia balance fetched.";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error getting developer Sepolia balance: {e.Message}");
            statusLogText.text = $"Balance Error: {e.Message}";
            developerBalanceText.text = "Balance: Error";
        }
    }

    // Initializes the PlayerSoul smart contract instance
    private async Task InitializePlayerSoulContract()
    {
        if (string.IsNullOrEmpty(PlayerSoulContractAddress))
        {
            Debug.LogError("PlayerSoulContractAddress is not set in the Inspector.");
            statusLogText.text = "Error: Contract address missing.";
            return;
        }
        if (PlayerSoulContractABI == null)
        {
            Debug.LogError("PlayerSoulContractABI is not assigned in the Inspector.");
            statusLogText.text = "Error: Contract ABI missing.";
            return;
        }

        try
        {
            statusLogText.text = "Initializing PlayerSoul contract...";
            // Use Thirdweb.Contract.Create static method as per your SDK's structure
            _playerSoulContract = await ThirdwebContract.Create(
                client: ThirdwebManager.Instance.Client,
                address: PlayerSoulContractAddress,
                chain: ActiveChainId,
                abi: PlayerSoulContractABI.text
            );
            statusLogText.text = "PlayerSoul contract initialized.";
            Debug.Log("PlayerSoul contract initialized successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to initialize PlayerSoul contract: {e.Message}");
            statusLogText.text = $"Contract Init Error: {e.Message}";
        }
    }

    // Mints an achievement to a specified player wallet
    public async void OnMintAchievementClicked()
    {
        if (_playerSoulContract == null || _connectedDeveloperWallet == null)
        {
            statusLogText.text = "Error: Connect developer wallet and initialize contract first.";
            Debug.LogError("Contract or Developer Wallet not initialized.");
            return;
        }

        string playerAddressToMintTo = playerWalletAddressInputField.text;
        string achievementURI = achievementURIInputField.text;

        // Basic validation for address and URI
        if (string.IsNullOrEmpty(playerAddressToMintTo) || !IsValidEvmAddress(playerAddressToMintTo))
        {
            statusLogText.text = "Error: Please enter a valid player wallet address.";
            Debug.LogError("Invalid player wallet address for minting.");
            return;
        }
        if (string.IsNullOrEmpty(achievementURI))
        {
            statusLogText.text = "Error: Please enter an Achievement IPFS URI.";
            Debug.LogError("Achievement URI cannot be empty for minting.");
            return;
        }

        try
        {
            statusLogText.text = $"Minting achievement for {playerAddressToMintTo.Substring(0, 6)}... (Check wallet for gas)";
            // Call the mintAchievement function (requires gas, paid by connected developer wallet)
            // Use Thirdweb.Contract.Write static method
            var result = await ThirdwebContract.Write(
                wallet: _connectedDeveloperWallet,
                contract: _playerSoulContract,
                method: "mintAchievement",
                weiValue: 0, // No ETH sent with the transaction
                parameters: new object[] { playerAddressToMintTo, achievementURI }
            );
            Debug.Log($"Mint Achievement Tx Hash: {result.TransactionHash}");
            statusLogText.text = $"Achievement Minted! Tx: {result.TransactionHash.Substring(0, 6)}...";
            // Optionally, clear input fields after successful mint
            playerWalletAddressInputField.text = "";
            achievementURIInputField.text = "";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Mint achievement failed: {e.Message}");
            statusLogText.text = $"Mint Failed: {e.Message}";
        }
    }

    // Helper method for basic EVM address validation
    private bool IsValidEvmAddress(string address)
    {
        if (string.IsNullOrEmpty(address)) return false;
        // Basic check: starts with 0x and is 42 characters long
        // More robust validation would involve Nethereum's AddressUtil.Current.IsValidEthereumAddress
        return address.StartsWith("0x") && address.Length == 42;
    }
}