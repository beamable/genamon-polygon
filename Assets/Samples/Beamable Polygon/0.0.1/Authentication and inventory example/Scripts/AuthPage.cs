using System;
using System.Linq;
using System.Text;
using Beamable;
using Beamable.Api.Auth;
using Beamable.Common;
using Beamable.Common.Api.Auth;
using Beamable.Player;
using Beamable.Polygon.Common;
using Beamable.Server.Clients;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PolygonExamples.Scripts
{
    /// <summary>
    /// A script that presents how to perform basic operations like connecting to a wallet, attach or detach external identity
    /// or sign a message with connected wallet
    /// </summary>
    public class AuthPage : TabPage
    {
        // [SerializeField] private Button _connectWalletButton;
        [SerializeField] private Button _attachIdentityButton;
        [SerializeField] private Button _detachIdentityButton;
        [SerializeField] private Button _getExternalIdentitiesButton;

        [SerializeField] private TextMeshProUGUI _beamId;
        [SerializeField] private TextMeshProUGUI _walletId;

        private IAuthService _authService;

        private async void Start()
        {
            
            _attachIdentityButton.onClick.AddListener(OnAttachClicked);
            _detachIdentityButton.onClick.AddListener(OnDetachClicked);
            _getExternalIdentitiesButton.onClick.AddListener(OnGetExternalClicked);

            _authService = Ctx.Api.AuthService;

            await BeamContext.Default.OnReady;
            await BeamContext.Default.Accounts.OnReady;

            var external = BeamContext.Default.Accounts.Current.ExternalIdentities.FirstOrDefault(ext =>
                ext.providerNamespace == Ctx.GetExampleData().Federation.Namespace
                && ext.providerService == Ctx.GetExampleData().Federation.Service);
            Ctx.GetExampleData().WalletId = external?.userId ?? null;
            
            _beamId.text = $"<b>Beam ID</b> {Ctx.Accounts.Current.GamerTag.ToString()}";
            OnRefresh();
        }

        public override void OnRefresh()
        {
            _attachIdentityButton.interactable = !Ctx.GetExampleData().Working && !Ctx.GetExampleData().WalletConnected;
            _detachIdentityButton.interactable = !Ctx.GetExampleData().Working && Ctx.GetExampleData().WalletConnected;
            _getExternalIdentitiesButton.interactable = !Ctx.GetExampleData().Working;
            UpdateWalletIdText();
        }

        void UpdateWalletIdText()
        {
            _walletId.text = Ctx.GetExampleData().WalletConnected
                ? $"<b>Wallet Id</b> {Ctx.GetExampleData().WalletId}"
                : String.Empty;
        }


        private async void OnAttachClicked()
        {
            Ctx.GetExampleData().Working = true;
            OnLog("Attaching wallet...");
            await SendAttachRequest();
            CheckIfWalletHasAttachedIdentity();
            Ctx.GetExampleData().Working = false;

            async Promise SendAttachRequest(ChallengeSolution challengeSolution = null)
            {
                StringBuilder builder = new();
                builder.AppendLine("Sending a request with:");
                builder.AppendLine($"Provider service: {Ctx.GetExampleData().Federation.Service}");
                if (challengeSolution != null)
                {
                    builder.AppendLine($"Signed solution: {challengeSolution.solution}");
                }

                OnLog(builder.ToString());

                var emptyToken = "";
                RegistrationResult result =
                    await Ctx.Accounts.AddExternalIdentity<PolygonCloudIdentity, PolygonFederationClient>(emptyToken);

                var publicKey = result.account.ExternalIdentities[0].userId; // aka- walletId
                
                if (result.isSuccess)
                {
                    Ctx.GetExampleData().WalletId = publicKey;

                    OnLog($"Succesfully attached an external identity... publicKey=[{publicKey}]");
                }
            }
        }

        private async void OnDetachClicked()
        {
            Ctx.GetExampleData().Working = true;
            OnLog("Detaching wallet...");
            await Ctx.Accounts.RemoveExternalIdentity<PolygonCloudIdentity, PolygonFederationClient>();
            
            if (!CheckIfWalletHasAttachedIdentity())
            {
                OnLog("Succesfully detached an external identity...");
            }

            Ctx.GetExampleData().WalletId = null;

            Ctx.GetExampleData().Working = false;
            OnRefresh();
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

        private bool CheckIfWalletHasAttachedIdentity()
        {
            if (Ctx.Accounts.Current == null)
                return false;

            if (Ctx.Accounts.Current.ExternalIdentities.Length == 0)
                return false;

            ExternalIdentity externalIdentity = Ctx.Accounts.Current.ExternalIdentities.FirstOrDefault(i =>
                i.providerNamespace == Ctx.GetExampleData().Federation.Namespace &&
                i.providerService == Ctx.GetExampleData().Federation.Service &&
                i.userId == Ctx.GetExampleData().WalletId);

            return externalIdentity != null;
        }
    }
}