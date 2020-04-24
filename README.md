# GTA V SimpleGangWar script

Grand Theft Auto V script to create a basic battle between two teams.

## Background

This script is inspired by [Kerosini's Gang War script](https://www.gta5-mods.com/scripts/gangwar-zip).
The focus of my script is to provide a similar simple script to create instant battles, while providing new customizable settings.

## Installing

- Download and extract the required dependencies: ScriptHookV & ScriptHookVDotNet
- Download & extract SimpleGangWar.cs into `Grand Theft Auto V/scripts` folder

## Usage

The key `B` is used to navigate through all the steps of the script. In-game help popups will describe what to do, but these are the different stages you will find:

1. The script will ask you to move to where the enemies will spawn
2. After pressing the hotkey, you must do the same to define where the allies will spawn
3. Right after defining both spawnpoints, peds from both teams will spawn on their respective spawnpoints, and fight each other
4. Press the hotkey once to enter the "exit mode" (it will ask for confirmation to stop the battle)
5. Pressing the hotkey again will inmediately stop the battle and remove all alive & dead peds from the map

## TODO

- Setup teams and other settings through a .ini file; Document settings
- Allow to pause & resume ped spawning
- Avoid spawn-killing
- Add winning conditions
- Smooth transition from battle end to cleanup (extra step?)
- Fix peds being idle if spawnpoints are too far (change or combine task to RunTo?)
- Add menu/more hotkeys to improve UX?
- Respawn on ally spawnpoint after player dies
- Organize data, settings, variables - for each teams on the script structurally (struct?)

## Changelog

- 0.1.1
	- Add names to blips
	- Set health & armor to peds of each team
	- Option to disable wanted level during gang war
	- Option to select ped task behaviour
	- Option to select combat movement for each team
	- Different script Tick intervals for idle or in-battle
- 0.0.1 - Initial release

## Known bugs

- If spawnpoints are too far away from each other, peds can idle and do nothing
- When using [Watch Your Death](https://gta5-mods.com/scripts/watch-your-death), while player is dead, enemies can run to ally spawnpoint without fighting, or be idle
