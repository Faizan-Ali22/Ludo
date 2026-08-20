namespace AudienceNetwork.Utility
{
	internal interface IAdUtilityBridge
	{
		double DeviceWidth();

		double DeviceHeight();

		double Width();

		double Height();

		double Convert(double deviceSize);

		void Prepare();
	}
}
