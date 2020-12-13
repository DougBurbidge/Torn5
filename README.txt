Torn 5 is a ground-up rewrite of Torn 4. It is a tournament scores manager, and can read data from Nexus, Helios and Laserforce systems. It requires .NET Framework 4.0. Note that Torn 5 is currently in a Beta state -- it mostly works, but not all features are complete, and there may be bugs. If you are using it to open Torn 4 files, I recommend keeping a safe backup of those .Torn files elsewhere, just in case.

Run the program. Click the Leagues button, then Preferences. If you have a lasergame system, select the appropriate system and enter the laser game database server address. For P&C systems, this is jsut the name or IP address of the databsae server machine. For Laserforce, this is something like \\lfmain\lf6. If you don't have a lasergame system, just leave it at Demo.

Click OK. Click Latest. This will cause Torn to query the server. A list of games should appear.

On the Leagues toolbar, click New or Open. Torn 5 can open Torn 4 .Torn files. When it save those files it will write extra information. The latest version of Torn 4 will ignore all that extra information, so you _should_ be able to open the file in one, make changes, then open it in the other, and then back again. For your new (or newly opened) .Torn file, click in the Tag cell at the left of the .Torn file's row in the Leagues grid. Enter a tag. This will be used as a sub-folder for this league's reports exports, so use a tag name that is a legal folder name (i.e. does not contain special characters like /\:*?"<>|).

If you like, you can open more league files. This is a difference between Torn 4 and Torn 5 -- Torn 5 allows you to have several league files open at once.

Click a game to see its packs/players. Now you can do all the usual Torn things -- right-click on a team box and Remember that team, Commit the game, Edit the league, etc.

Because the games listing is now an always-visible list instead of being a drop-down "combo box", you can now select multiple games. Some features work on all selected games -- e.g. Forget will remove all the selected games from whatever league they had been committed to. Set Description lets you title committed games or groups of games: "Round 1", "Repechage 3", "Cascade 1", "Finals", etc. These titles will be shown in reports.

Reporting is different in Torn 5 -- click Set Folder to set an initial folder for reports to be exported to. Then click Report. Torn 5 will export a default set of reports to that folder. Note that these will include details of each team, game and player. If you would like to configure different reports for the selected league, click Configure and then add reports as desired.

If you select multiple leagues (select one league then hold down the ctrl key as you select more) and click Export, all the selected leagues will have reports exported.

If you have enabled the web server under Preferences, any web browser that can see the machine Torn is running on can view reports directly from Torn 5's latest data, without you needing to click Report. If you're running Torn on a machine called mymachine and you've left it on the default port of 8080, point a web browser at http://mymachine:8080 to see these interactive reports.

For P&C systems, for games that have been committed during this run of Torn 5, more dta is shown: each game has a "heat map" showing who shot who and a "worm" showing teams' scores. but this (extensive) data is not stored in the .Torn file, so if you exit Torn 5 and restart, a fresh report will _not_ contain this additional detail.

Things not yet implemented: fixture generation, pushing scores to a scoreboard, LotR mode, setting up pyramid rounds, and a variety of other bits and bobs.
