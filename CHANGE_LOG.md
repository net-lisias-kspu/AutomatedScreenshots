# Automated Screenshots /L Unleashed :: Change Log

* 2021-1105: 0.8.6.0 (Lisias) for KSP >= 1.3.1
	+ Updating KSPe support for v2.4 (Including Toolbar)
	+ Some performance issues solved or at least mitigated
		- Heavy modded rigs should have a slight relief.
	+ Lowers the bar downto KSP 1.3.1!! **#HURRAY!!**
* 2018-0804: 0.8.4.2 (Lisias) for KSP 1.4.x (reissue)
	+ Fixed the Textures path on code
	+ Fixing name space and directory
	+ Moving the Configuration Data into <KSP_ROOT>/PluginData .
* 2018-0621: 0.8.4.1 (linuxgurugame) for KSP 1.4.x
	+ Added dependency checking 
* 2018-0621: 0.8.4 (linuxgurugame) for KSP 1.4.x
	+ Updated code to use latest version of ToolbarController
	+ Removed blizzy options, now handled by ToolbarController 
* 2018-0330: 0.8.3 (linuxgurugame) for KSP 1.4.x
	+ Updated for 1.4.1/1.4.2
	+ Added support for the Click Through Blocker
	+ Added support for the ToolbarControler
	+ Removed ALL old toolbar code (greatly simplified codebase)
	+ Moved textures into PluginData directory to avoid useless error messages from Unity 
* 2017-1008: 0.8.2 (linuxgurugame) for KSP 1.3.1
	+ Updated for KSP 1.3.1 
* 2017-0808: 0.8.1 (linuxgurugame) for KSP 1.3
	+ Fixed dual screenshots not working all the time 
* 2017-0602: 0.8.0.1 (linuxgurugame) for KSP 1.3
	+ Rebuild due to bad packaging 
* 2017-0526: 0.8.0 (linuxgurugame) for KSP 1.3
	+ Replaced depreciated call to GameEvents.onLevelWas Loaded with Unity SceneManagment calls
	+ Updated for 1.3
* 2017-0130: 0.7.9 (linuxgurugame) for KSP 1.2.1
	+ Now deletes the loadmeta file as well as the sfs file
* 2017-0130: 0.7.8 (linuxgurugame) for KSP 1.2.1
	+ Added AssemblyVersion updating code
	+ Fixed window showing up on the main menu after being in a game
	+ Fixed toolbar button showing up on the main menu
* 2016-1102: 0.7.7 (linuxgurugame) for KSP 1.2.1
	+ Updated version for 1.2.1 
* 2016-1021: 0.7.6 (linuxgurugame) for KSP 1.2 
	+ Fixed problem with ToolbarWrapper.cs
		- Update per blizzy78/ksp_toolbar#39 to prevent NotSupportedException. 
* 2016-1015: 0.7.5 (linuxgurugame) for KSP 1.2 
	+ Moved config file into PluginData 
* 2016-0916: 0.7.4 (linuxgurugame) for KSP 1.2 PRERELEASE
	+ Updated for 1.2
	+ Changed toggle for autosave from ctrl-F5 to ctrl-F6
* 2016-0523: 0.7.3 (linuxgurugame) for KSP 1.1
	+ Fixed case issue in file names, only a problem on Linux & OSX 
* 2016-0515: 0.7.2 (linuxgurugame) for KSP 1.1
	+ Added new option to start autosaving after loading game
* 2016-0419: 0.7.1.2 (linuxgurugame) for KSP 1.1
	+ No changelog provided 
* 2015-1002: 7.1.1 (no binary)
	+ No changelog provided 
* 2015-1002: 7.1 (no binary)
	+ No changelog provided 
* 201?-****: 0.6 (no binary)
	+ Added ability to supersize screenshots 
* 201?-****: 0.5.3 (no binary)
	+ Fixed bug where if a directory was unwritable, the screen could get locked with the gui hidden
	+ Fixed bug where a relative path would not work.
* 201?-****: 0.5.2 (no binary)
	+ Changed from LateUpdate to FixedUpdate, to allow faster screenshots
	+ Changed minimum time interval from 0.1 to Time.deltaTime (usualy 0.02)
	+ Fixed bug where if screenshots were stopped, and there was a png waiting to be converted, and it was deleted, no more screenshots could be taken until the game was restarted
* 201?-****: 0.5.1 (no binary)
	+ Added line to config window to show the activation key for the automated saves.  Not configurable at this time 
* 201?-****: 0.5.0 (no binary)
	+ Added public function for external mods to call to do an automated save
	+ Added public funciton for external mods to call to do an automated screenshot
	+ Updated zero-padding code for filename to use the ToString() syntax available, save a little codespace
	+ Refactored game save code to put all save functions into single class
* 201?-****: 0.4.1 (no binary)
	+ updated AVC version file
* 201?-****: 0.4.0 (no binary)
	+ Changed minimum frequency of screenshots from 1 sec to 0.1 sec
	+ Fixed bug where no active vessel caused spamming in log file in the SpaceCenter scene
	+ Added automated saving of save files
	+ Added rotation of save files to limit number of files 
	+ Fixed screenshots during timewarp, to NOT take too many snapshots.  Now	 uses realtime, not gametime
	+ Fixed name of Plugin directory, changed to Plugins
	+ Added check for colon in file name, replace it with a dash
	+ Updated hour/min/sec time to have leading zeroes
* 201?-****: 0.3.0 (no binary)
	+ Updated logging code so INFO is forced during development/debugging
	+ Added code to be able to tell the Historian mod a screenshot is happening.  Needs a small change to Historian to work
	+ Cleaned up some more code
	+ Updated buildrelease.bat
	+ Removed unnecessary PNG files from release
	+ Added ChangeLog.txt to release
	+ Moved ChangeLog.txt up one directory
* 201?-****: 0.2.0 (no binary)
	+ Added pre crash/landing detection 
	+ Added ability to take screenshots both with and without UI
	+ Bug fixed where if you had the UI hidden, it would be enabled after a screenshot
	+ Fixed missing toolbar icons
* 201?-****: 0.1.0 (no binary)
	+ initial release
