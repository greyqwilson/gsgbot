using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordbot.SteamWebAPI.SteamApp
{
	class SteamStoreApp
	{
	}


	public class Rootobject
	{
		public AppId appId { get; set; }
	}

	public class AppId
	{
		public bool success { get; set; }
		public Data data { get; set; }
	}

	public class Data
	{
		public string name { get; set; }
		public int steam_appid { get; set; }
		public Category[] categories { get; set; }
		public Genre[] genres { get; set; }
	}

	public class Category
	{
		public int id { get; set; }
		public string description { get; set; }
	}

	public class Genre
	{
		public string id { get; set; }
		public string description { get; set; }
	}

}
