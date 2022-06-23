using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using discordbot.SteamWebAPI;

namespace discordbot
{
	public static class FileOps
	{
		//Should have an initial read (sync) and then an async read
		const string PATH = "\\GameLibraries";
		const string FILENAME = "GameLibraries.json";
		const string ALIASFILE = "GameAliases.json";



		public static string ReadLibraries(ref Dictionary<ulong, GameLibrary> gameDic)
		{
			//String line;
			//Make file if it doesn't exist. This probably isnt the best way to do this
			try
			{
				InitializeFile(PATH + "\\" + FILENAME);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return (e.ToString());
			}
			//All the reading goes here!
			using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + FILENAME))
			{
				string line = sr.ReadLine();
				if (line == null)
				{
					Console.WriteLine("Blank file");
					return ("No libraries read");
				}

				//This might be redundant with MakeDictionary
				//while (line != null)
				//{
				//	Console.WriteLine("Successfull!");
				//	//JSON Deserialize here;
				//	GameLibrary gameLibrary = JsonConvert.DeserializeObject<GameLibrary>(line);
				//	gameDic.Add(gameLibrary.Owner, gameLibrary);
				//	//Read next line
				//	line = sr.ReadLine();
				//}
				sr.Close();
				return ("Library successfully read x# of libraries!");
			}
			
		}

		public async static void AddEntry(GameLibrary gameLibrary)
		{
			//Serialize here
			string serialize = JsonConvert.SerializeObject(gameLibrary);
			await File.AppendAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + FILENAME,
										serialize, cancellationToken: System.Threading.CancellationToken.None);
		}

		//path, PATH, and FILENAME should be a little clearer in terms of organization
		private static void InitializeFile(string path)
		{
			Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + PATH);
			using StreamWriter streamWriter = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + path);
		}

		private static bool CheckFileExist(string path)
		{
			return Directory.Exists(path);
		}

		public async static void RemoveEntry(string game)
		{
			//removes here
		}

		public async static void SaveLibrary(GameLibrary gameLibrary)
		{
			string toWrite = JsonConvert.SerializeObject(gameLibrary);
			await File.AppendAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + FILENAME,
															toWrite, cancellationToken: System.Threading.CancellationToken.None);
			//sw.Close();
		}

		public async static void SaveDictionary(Dictionary<ulong, GameLibrary> libraryDic)
		{
			string toWrite = JsonConvert.SerializeObject(libraryDic, Formatting.Indented);
			await File.WriteAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + FILENAME,
															toWrite, cancellationToken: System.Threading.CancellationToken.None);
		}

		public static Dictionary<ulong, GameLibrary> MakeDictionary()
		{
			//Want to make async, not sure how as return must be a Task
			using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + FILENAME))
			{
				String line = sr.ReadToEnd();

				//Json convert struggles to convert this, aka it cannot with a cast
				Dictionary<ulong, GameLibrary> theDic = JsonConvert.DeserializeObject<Dictionary<ulong, GameLibrary> >(line);
				return theDic;
			}
		}

		public static Dictionary<string, string> ReadAliasFile()
		{
			//Not the best data structure for surjective a->x b->x c->z
			using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + ALIASFILE))
			{
				string table = sr.ReadToEnd();
				Dictionary<string, string> aliasTable = JsonConvert.DeserializeObject<Dictionary<string, string> >(table);
				return aliasTable;
			}
		}

		public async static void SaveAliasFile(Dictionary<string, string> aliasTable)
		{
			string toWrite = JsonConvert.SerializeObject(aliasTable, Formatting.Indented);
			await File.WriteAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + ALIASFILE,
															toWrite, cancellationToken: System.Threading.CancellationToken.None);
		}

		public async static void InitializeAlias()
		{
			Dictionary<string, string> aliasTable = new Dictionary<string, string>();
			aliasTable.Add("nope", "no");
			string toWrite = JsonConvert.SerializeObject(aliasTable, Formatting.Indented);
			await File.WriteAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\" + ALIASFILE,
															toWrite, cancellationToken: System.Threading.CancellationToken.None);
		}

		public async static void SteamLibTest()
		{
			//SteamWebAPIHandler webAPI = new SteamWebAPIHandler();
			//Response response = webAPI.GetSteamResponse();
			//string toWrite = JsonConvert.SerializeObject(response, Formatting.Indented);
			//await File.WriteAllTextAsync(AppDomain.CurrentDomain.BaseDirectory + PATH + "\\steamlib.json",
			//							toWrite, cancellationToken: System.Threading.CancellationToken.None);
		}

		//Partially implemented. Fully made as command
		public static void MergeSteamToNativeList(GameLibrary gl)
		{
			Dictionary<ulong, GameLibrary> libraryDic = MakeDictionary();

			SteamWebAPIHandler webAPI = new SteamWebAPIHandler();
			//Get http response from steamwebapi, store into object
			Response response = webAPI.GetSteamResponse(gl.SteamId);
			//Convert response object's List<game> to List<string>
			List<string> steamgames = webAPI.SteamResponseToList(response);
			//Games in current user's library
			List<string> currentgames = gl.GetGames();
			foreach (string game in steamgames)
			{
				//If the game doesn't exist in the current library, add it
				if (!currentgames.Contains(game))
				{
					currentgames.Add(game);
				}
			}
			//Access Dictionary and library
		}

		//Thank you konstantin spirin and pham x. bach stackoverflow
		public static IEnumerable<string> StringSplit(string str, int chunkSize)
		{
			return Enumerable.Range(0, str.Length / chunkSize)
				.Select(i => str.Substring(i * chunkSize, chunkSize));
		}

	}
}
