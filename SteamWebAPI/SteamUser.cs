using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordbot.SteamWebAPI.SteamUser
{
	class SteamUser
	{
	}

	public class Rootobject
	{
		public Response response { get; set; }
	}

	public class Response
	{
		public string steamid { get; set; }
		public int success { get; set; }
		public Player[] players { get; set; }
	}

	public class Player
	{
		public string steamid { get; set; }
		public int communityvisibilitystate { get; set; }
		public int profilestate { get; set; }
		public string personaname { get; set; }
		public string profileurl { get; set; }

	}


}
