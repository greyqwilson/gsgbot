Current Issues:
- Add function doesnt work. Something to do with Gamelibrary.games list being destroyed -- FIXED: added tag to games list in GameLibrary
  + workaround right now is to manually make list outside of class. didnt work though
  + maybe we need to add gamelibrary manually
- Json dictionary file was broken from polluting file with two json objects -- FIXED
- Adding a new game to a library currently adds a new JSON entry causing duplicates. --FIXED by making it Write, not append
- Linking via steam Name doesn't work because getting error 500 (bad response) will return from function

Current Tasks:
 - ALL GameLibrary objects MUST BE INITIALIZED, otherwise JSONConvert Fails to do its job
 |Func| GetGames
	- Account for the number of people in chat that do not have a library
	- Save name of users that have no libraries and those that do for printout
- Make commands easier to use like calling addGame without explicitly stating user
 |Func| AddGame
	- Conditins aren't precise and just returns a false message. Jake attempts to add game and it fails
- !library needs to print as a single message
- Doesnt accept full game name
Future Ideas:
 - Command log saved to file
 - Config
 - Allow for alias detection
 - Print who has a library
 - Print a user's library
 - Sort alias/catalog alphabetically
 - A super fast way to compare games by OR-ing games lists
 - Make replies only viewable by person calling command

 Changed:
 - !addgame no longer needs a username to run
