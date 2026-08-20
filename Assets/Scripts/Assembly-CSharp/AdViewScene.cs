using System;
using AudienceNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AdViewScene : BaseScene
{
	private AdView adView;

	private AdPosition currentAdViewPosition;

	private ScreenOrientation currentScreenOrientation;

	public Text statusLabel;

	private AdSize[] adSizeArray = (AdSize[])Enum.GetValues(typeof(AdSize));

	private int currentAdSize;

	public Button loadAdButton;

	private void OnDestroy()
	{
		if ((bool)adView)
		{
			adView.Dispose();
		}
		DConsole.Log("AdViewTest was destroyed!");
	}

	private void Awake()
	{
		AudienceNetworkAds.Initialize();
		SetLoadAddButtonText();
		SettingsScene.InitializeSettings();
	}

	private void SetLoadAddButtonText()
	{
		loadAdButton.GetComponentInChildren<Text>().text = "Load Banner (" + adSizeArray[currentAdSize].ToString() + ")";
	}

	public void LoadBanner()
	{
		if ((bool)adView)
		{
			adView.Dispose();
		}
		statusLabel.text = "Loading Banner...";
		adView = new AdView("YOUR_PLACEMENT_ID", adSizeArray[currentAdSize]);
		adView.Register(base.gameObject);
		currentAdViewPosition = AdPosition.CUSTOM;
		adView.AdViewDidLoad = delegate
		{
			currentScreenOrientation = Screen.orientation;
			adView.Show(100.0);
			string text = (adView.IsValid() ? "valid" : "invalid");
			statusLabel.text = "Banner loaded and is " + text + ".";
			DConsole.Log("Banner loaded");
		};
		adView.AdViewDidFailWithError = delegate(string error)
		{
			statusLabel.text = "Banner failed to load with error: " + error;
			DConsole.Log("Banner failed to load with error: " + error);
		};
		adView.AdViewWillLogImpression = delegate
		{
			statusLabel.text = "Banner logged impression.";
			DConsole.Log("Banner logged impression.");
		};
		adView.AdViewDidClick = delegate
		{
			statusLabel.text = "Banner clicked.";
			DConsole.Log("Banner clicked.");
		};
		adView.LoadAd();
	}

	public void ChangeBannerSize()
	{
		currentAdSize++;
		currentAdSize %= adSizeArray.Length;
		SetLoadAddButtonText();
	}

	public void NextScene()
	{
		SceneManager.LoadScene("RewardedVideoAdScene");
	}

	public void ChangePosition()
	{
		switch (currentAdViewPosition)
		{
		case AdPosition.TOP:
			SetAdViewPosition(AdPosition.BOTTOM);
			break;
		case AdPosition.BOTTOM:
			SetAdViewPosition(AdPosition.CUSTOM);
			break;
		case AdPosition.CUSTOM:
			SetAdViewPosition(AdPosition.TOP);
			break;
		}
	}

	private void OnRectTransformDimensionsChange()
	{
		if ((bool)adView && Screen.orientation != currentScreenOrientation)
		{
			SetAdViewPosition(currentAdViewPosition);
			currentScreenOrientation = Screen.orientation;
		}
	}

	private void SetAdViewPosition(AdPosition adPosition)
	{
		switch (adPosition)
		{
		case AdPosition.TOP:
			adView.Show(AdPosition.TOP);
			currentAdViewPosition = AdPosition.TOP;
			break;
		case AdPosition.BOTTOM:
			adView.Show(AdPosition.BOTTOM);
			currentAdViewPosition = AdPosition.BOTTOM;
			break;
		case AdPosition.CUSTOM:
			adView.Show(100.0);
			currentAdViewPosition = AdPosition.CUSTOM;
			break;
		}
	}
}
