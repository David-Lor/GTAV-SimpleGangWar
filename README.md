# GTA V SimpleGangWar script

Grand Theft Auto V script to create a basic battle between two teams.

## Background

This script is inspired by [Kerosini's Gang War script](https://www.gta5-mods.com/scripts/gangwar-zip).
The focus of my script is to provide a similar simple script to create instant battles, while providing new customizable settings.

## Installing

- Download and extract the required dependencies: ScriptHookV & ScriptHookVDotNet
- Download & extract `SimpleGangWar.cs` & `SimpleGangWar.ini` into `Grand Theft Auto V/scripts` folder

## Usage

The key `B` is used to navigate through all the steps of the script. In-game help popups will describe what to do, but these are the different stages you will find:

1. The script will ask you to move to where the enemies will spawn
2. After pressing the hotkey, you must do the same to define where the allies will spawn
3. Right after defining both spawnpoints, peds from both teams will spawn on their respective spawnpoints, and fight each other
4. Press the hotkey once to enter the "exit mode" (it will ask for confirmation to stop the battle)
5. Pressing the hotkey again will inmediately stop the battle and remove all alive & dead peds from the map

## Settings

Settings can be defined on the `SimpleGangWar.ini` file, being the following:

### ALLIED_TEAM & ENEMY_TEAM

_All lists of items (models & weapons) are separated by comma (`,`) or semi-colon (`;`). Spaces and case ignored._

- `Models`: list of ped models ([Reference](https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/PedHash.cs))
- `Weapons`: list of ped weapons ([Reference](https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/WeaponHash.cs))
- `Health`: health for peds (should not be least than 100)
- `Armor`: armor for peds (from 0)
- `Accuracy`: accuracy for peds (from 0)
- `CombatMovement`: how the peds will move through the battlefield. This can be used to make one team defend its spawnpoint, while the other team tries to attack it. One of following:
	- `stationary`: not move at all
	- `defensive`: stay near the spawnpoint and take cover
	- `offensive`: focus on attacking the enemy team
	- `suicidal`: more aggresive attack
	- _stationary & suicidal seem to take no effect, so is better to stick to just **defensive** and **offensive**_

## SETTINGS

- `Hotkey`: the single hotkey used on the script ([Reference](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key?view=netcore-3.1#fields))
- `MaxPedsPerTeam`: maximum alive peds on each team (true/false)
- `NoWantedLevel`: if true, disable wanted level during the battle (true/false)
- `ShowBlipsOnPeds`: if true, each spawned ped will have a blip on the map (true/false)
- `DropWeaponOnDead`: if false, dead peds won't drop their weapons - they will remain stick to their bodies (true/false)
- `RemoveDeadPeds`: if true, mark dead peds as no longer needed, making the game handle their cleanup (true/false)
- `RunToSpawnpoint`: if true, the peds task will be to run to their enemies' spawnpoint; if false, will be to fight hated targets on the area (true/false).
  The task RunTo (true) seems to have lower negative effect on peds behaviour (avoid them from being idle stuck - but it can still happen if spawnpoints are too far away).
  The task FightAgainstHatedTargets (false) can be interesting when spawnpoints are closer, as peds might have more freedom to flank the enemy?

## TODO

- Allow to pause & resume ped spawning
- Avoid spawn-killing
- Add winning conditions
- Smooth transition from battle end to cleanup (extra step?)
- Add menu/more hotkeys to improve UX?
- Respawn on ally spawnpoint after player dies
- Organize data, settings, variables - for each teams on the script structurally (struct?)

## Changelog

- 1.0.1
	- Support settings through .ini file
	- Change if-else to switch in OnKeyUp
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
