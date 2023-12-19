using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SolanaExamples.Scripts
{
	public class SolanaAuthExample : MonoBehaviour
	{
		[SerializeField] private List<TabPage> _pages;
		[SerializeField] private Transform _logsParent;

		private void Awake()
		{
			OnTabButtonClicked("auth");

			foreach (TabPage tabPage in _pages)
			{
				Data.Instance.OnDataChanged += tabPage.OnRefresh;
				tabPage.OnLog = Log;
			}
		}

		public void OnTabButtonClicked(string selectedPage)
		{
			foreach (TabPage tabPage in _pages)
			{
				tabPage.SetVisible(tabPage.Page == selectedPage);
			}
		}

		private void Log(string message)
		{
#if UNITY_EDITOR
			Debug.Log(message);
#endif

#if (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
			TextMeshProUGUI log = new GameObject("LogEntry").AddComponent<TextMeshProUGUI>();
			log.text = $"{message}";
			log.transform.SetParent(_logsParent, false);
#endif
		}
	}
}