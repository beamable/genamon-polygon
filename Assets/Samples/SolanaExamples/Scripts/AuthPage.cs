using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Beamable;
using Beamable.Api.Auth;
using Beamable.Common;
using Beamable.Common.Api.Auth;
using Beamable.Player;
using Beamable.Server.Clients;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using Solana.Unity.Wallet.Bip39;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SolanaExamples.Scripts
{
    /// <summary>
    /// A script that presents how to perform basic operations like connecting to a wallet, attach or detach external identity
    /// or sign a message with connected wallet
    /// </summary>
    public class AuthPage : TabPage
    {
        [SerializeField] private Button _connectWalletButton;
        [SerializeField] private Button _attachIdentityButton;
        [SerializeField] private Button _detachIdentityButton;
        [SerializeField] private Button _getExternalIdentitiesButton;
        [SerializeField] private Button _signMessageButton;

        [SerializeField] private TextMeshProUGUI _beamId;
        [SerializeField] private TextMeshProUGUI _walletId;

        private IAuthService _authService;

        private async void Start()
        {
            Data.Instance.Working = true;
            
            _connectWalletButton.onClick.AddListener(OnConnectClicked);
            _attachIdentityButton.onClick.AddListener(OnAttachClicked);
            _detachIdentityButton.onClick.AddListener(OnDetachClicked);
            _getExternalIdentitiesButton.onClick.AddListener(OnGetExternalClicked);
            _signMessageButton.onClick.AddListener(OnSignClicked);

            _authService = Ctx.Api.AuthService;

            await Ctx.OnReady;
            await Ctx.Accounts.OnReady;

            _beamId.text = $"<b>Beam ID</b> {Ctx.Accounts.Current.GamerTag.ToString()}";

            Data.Instance.Working = false;
        }

        public override void OnRefresh()
        {
            _connectWalletButton.interactable = !Data.Instance.Working && !Data.Instance.WalletConnected;
            _attachIdentityButton.interactable = !Data.Instance.Working && Data.Instance.WalletConnected &&
                                                 !Data.Instance.WalletAttached;
            _detachIdentityButton.interactable = !Data.Instance.Working && Data.Instance.WalletConnected &&
                                                 Data.Instance.WalletAttached;
            _getExternalIdentitiesButton.interactable = !Data.Instance.Working;
            _signMessageButton.interactable = !Data.Instance.Working && Data.Instance.WalletConnected;

            _walletId.text = Data.Instance.WalletConnected
                ? $"<b>Wallet Id</b> {Data.Instance.Account.PublicKey.Key}"
                : String.Empty;
        }

        private async void OnConnectClicked()
        {
            Data.Instance.Working = true;

            OnLog("Connecting to a wallet...");
            await Login();

            if (Data.Instance.WalletConnected)
            {
                Data.Instance.WalletAttached = CheckIfWalletHasAttachedIdentity();
            }

            Data.Instance.Working = false;
        }

        private async void OnAttachClicked()
        {
            Data.Instance.Working = true;
            OnLog("Attaching wallet...");
            await SendAttachRequest();
            Data.Instance.WalletAttached = CheckIfWalletHasAttachedIdentity();
            Data.Instance.Working = false;

            async Promise SendAttachRequest(ChallengeSolution challengeSolution = null)
            {
                StringBuilder builder = new();
                builder.AppendLine("Sending a request with:");
                builder.AppendLine($"Public key: {Data.Instance.Account.PublicKey.Key}");
                builder.AppendLine($"Provider service: {Data.Instance.Federation.Service}");
                if (challengeSolution != null)
                {
                    builder.AppendLine($"Signed solution: {challengeSolution.solution}");
                }

                OnLog(builder.ToString());

                RegistrationResult result =
                    await Ctx.Accounts.AddExternalIdentity<SolanaCloudIdentity, SolanaFederationClient>(
                        Data.Instance.Account.PublicKey.Key, SolveChallenge);

                if (result.isSuccess)
                {
                    OnLog("Succesfully attached an external identity...");
                }
            }
        }

        private async void OnDetachClicked()
        {
            Data.Instance.Working = true;
            OnLog("Detaching wallet...");
            await Ctx.Accounts.RemoveExternalIdentity<SolanaCloudIdentity, SolanaFederationClient>();
            Data.Instance.WalletAttached = CheckIfWalletHasAttachedIdentity();

            if (!Data.Instance.WalletAttached)
            {
                OnLog("Succesfully detached an external identity...");
            }

            Data.Instance.Working = false;
        }

        /// <summary>
        /// Method that shows how to sign a message with connected wallet 
        /// </summary>
        private async void OnSignClicked()
        {
            Data.Instance.Working = true;
            OnLog("Signing a message...");

            string message = "Sample message to sign";

            // Currently connected wallet is responsible for signing passed challenge. InGameWallet (in editor) is 
            // handling this automatically. PhantomWallet (mobile and WebGL) connects either with mobile app or browser
            // extension.
            byte[] signatureBytes = await Data.Instance.Wallet.SignMessage(Encoding.UTF8.GetBytes(message));

            // Signature signed by a wallet should be converted back to Base64String as that's the format that server is
            // waiting for
            string signedSignature = Convert.ToBase64String(signatureBytes);
            OnLog($"Signed signature: {signedSignature}");
            Data.Instance.Working = false;
        }

        /// <summary>
        /// Method that renders currently connected to account external identities where Service is a microservice responsible
        /// for handling custom server side logic, Namespace shows which namespace will be handled (namespaces can be implemented
        /// by deriving IThirdPartyCloudIdentity interface and Public Key is a wallet address that has been connected to an
        /// account
        /// </summary>
        private void OnGetExternalClicked()
        {
            OnLog("Gettting external identities info...");
            if (Ctx.Accounts.Current == null) return;

            if (Ctx.Accounts.Current.ExternalIdentities.Length != 0)
            {
                StringBuilder builder = new();
                foreach (ExternalIdentity identity in Ctx.Accounts.Current.ExternalIdentities)
                {
                    builder.AppendLine(
                        $"Service: {identity.providerService}, namespace: {identity.providerNamespace}, public key: {identity.userId}");
                }

                OnLog(builder.ToString());
            }
            else
            {
                OnLog("No external identities found...");
            }
        }

        /// <summary>
        /// Method that shows a way to solve a challenge received from a server. It needs to be done to proof that we
        /// are true owners of a wallet. After sending it back to a server it verifies it an decides wheter solution was
        /// correct or not. Challenge token we are receving from server is a three-part, dot separated string and has
        /// following format: {challenge}.{validUntilEpoch}.{signature} where:
        ///		{challenge}			- Base64 encoded string
        ///		{validUntilEpoch}	- valid until epoch time in milliseconds, Int64 value
        ///		{signature}			- Base64 encoded token signature
        /// </summary>
        /// <param name="challengeToken"></param>
        /// <returns></returns>
        private async Promise<string> SolveChallenge(string challengeToken)
        {
            OnLog($"Signing a challenge: {challengeToken}");

            // Parsing received challenge token to a 3 part struct
            ChallengeToken parsedToken = _authService.ParseChallengeToken(challengeToken);

            // Challenge we received to solve is Base64String 
            byte[] challengeBytes = Convert.FromBase64String(parsedToken.challenge);

            // Currently connected wallet is responsible for signing passed challenge. InGameWallet (in editor) is 
            // handling this automatically. PhantomWallet (mobile and WebGL) connects either with mobile app or browser
            // extension.
            byte[] signatureBytes = await Data.Instance.Wallet.SignMessage(challengeBytes);

            // Signature signed by a wallet should be converted back to Base64String as that's the format that server is
            // waiting for
            string signedSignature = Convert.ToBase64String(signatureBytes);

            OnLog($"Signed signature: {signedSignature}");

            return signedSignature;
        }

        private bool CheckIfWalletHasAttachedIdentity()
        {
            if (Ctx.Accounts.Current == null)
                return false;

            if (Ctx.Accounts.Current.ExternalIdentities.Length == 0)
                return false;

            ExternalIdentity externalIdentity = Ctx.Accounts.Current.ExternalIdentities.FirstOrDefault(i =>
                i.providerNamespace == Data.Instance.Federation.Namespace &&
                i.providerService == Data.Instance.Federation.Service &&
                i.userId == Data.Instance.Account.PublicKey);

            return externalIdentity != null;
        }

        public async Task Login()
        {
#if UNITY_EDITOR
            Data.Instance.Account = await LoginInGame();
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
		    Data.Instance.Account = await LoginPhantom();
#endif
          

            OnLog(Data.Instance.Account != null
                ? $"Wallet connected with PublicKey: {Data.Instance.Account.PublicKey.Key}"
                : "Something gone wrong while connecting with a wallet");
        }

        private async Task<Account> LoginInGame()
        {
            // InGameWallet class is used for editor operations. It automatically approves all messages and transactions.
            Data.Instance.Wallet = new InGameWallet(RpcCluster.DevNet, null, true);

            string pass = Ctx.Accounts.Current.GamerTag.ToString();
            
            // We are retrieving local wallet or creating a new one if none was found
            return await Data.Instance.Wallet.Login(pass) ??
                   await Data.Instance.Wallet.CreateAccount(new Mnemonic(WordList.English, WordCount.Twelve).ToString(),
                       pass);
        }

        private async Task<Account> LoginPhantom()
        {
            // PhantomWallet class is used for clients built for Android, iOS and WebGL. It handles communication with
            // Phantom Wallet app installed on mobile device and Phantom Wallet browser extensions installed on desktop.
            Data.Instance.Wallet =
                new PhantomWallet(Data.Instance.WalletOptions, RpcCluster.DevNet, string.Empty, true);
            return await Data.Instance.Wallet.Login();
        }
    }
}