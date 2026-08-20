namespace AudienceNetwork
{
	internal interface IAdViewBridge
	{
		int Create(string placementId, AdView adView, AdSize size);

		int Load(int uniqueId);

		int Load(int uniqueId, string bidPayload);

		bool IsValid(int uniqueId);

		bool Show(int uniqueId, double x, double y, double width, double height);

		void SetExtraHints(int uniqueId, ExtraHints extraHints);

		void Release(int uniqueId);

		void OnLoad(int uniqueId, FBAdViewBridgeCallback callback);

		void OnImpression(int uniqueId, FBAdViewBridgeCallback callback);

		void OnClick(int uniqueId, FBAdViewBridgeCallback callback);

		void OnError(int uniqueId, FBAdViewBridgeErrorCallback callback);

		void OnFinishedClick(int uniqueId, FBAdViewBridgeCallback callback);
	}
}
