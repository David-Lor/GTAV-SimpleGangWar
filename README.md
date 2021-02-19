# GTA V SimpleGangWar script

Grand Theft Auto V script to create a basic battle between two teams.

## Background

This script is inspired by [Kerosini's Gang War script](https://www.gta5-mods.com/scripts/gangwar-zip).
The focus of my script is to provide a similar simple script to create instant battles, while providing new customizable settings.

## Installing

- Download and extract the required dependencies: ScriptHookV & ScriptHookVDotNet
- Download & extract `SimpleGangWar.cs` & `SimpleGangWar.ini` into `Grand Theft Auto V/scripts` folder

## Usage

The key `B` ("Hotkey") is used to navigate through all the steps of the script. In-game help popups will describe what to do, but these are the different stages you will find:

1. The script will ask you to move to where the enemies will spawn
2. After pressing the hotkey, you must do the same to define where the allies will spawn
3. Right after defining both spawnpoints, peds from both teams will spawn on their respective spawnpoints, and fight each other
4. Press the hotkey once to enter the "exit mode" (it will ask for confirmation to stop the battle)
5. Pressing the hotkey again will inmediately stop the battle and remove all alive & dead peds from the map

An additional hotkey `N` ("SpawnHotkey") is used to pause/resume the ped spawning in both teams. The map blips ("E" and "A") will blink whenever the spawning is paused.

## Settings

Settings can be defined on the `SimpleGangWar.ini` file, being the following:

### ALLIED_TEAM & ENEMY_TEAM

_All lists of items (models & weapons) are separated by comma (`,`) or semi-colon (`;`). Spaces and case ignored._

- `Models`: list of ped models ([Reference](https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/PedHash.cs) | [Reference with pics (use the names on the other link)](https://docs.fivem.net/docs/game-references/ped-models))
- `Weapons`: list of ped weapons ([Reference](https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/WeaponHash.cs))
- `Health`: health for peds (should not be least than 100)
- `Armor`: armor for peds (from 0)
- `Accuracy`: accuracy for peds (from 0)
- `CombatMovement`: how the peds will move through the battlefield. This can be used to make one team defend its spawnpoint, while the other team tries to attack it. If RunToSpawnpoint=true, this setting most probably will be ignored. One of following:
	- `stationary`: not move at all
	- `defensive`: stay near the spawnpoint and take cover
	- `offensive`: focus on attacking the enemy team
	- `suicidal`: more aggresive attack
	- `disabled`: do not alter this setting on peds
	- `random`: randomize between `defensive` and `offensive` for each spawned ped. This does not always work as expected, since some peds can be stuck on the spawnpoint waiting for other peds to attack, but since they are defending their position, nobody would attack
- `CombatRange`: how far or close the peds will fight against their enemies. This might not have a huge difference, depending on the scenario. One of following:
	- `near`
	- `medium`
	- `far`
	- `disabled`: do not alter this setting on peds
	- `random`: randomize between `near`, `medium`, `far` for each spawned ped
- `MaxPeds`: maximum alive peds on the team (if not specified, the MaxPedsPerTeam setting will be used)

### SETTINGS

- `Hotkey`: the single hotkey used to iterate over the script stages ([Reference](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key?view=netcore-3.1#fields))
- `SpawnHotkey`: hotkey used to pause/resume ped spawn in both teams ([Reference](https://docs.microsoft.com/en-us/dotnet/api/system.windows.input.key?view=netcore-3.1#fields))
- `MaxPedsPerTeam`: maximum alive peds on each team - teams with the setting MaxPeds will ignore this option
- `NoWantedLevel`: if true, disable wanted level during the battle (true/false)
- `ShowBlipsOnPeds`: if true, each spawned ped will have a blip on the map (true/false)
- `DropWeaponOnDead`: if false, dead peds won't drop their weapons - they will remain stick to their bodies (true/false)
- `RemoveDeadPeds`: if true, mark dead peds as no longer needed, making the game handle their cleanup (true/false)
- `RunToSpawnpoint`: if true, the peds task will be to run to their enemies' spawnpoint; if false, will be to fight hated targets on the area (true/false).
  The task RunTo (true) seems to have lower negative effect on peds behaviour (avoid them from being idle stuck - but it can still happen if spawnpoints are too far away).
  The task FightAgainstHatedTargets (false) can be interesting when spawnpoints are closer, as peds might have more freedom to flank the enemy?
- `ProcessOtherRelationshipGroups`: if true, get all relationship groups from other existing peds and match these groups with the groups of SimpleGangWar peds.
  Set it to true if you experience the spawned peds fighting against other peds (like mission peds) when they should not be (for example, enemy peds of a mission fighting against enemy peds of SimpleGangWar).
- `SpawnpointFloodLimitPeds`: limit how many peds can be near its spawnpoint. If more than this quantity of peds are near the spawnpoint, no more peds on the team will spawn. Disable this feature by setting this variable to `0`.
- `SpawnpointFloodLimitDistance`: in-game distance from a team spawnpoint to keep track of the SpawnpointFloodLimitPeds. Can be integer or decimal (if using decimals, use dot or comma depending on your system regional settings)
- `IdleInterval`: delay between loop runs, when battle is not running, in ms
- `BattleInterval`: delay between loop runs, when battle is running, in ms

## Known bugs

- If spawnpoints are too far away from each other, peds can idle and do nothing
- When using [Watch Your Death](https://gta5-mods.com/scripts/watch-your-death), while player is dead, enemies can run to ally spawnpoint without fighting, or be idle
- Peds can avoid reloads (this is mostly noticeable with muskets)

## TODO

- Avoid spawn-killing
- Add winning conditions
- Smooth transition from battle end to cleanup (extra step?)
- Add menu/more hotkeys to improve UX?
- Respawn player on ally spawnpoint after dying
- Organize data, settings, variables - for each teams on the script structurally (struct?)

## Changelog

- 2.2.1
	- Add spawnpoint anti-flood feature (avoid peds from flooding their spawnpoints)
	- Add options to randomize CombatMovement & CombatRange
	- Add options to disable altering CombatMovement & CombatRange
- 2.1.1
	- Add CombatRange setting
	- Add ProcessOtherRelationshipGroups setting
	- Add IdleInterval & BattleInterval settings to .ini file (they were defined on the script but not documented on the .ini file)
	- Add docstrings to the script functions
- 2.0.1
	- Pause/resume ped spawning in both teams
	- Fix usage of default hotkeys when not specified in .ini file
	- Rearrange variables in script
	- Refactor README
- 1.1.1
	- Options to set ped limit per team
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
- 0.0.1
	- Initial release
