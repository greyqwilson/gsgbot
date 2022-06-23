using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System;
using Discord.Rest;
using System.Collections.Generic;
using System.Linq;

namespace discordbot
{

	//Create a module with no prefix
	public class InfoModule : ModuleBase<SocketCommandContext>
	{
		//~say hello world -> hello world
		[Command("say")]
		[Summary("Echoes a message.")]
		public Task SayAsync([Remainder][Summary("The text to echo")] string echo)
			=> ReplyAsync(echo);
		//ReplyAsync is a method on ModuleBase
	}

	
	public class GeneralCommands : ModuleBase<SocketCommandContext>
	{
		private CommandService _commands;
		public GeneralCommands(CommandService commands)
		{
			_commands = commands;
		}

		public Dictionary<ulong, GameLibrary> libraryDic;

		//Right now this functions by taking user input libraries.
		//Wish to implement steam api integration
		//~getgame -> Risk of Rain 2, CK2, HoI4...
		[Command("getgame")]
		[Summary("Prints a list of games that can be played with all members" +
			"currently in voice chat channel. add a space then a 1 to get a random game")]
		public async Task GetGame([Summary("The voice channel to be checked")] SocketVoiceChannel channel,
								  [Summary("Operating mode. 0 for default, 1 for random")] int mode = 0)
		{
			await ReplyAsync("Looking for matches in " + channel.Name + ":");
			//Get list of people currently connected to voice
			IReadOnlyCollection<Discord.WebSocket.SocketUser> users = channel.Users;
			int numUsers = users.Count;
			//If no one is in channel, then quit
			if (users == null)
			{
				await ReplyAsync("No one is in " + channel.Name);
				return;
			}

			//Get library for each person in list
			libraryDic = FileOps.MakeDictionary();
			
			if (libraryDic == null)
			{
				await ReplyAsync("Cannot match games with an empty GameLibraries.json.");
				return;
			}

			// Game, [PersonA, PersonB, ...]
			//Dictionary<string, List<string>> matches = new Dictionary<string, List<string>>();
			List<string> matches = new List<string>();
			List<List<string>> allUserGames = new List<List<string>>();
			//List<GameLibrary> allLibs = new List<GameLibrary>();
			//Gather a list of everyone's library
			int usersNoLibrary = 0;
			foreach (Discord.WebSocket.SocketUser user in users)
			{
				//Does this user have a library?
				if (libraryDic.ContainsKey(user.Id))
				{
					allUserGames.Add(libraryDic[user.Id].GetGames());
				}
				else
				{
					usersNoLibrary++;
				}
				//allLibs.Add(libraryDic[user.Id]);
			}
			List<string> user1List = allUserGames.First<List<string>>();
			//Remove user1List as we will use it to compare to rest
			allUserGames.RemoveAt(0);
			numUsers--; //To account for user1List being removed
			//int numUsers = allUserGames.Count;
			int countedMatches = 0;
			
			foreach (string game in user1List)
			{
				for (int i=0; i < allUserGames.Count; i++)
				{
					//Get the next list
					List<string> user2List = allUserGames.First<List<string>>();
					//Shuffle list to end
					allUserGames.Add(user2List);
					//Remove it from searching list
					allUserGames.RemoveAt(0);

					//If the game we are currently searching for is NOT in the other person's list
					//Stop searching for the current game, and begin searching for the next game in user1List
					if (!user2List.Contains(game))
					{
						//Go check next game. continue or break? to leave for loop only
						break;
					}
					//Otherwise, we must continue searching, adding a count for each we find
					countedMatches++;
					//If counted matches is equal to the number of users in chat
					if (countedMatches == numUsers)
					{
						//Add the game to matches
						matches.Add(game);
						//Reset countedmatches
						countedMatches = 0;
						//Replenish user list of games (already done above with shuffle)
						//here with a foreach or after each removal

					}
				}

			}
			//print our matches
			String allMatches = "";
			foreach (string game in matches)
			{
				allMatches += game + "\n";
			}
			//Reget number of users in chat
			numUsers = users.Count;
			//await ReplyAsync("Of the " + numUsers + " in chat, " + usersNoLibrary + " have no libraries.");
			if (allMatches.Length != 0 && mode == 0)
			{
				//Discord max message length is 2000 characters
				if (allMatches.Length < 2000)
				{
					await ReplyAsync("Everyone has:\n" + allMatches);
				}
				else
				{
					IEnumerable<string> pages = FileOps.StringSplit(allMatches, 1900);
					int i = 1;
					foreach(string page in pages)
					{
						await ReplyAsync("Page " + i + ":\n" + page);
						i++;
						System.Threading.Thread.Sleep(300);
					}
					//int numPages = allMatches.Length % 1900;
					//int splitIndex = 1900;
					//for (int i=1; i <= numPages; i++)
					//{
					//	String userGames2 = allMatches.Substring(splitIndex, 1900);
					//	allMatches.
					//	userGames2.Trim();
					//	await ReplyAsync(allMatches);
					//	System.Threading.Thread.Sleep(1000);
					//	await ReplyAsync(userGames2);
					//}
				}
				
			}
			else if (allMatches.Length != 0 && mode == 1)
			{
				Random random = new Random(Guid.NewGuid().GetHashCode()); //thank you joppiesaus from stackoverflow
				int randnum = random.Next() % (matches.Count);
				string randomGame = matches[randnum];
				await ReplyAsync("Random game is...\n" + randomGame);
			}	
			else
			{
				await ReplyAsync("No one has any matching games in their library.");
			}
			//Array arr = allUserGames.ToArray<List<string>>();


			//Check to see if read file makes sense
			//Get all users games
			//Add all of their games to some structure
			//Might be good to make a recursive function

			//void HelperDude()
			//{

			//}

			//foreach (Discord.WebSocket.SocketUser user in users)
			//{
			//	List<string> games = libraryDic[user.Id].GetGames();
			//	allUsersGames.Add(games);
			//	matches.Add()
			//	Stack<string> current = new Stack<string>();
			//	Find matches somehow
			//	for (int i = 0; i < users.Count; i++)
			//	{

			//	}
			//	await ReplyAsync(user.Username);
			//}

			await Context.Channel.SendMessageAsync("I am currently a work in progress.");
		}

		[Command("makelibrary")]
		[Summary("Makes a library for the chosen user")]
		public async Task MakeLibrary([Summary("The user to create a library for")]
										Discord.WebSocket.SocketUser user = null)
		{
			var userInfo = user ?? Context.Message.Author;
			//await ReplyAsync("Attempting to make library...");
			FileOps.ReadLibraries(ref libraryDic);

			//This is necessary on each action involving dictionary or library
			libraryDic = FileOps.MakeDictionary();
			//Check to see if read file makes sense
			if (libraryDic == null)
			{
				await ReplyAsync("Dictionary is empty...making new one");
				libraryDic = new Dictionary<ulong, GameLibrary>();
			}

			if (userInfo != null)
			{
				if (libraryDic.ContainsKey(userInfo.Id) == false)
				{
					//await ReplyAsync(user.Username + " was not found. Adding now...");
					ulong id = userInfo.Id;
					GameLibrary gameLibrary = new GameLibrary(id);
					//gameLibrary must be saved
					if (libraryDic.TryAdd(id, gameLibrary))
					{
						//FileOps.SaveLibrary(gameLibrary);
						FileOps.SaveDictionary(libraryDic);
						//await ReplyAsync(user.Username + "'s library has been made!");
					}
					else
					{
						await ReplyAsync("Failed to create a library for " + user.Username);
					}
				}
				else
				{
					//await ReplyAsync(userInfo.Username + " already has a games library.");
					return;
				}
			}
			else
			{
				await ReplyAsync("Unable to create library. \"" + user + "\" was not found.");
			}
		}

		[Command("addgame")]
		[Summary("Adds a game to a user's GameLibrary")]
		public async Task AddGame([Summary("Game to add to user's library")]
									string game = null)
		{
			libraryDic = FileOps.MakeDictionary();
			//Check to see if read file makes sense
			if (libraryDic == null)
			{
				await ReplyAsync("Dictionary is empty...making new one");
				libraryDic = new Dictionary<ulong, GameLibrary>();
			}

			await ReplyAsync("Attempting to add " + game + " to " + Context.User.Username + "'s library.");
			//await ReplyAsync("DEBUG: GameLibrary Key = " + libraryDic[user.Id].Owner + " (" + user.Username + ")");
			//Might be able to fix all of this with a libraryDic.tryAdd
			//Check to see if user has library, if not either make library or tell to make library
			if (libraryDic.ContainsKey(Context.User.Id) == true)
			{
				//Check to see if game already exists in their library
				if (libraryDic[Context.User.Id].AddGame(game) == true)
				{
					FileOps.SaveDictionary(libraryDic);
					//If it doesn't, add the game
					await ReplyAsync(game + " was successfully added!");
					//game has already been added by if statement
				}
				else
					await ReplyAsync("Game is already in library or failed to add.");
				//If it does, throw exception
				
			}
			
		}

		[Command("removegame")]
		[Summary("Removes a game from a user's GameLibrary")]
		public async Task RemoveGame([Summary("Game to remove from user's library")]
										string game = null)
		{
			//Open json
			libraryDic = FileOps.MakeDictionary();
			//Check to see if read file makes sense
			if (libraryDic == null)
			{
				await ReplyAsync("No data found in GameLibraries.json. Try to create a library and a game first.");
				libraryDic = new Dictionary<ulong, GameLibrary>();
				return;
			}
			//Check to see if user has library, if not either make library or tell to make library
			if (libraryDic.ContainsKey(Context.User.Id) == true)
			{

				//Check to see if game already exists in their library
				if (libraryDic[Context.User.Id].RemoveGame(game) == true)
				{
					//If it does remove the game
					FileOps.SaveDictionary(libraryDic);
					await ReplyAsync(game + " was successfully removed!");
					//game has already been added by if statement
				}
				else
					await ReplyAsync("Game does not exist in library.");
				//If it does, throw exception
				//Check to see if game already exists in their library
				//If it doesn't, throw exception
			}
		}

		[Command("libraries")]
		[Summary("Print the users of the server that have games libraries.")]
		public async Task WhoLibraries()
		{
			libraryDic = FileOps.MakeDictionary();
			//Check to see if read file makes sense
			if (libraryDic == null)
			{
				await ReplyAsync("No libraries exist in GameLibraries.json.");
				return;
			}
			Dictionary<ulong, GameLibrary>.KeyCollection usersKeys = libraryDic.Keys;
			string librariesPrintout = "";
			foreach (ulong u in usersKeys)
			{
				Discord.IUser user = Context.Guild.GetUser(u);
				librariesPrintout += user.Username + "\n";
			}
			await ReplyAsync(librariesPrintout);
		}

		[Command("library")]
		[Summary("Prints the library of the specified user.")]
		public async Task PrintLibrary([Summary("The user who's library will be printed")]
										Discord.WebSocket.SocketUser user = null)
		{
			//Attempt to find user
			var userInfo = user ?? Context.Message.Author;
			if (userInfo != null)
			{				
				libraryDic = FileOps.MakeDictionary();
				//Check to see if read file makes sense
				if (libraryDic == null)
				{
					await ReplyAsync("No libraries exist in GameLibraries.json.");
					return;
				}
				//See if the user has a library
				if (libraryDic.ContainsKey(userInfo.Id))
				{
					//Print it
					List<string> games = libraryDic[userInfo.Id].GetGames();
					String userGames = "";
					foreach (string game in games)
					{
						userGames += game + ", ";
					}
					//Discord max message length is 2000 characters
					if (userGames.Length < 2000)
					{
						await ReplyAsync(userGames);
					}
					else
					{
						String userGames2 = userGames.Substring(1900, 1900);
						userGames2.Trim();
						await ReplyAsync(userGames);
						System.Threading.Thread.Sleep(1000);
						await ReplyAsync(userGames2);
					}
				}
				else
				{
					await ReplyAsync("User does not have a games library.");
				}
			}
			else
			{
				await ReplyAsync("User not found.");
				return;
			}
				
		}

			// ~sample square 20 -> 400
		[Command("square")]
		[Summary("Squares a number.")]
		public async Task SquareAsync(
			[Summary("The number to square.")]
		int num)
		{
			// We can also access the channel from the Command Context.
			await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
		}

		// ~sample userinfo --> foxbot#0282
		// ~sample userinfo @Khionu --> Khionu#8708
		// ~sample userinfo Khionu#8708 --> Khionu#8708
		// ~sample userinfo Khionu --> Khionu#8708
		// ~sample userinfo 96642168176807936 --> Khionu#8708
		// ~sample whois 96642168176807936 --> Khionu#8708
		[Command("userinfo")]
		[Summary
		("Returns info about the current user, or the user parameter, if one passed.")]
		[Alias("user", "whois")]
		public async Task UserInfoAsync(
			[Summary("The (optional) user to get info from")]
			Discord.WebSocket.SocketUser user = null)
		{
			var userInfo = user ?? Context.Client.CurrentUser;
			await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
		}

		[Command("shutdown")]
		[RequireOwner]//Should look more into this
		[Summary("Turns off the bot (delicately).")]
		public async Task Shutdown()
		{
			//only logs off
			await ReplyAsync("Goodbye!");
			await Context.Client.LogoutAsync();
			Environment.Exit(1);
			//do that shit here
		}

		
		[RequireOwner]
		[Command("initalias")]
		[Summary("Create a fresh alias table.")]
		public async Task InitAlias()
		{
			FileOps.InitializeAlias();
		}

		[RequireUserPermission(Discord.ChannelPermission.CreatePublicThreads)]
		[Command("addgamecatalog")] //rename everything to be catalog
		[Summary("Adds a game to the server game catalog. This command is available for both Green Sun roles." +
				 " Format for this command is\n !addgamecatalog [alias] [full name in quotes] .\n" +
				 "PLEASE FORMAT DATA CORRECTLY. Garbage in = Garbage out")]
		public async Task AddGameCatalog([Summary("The game's alias")] string alias,
										 [Summary("The game's full name")] string fullName)
		{
			Dictionary<string, string> catalog = FileOps.ReadAliasFile();
			if (catalog == null)
			{
				//PANIC
				await ReplyAsync("No games catalog was found. Creating...");
				catalog = new Dictionary<string, string>();
			}
			
			if (catalog.TryAdd(alias, fullName))
			{
				FileOps.SaveAliasFile(catalog);
				await ReplyAsync(fullName + " was successfully added to the catalog!");
			}
			else
			{
				await ReplyAsync(fullName + " already exists in the catalog");
			}
		}

		[RequireUserPermission(Discord.ChannelPermission.CreatePublicThreads)]
		[Command("removegamecatalog")]
		[Summary("Removes a game from server game catalog.")]
		public async Task RemoveGameCatalog([Summary("The game's alias")] string alias)
		{
			Dictionary<string, string> catalog = FileOps.ReadAliasFile();
			if (catalog == null)
			{
				await ReplyAsync("Games catalog is empty. Cannot remove");
				return;
			}
			
			if (catalog.Remove(alias))
			{
				FileOps.SaveAliasFile(catalog);
				await ReplyAsync(alias + " was successfully removed!");
			}
			else
			{
				await ReplyAsync(alias + " was not found in the game catalog. Check alias and retry.");
			}
		}

		[Command("showcatalog")]//This needs to not print one at a time. It should be an embed just like help
		[Summary("Prints the games available to add to user game libraries.")]
		public async Task PrintCatalog()
		{
			Dictionary<string, string> catalog = FileOps.ReadAliasFile();
			if (catalog == null)
			{
				//PANIC
				await ReplyAsync("No catalog was found in GameAliases.json");
				return;
			}

			Dictionary<string, string>.KeyCollection aliasCatalog = catalog.Keys;
			int i = 1;

			Discord.EmbedBuilder eb = new Discord.EmbedBuilder();
			String gameNameSet = "";
			foreach (string alias in aliasCatalog)
			{
				string fullName = catalog.GetValueOrDefault(alias);
				gameNameSet += i + ". " + fullName + ": " + alias + "\n";
				i++;
			}
			await ReplyAsync(gameNameSet);
			//await ReplyAsync("", false, eb.Build(), null, null, null, null, null, null);
		}
		[RequireOwner]
		[Command("allLibrariesInit")]
		public async Task AllLibraryInitialize()
		{
			IReadOnlyCollection<Discord.WebSocket.SocketGuildUser> allUsers = Context.Guild.Users;
			foreach (Discord.WebSocket.SocketGuildUser user in allUsers)
			{
				if (!user.IsBot)
				{
					await MakeLibrary(user);
				}
			}
			await ReplyAsync("Attempted to add guild: " + allUsers.Count);
		}

		[Command("linksteam")]
		[Summary("Takes the library associated with the entered Steam account and downloads it for use in findgame.")]
		public async Task LinkSteamLib([Summary("The user's Steam ID.")] String steamInfo)
		{
			//Should try to do some checking before attempting to create library a second time.
			
			SteamWebAPI.SteamWebAPIHandler webAPI = new SteamWebAPI.SteamWebAPIHandler();
			
			//See if they entered an ID
			String steamPersonaName = webAPI.GetSteamPersonaName(steamInfo);
			String steamId;

			//If they gave a valid steam ID
			if (steamPersonaName != null)
			{
				await ReplyAsync("Detected Steam profile name: " + steamPersonaName + " with given ID.");
				steamId = steamInfo;
			}

			else
			{
				//They may have entered a vanity url so check that
				if (webAPI.GetSteamID(steamInfo) != null)
				{
					await ReplyAsync("Found Steam ID.");
					//They sent a name, so set name to appropriate variable
					steamPersonaName = steamInfo;
					//Get their ID
					steamId = webAPI.GetSteamID(steamInfo);
				}
				else
				{
					await ReplyAsync("Failed to find a Steam profile associated with the given ID or name.");
					return;
				}
			}
			await ReplyAsync("Attempting to merge Steam games into current library. I may take a while.");
			Dictionary<ulong, GameLibrary> libraryDic = FileOps.MakeDictionary();

			//Set their steam name and steam ID to their gamelibrary
			libraryDic[Context.User.Id].SetSteamId(steamId);
			libraryDic[Context.User.Id].SetSteamName(steamPersonaName);

			//Get http response from steamwebapi, store into object
			SteamWebAPI.Response response = webAPI.GetSteamResponse(libraryDic[Context.User.Id].SteamId);
			//Convert response object's List<game> to List<string>
			List<string> steamgames = webAPI.SteamResponseToList(response);
			//Games in current user's library
			List<string> currentgames = libraryDic[Context.User.Id].GetGames();
			foreach (string game in steamgames)
			{
				//If the game doesn't exist in the current library, add it
				if (!currentgames.Contains(game))
				{
					currentgames.Add(game);
				}
			}
			await ReplyAsync("Done!");
			FileOps.SaveDictionary(libraryDic);
			//
		}
		[Alias("lsl")]

		[RequireOwner]
		[Command("addsteamaliases")]
		public async Task AddSteamAliases([Summary("Updates the alias file for the user's Steam ID.")] string steamId)
		{
			SteamWebAPI.SteamWebAPIHandler webAPI = new SteamWebAPI.SteamWebAPIHandler();
			webAPI.PrepareAliases(steamId);
		}

		[Command("help")]
		[Summary("Prints all known commands and their summaries")]
		public async Task Help()
		{
			IEnumerable<CommandInfo> commandInfos = _commands.Commands;
			Discord.EmbedBuilder eb = new Discord.EmbedBuilder();
			foreach (CommandInfo info in commandInfos)
			{
				string embedText = info.Summary ?? "No description!\n";
				eb.AddField(info.Name, embedText);
			}

			await ReplyAsync("", false, eb.Build(), null, null, null, null, null, null);
		}

	}
}
