# GTA V SimpleGangWar script

Grand Theft Auto V script to create a basic battle between two teams.

## Installing

- Download and extract the required dependencies: ScriptHookV & ScriptHookVDotNet
- Download & extract SimpleGangWar.cs into `Grand Theft Auto V/scripts` folder

## Usage

The key `B` can be used to navigate through all the steps of the script. In-game help popups will describe what to do, but these are the different stages you will find:

1. The script will ask you to move to where the enemies will spawn
2. After pressing the hotkey, you must do the same to define where the allies will spawn
3. Right after defining both spawnpoints, peds from both teams will spawn on their respective spawnpoints, and fight each other
4. Press the hotkey once to enter the "exit mode" (it will ask for confirmation to stop the battle)
5. The last hotkey press will inmediately stop the battle and remove all alive & dead peds from the map

## TODO

- Setup teams and other settings through a .ini file
- Allow to pause & resume ped spawn
- Avoid spawn-killing
- Fix peds being idle if spawnpoints are too far (change or combine task to RunTo?)
- Add menu/more hotkeys to improve UX?

## Changelog

- 0.0.1 - Initial release

