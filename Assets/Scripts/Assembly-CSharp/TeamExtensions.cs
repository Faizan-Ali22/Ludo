using ExitGames.Client.Photon;

public static class TeamExtensions
{
	public static PunTeams.Team GetTeam(this PhotonPlayer player)
	{
		if (player.CustomProperties.TryGetValue("team", out var value))
		{
			return (PunTeams.Team)value;
		}
		return PunTeams.Team.none;
	}

	public static void SetTeam(this PhotonPlayer player, PunTeams.Team team)
	{
		if (!PhotonNetwork.connectedAndReady)
		{
			DConsole.LogWarning(string.Concat("JoinTeam was called in state: ", PhotonNetwork.connectionStateDetailed, ". Not connectedAndReady."));
		}
		else if (player.GetTeam() != team)
		{
			player.SetCustomProperties(new Hashtable { 
			{
				"team",
				(byte)team
			} });
		}
	}
}
