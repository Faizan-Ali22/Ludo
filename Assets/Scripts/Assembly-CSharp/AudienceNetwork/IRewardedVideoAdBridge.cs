namespace AudienceNetwork
{
	internal interface IRewardedVideoAdBridge
	{
		int Create(string placementId, RewardData rewardData, RewardedVideoAd rewardedVideoAd);

		int Load(int uniqueId);

		int Load(int uniqueId, string bidPayload);

		bool IsValid(int uniqueId);

		bool Show(int uniqueId);

		void SetExtraHints(int uniqueId, ExtraHints extraHints);

		void Release(int uniqueId);

		void OnLoad(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnImpression(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnClick(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnError(int uniqueId, FBRewardedVideoAdBridgeErrorCallback callback);

		void OnWillClose(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnDidClose(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnComplete(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnDidSucceed(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnDidFail(int uniqueId, FBRewardedVideoAdBridgeCallback callback);

		void OnActivityDestroyed(int uniqueId, FBRewardedVideoAdBridgeCallback callback);
	}
}
