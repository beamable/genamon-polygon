using System;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Solana.Unity.SDK;
using Solana.Unity.Wallet;
using UnityEngine;

namespace SolanaExamples.Scripts
{
	/// <summary>
	/// A script that holds the data for connected wallet and account as well as data necessary to general functioning
	/// of the example
	/// </summary>
	public class Data : MonoBehaviour
	{
		public event Action OnDataChanged;

		[SerializeField] private Federation _federation;
		[SerializeField] private CurrencyRef _currencyRef;
		[SerializeField] private ItemRef _itemRef;

		#region Auto properties

		public static Data Instance { get; private set; }
		public WalletBase Wallet { get; set; }
		public PhantomWalletOptions WalletOptions { get; } = new() { appMetaDataUrl = "https://beamable.com" };

		#endregion

		#region Properties

		private Account _account;

		public Account Account
		{
			get => _account;
			set
			{
				_account = value;
				OnDataChanged?.Invoke();
			}
		}

		private bool _working;

		public bool Working
		{
			get => _working;
			set
			{
				_working = value;
				OnDataChanged?.Invoke();
			}
		}


		private bool _walletAttached;

		public bool WalletAttached
		{
			get => _walletAttached;
			set
			{
				_walletAttached = value;
				OnDataChanged?.Invoke();
			}
		}

		#endregion

		#region Property getters

		public bool WalletConnected => Account != null;
		public Federation Federation => _federation;
		public CurrencyRef CurrencyRef => _currencyRef;
		public ItemRef ItemRef => _itemRef;
		
		#endregion

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
			}
		}

		private void Start()
		{
			OnDataChanged?.Invoke();
		}
	}
}