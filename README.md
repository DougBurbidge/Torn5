# Torn 5
> Torn 5 is a laser tag tournament scores manager.

Torn 5 reads data from a lasergame server (currently Zone systems and Laserforce systems) and lets you commit games to its internal database. You can then track the performance of teams and players, and generate ladders and other reports.

It is a complete rewrite of Torn 4 (available from http://dougburbidge.com/Apps/ ) in C#.

It aims to eventually include most or all of the functionality of Torn 4, and to offer better web reports.

## Features

* Load games from P&C Nexus or Zeon systems, including hit-by-hit data
* Load games from Laserforce systems (aggregate data only)
* Create league files, and have several league files open at once
* Set handicaps, adjust scores, apply penalties, re-ID players
* Commit games
* Report on games
* Upload reports to the internet

## Configuration

Settings are stored in the Windows ApplicationData folder, in Torn\Torn5.Settings.


## Contributing

If you'd like to contribute, please fork the repository and use a feature branch. Pull requests are welcome. (Please try to follow the style already used in the application.)

I'm using SharpDevelop 4.4 as the IDE, and targeting .NET 4.0.

## Links

- My apps: http://dougburbidge.com/Apps/
- Repository: https://github.com/DougBurbidge/Torn5
- Past tournament results: http://dougburbidge.com/OzNationals/
