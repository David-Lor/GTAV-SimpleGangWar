using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Windows.Forms;
using System.Collections.Generic;

public class SimpleGangWar : Script {

    // Peds: https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/PedHash.cs
    // Weapons: https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/WeaponHash.cs
    // Peds with images (do not use the exact names listed here): https://wiki.gtanet.work/index.php/Peds - https://docs.fivem.net/docs/game-references/ped-models

    // Families (allies) vs Ballas (enemies)
    private static readonly string[] pedsAllies = { "Families01GFY", "Famca01GMY", "Famdnf01GMY", "Famfor01GMY" };
    private static readonly string[] weaponsAllies = { "AssaultRifle", "CompactRifle", "SNSPistol", "VintagePistol", "Pistol", "MicroSMG" };
    private static readonly string[] pedsEnemies = { "BallaEast01GMY", "BallaOrig01GMY", "Ballas01GFY", "BallaSout01GMY" };
    private static readonly string[] weaponsEnemies = { "MicroSMG", "MachinePistol", "Gusenberg", "Musket", "Pistol", "VintagePistol", "CompactRifle" };
    
    // Misc gangs (allies) vs Misc security forces (enemies)
    /*private static readonly string[] pedsAllies = { "Families01GFY", "Famca01GMY", "Famdnf01GMY", "Famfor01GMY", "BallaEast01GMY", "BallaOrig01GMY", "Ballas01GFY", "BallaSout01GMY", "Vagos01GFY", "Lost01GFY", "Lost01GMY", "Lost02GMY", "Lost03GMY" };
    private static readonly string[] weaponsAllies = { "AssaultRifle", "CompactRifle", "Pistol", "SNSPistol", "VintagePistol", "MicroSMG", "MG", "MachinePistol" };
    private static readonly string[] pedsEnemies = { "BogdanGoon", "AvonGoon", "Blackops01SMY", "Blackops02SMY", "Blackops03SMY", "Marine03SMY", "Devinsec01SMY", "SecuroGuardMale01", "Armoured01SMM", "Armoured02SMM", "Armoured01", "ChemSec01SMM" };
    private static readonly string[] weaponsEnemies = { "CarbineRifle", "CarbineRifleMk2", "CombatMG", "CombatMGMk2", "PumpShotgunMk2", "PistolMk2", "RevolverMk2", "SMG", "SMGMk2", "CombatPDW" };*/

	// Ballas (allies) vs Families (enemies)
	/*private static readonly string[] pedsAllies = { "BallaEast01GMY", "BallaOrig01GMY", "Ballas01GFY", "BallaSout01GMY" };
    private static readonly string[] weaponsAllies = { "AssaultRifle", "CompactRifle", "Gusenberg", "SNSPistol", "Pistol", "VintagePistol", "MachinePistol", "MicroSMG", "SMG" };
    private static readonly string[] pedsEnemies = { "Families01GFY", "Famca01GMY", "Famdnf01GMY", "Famfor01GMY" };
    private static readonly string[] weaponsEnemies = { "AssaultRifle", "CompactRifle", "Gusenberg", "SNSPistol", "Pistol", "VintagePistol", "MachinePistol", "MicroSMG", "SMG" };*/

    private static readonly int healthAllies = 120;
    private static readonly int armorAllies = 0;
    private static readonly int healthEnemies = 120;
    private static readonly int armorEnemies = 0;
    private static readonly int accuracyAllies = 5;
    private static readonly int accuracyEnemies = 5;
    private static readonly CombatMovement combatMovementAllies = CombatMovement.Offensive;
    private static readonly CombatMovement combatMovementEnemies = CombatMovement.Offensive;

    private static readonly int maxPedsPerTeam = 10;
    private static readonly Keys hotkey = Keys.B;
    private static readonly bool noWantedLevel = true;
    private static readonly bool showBlipsOnPeds = true;
    private static readonly bool dropWeaponOnDead = false;
    private static readonly bool removeDeadPeds = true;
    private static readonly bool runToSpawnpoint = true;
    private static readonly int idleInterval = 500;
    private static readonly int battleInterval = 100;

    private int relationshipGroupAllies;
    private int relationshipGroupEnemies;
    private int originalWantedLevel;
    private List<Ped> spawnedAllies = new List<Ped>();
    private List<Ped> spawnedEnemies = new List<Ped>();
    private List<Ped> deadPeds = new List<Ped>();
    private List<Ped> pedsRemove = new List<Ped>();

    private enum CombatMovement {
        // https://runtime.fivem.net/doc/natives/?_0x4D9CA1009AFBD057
        Stationary = 0,
        Defensive = 1,
        Offensive = 2,
        Suicidal = 3
    }

    private enum Stage {
        Initial = 0,
        DefiningEnemySpawnpoint = 1,
        EnemySpawnpointDefined = 2,
        Running = 3,
        StopKeyPressed = 4
    }

    private Stage stage = Stage.Initial;
    private Vector3 spawnpointAllies;
    private Vector3 spawnpointEnemies;
    private Blip spawnpointBlipAllies;
    private Blip spawnpointBlipEnemies;
    private float spawnpointsDistance;

    private static Random random = new Random();

    public SimpleGangWar() {
        Tick += OnTick;
        KeyUp += OnKeyUp;
        Interval = idleInterval;

        relationshipGroupAllies = World.AddRelationshipGroup("simplegangwar_allies");
        relationshipGroupEnemies = World.AddRelationshipGroup("simplegangwar_enemies");
        int relationshipGroupPlayer = Game.Player.Character.RelationshipGroup;
        SetRelationshipBetweenGroups(Relationship.Hate, relationshipGroupAllies, relationshipGroupEnemies);
        SetRelationshipBetweenGroups(Relationship.Respect, relationshipGroupAllies, relationshipGroupAllies);
        SetRelationshipBetweenGroups(Relationship.Respect, relationshipGroupEnemies, relationshipGroupEnemies);
        SetRelationshipBetweenGroups(Relationship.Respect, relationshipGroupAllies, relationshipGroupPlayer);
        SetRelationshipBetweenGroups(Relationship.Hate, relationshipGroupEnemies, relationshipGroupPlayer);

        UI.Notify("SimpleGangWar loaded");
    }

    private void OnTick(object sender, EventArgs e) {
        if (stage >= Stage.Running) {
            SpawnPeds(true);
            SpawnPeds(false);
            
            ProcessSpawnedPeds(true);
            ProcessSpawnedPeds(false);
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e) {
        if (e.KeyCode == hotkey) {
            if (stage == Stage.Initial) {
                UI.ShowHelpMessage("Welcome to SimpleGangWar!\nGo to the enemy spawnpoint and press the hotkey again to define it.", 180000, true);
                stage = Stage.DefiningEnemySpawnpoint;
            } else if (stage == Stage.DefiningEnemySpawnpoint) {
                DefineSpawnpoint(false);
                UI.ShowHelpMessage("Enemy spawnpoint defined! Now go to the allied spawnpoint and press the hotkey again to define it.", 180000, true);
                stage = Stage.EnemySpawnpointDefined;
            } else if (stage == Stage.EnemySpawnpointDefined) {
                DefineSpawnpoint(true);
                SetupBattle();
                UI.ShowHelpMessage("The battle begins NOW!", 5000, true);
                stage = Stage.Running;
            } else if (stage == Stage.Running) {
                UI.ShowHelpMessage("Do you really want to stop the battle? Press the hotkey again to confirm.", 7000, true);
                stage = Stage.StopKeyPressed;
            } else if (stage == Stage.StopKeyPressed) {
                UI.ShowHelpMessage("The battle has ended!", 5000, true);
                stage = Stage.Initial;
                Teardown();
            }
        }
    }

    private void SetupBattle() {
        Interval = battleInterval;
        spawnpointsDistance = spawnpointEnemies.DistanceTo(spawnpointAllies);

        if (noWantedLevel) {
            originalWantedLevel = Game.MaxWantedLevel;
            Game.MaxWantedLevel = 0;
        }
    }

    private void SpawnPeds(bool alliedTeam) {
        List<Ped> spawnedPedsList = alliedTeam ? spawnedAllies : spawnedEnemies;

        while (spawnedPedsList.Count < maxPedsPerTeam) {
            SpawnRandomPed(alliedTeam);
        }
    }

    private Ped SpawnRandomPed(bool alliedTeam) {
        Vector3 pedPosition = alliedTeam ? spawnpointAllies : spawnpointEnemies;
        string pedName = RandomChoice<string>(alliedTeam ? pedsAllies : pedsEnemies);
        string weaponName = RandomChoice<string>(alliedTeam ? weaponsAllies : weaponsEnemies);
        PedHash pedSpawn;
        WeaponHash weaponGive;

        // TODO verify names from arrays on script startup
        if (!Enum.TryParse<PedHash>(pedName, out pedSpawn)) {
            throw new FormatException("Ped name " + pedName + " does not exist!");
        }
        if (!Enum.TryParse<WeaponHash>(weaponName, out weaponGive)) {
            throw new FormatException("Weapon name " + weaponName + " does not exist!");
        }

        Ped ped = World.CreatePed(pedSpawn, pedPosition);
        ped.Weapons.Give(weaponGive, 1, true, true);

        ped.Health = ped.MaxHealth = alliedTeam ? healthAllies : healthEnemies;
        ped.Armor = alliedTeam ? armorAllies : armorEnemies;
        ped.Money = 0;
        ped.Accuracy = alliedTeam ? accuracyAllies : accuracyEnemies;
        ped.RelationshipGroup = alliedTeam ? relationshipGroupAllies : relationshipGroupEnemies;
        ped.DropsWeaponsOnDeath = dropWeaponOnDead;

        Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46, true);  // force peds to fight
        Function.Call(Hash.SET_PED_SEEING_RANGE, ped, spawnpointsDistance);
        //Function.Call(Hash.SET_PED_COMBAT_RANGE, ped, 0);  // 0=near, 2=far
        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, ped, (int)(alliedTeam ? combatMovementAllies : combatMovementEnemies));

        if (showBlipsOnPeds) {
            Blip blip = ped.AddBlip();
            blip.Color = alliedTeam ? BlipColor.Blue : BlipColor.Orange;
            blip.Name = alliedTeam ? "Ally team member" : "Enemy team member";
            blip.Scale = 0.5f;
        }

        ped.Task.ClearAllImmediately();
        ped.AlwaysKeepTask = true;
        (alliedTeam ? spawnedAllies : spawnedEnemies).Add(ped);

        return ped;
    }

    private void ProcessSpawnedPeds(bool alliedTeam) {
        List<Ped> pedList = alliedTeam ? spawnedAllies : spawnedEnemies;

        foreach (Ped ped in pedList) {
            if (ped.IsDead) {
                ped.CurrentBlip.Remove();
                pedsRemove.Add(ped);
                deadPeds.Add(ped);
                if (removeDeadPeds) ped.MarkAsNoLongerNeeded();
            } else if (ped.IsIdle) {
                if (runToSpawnpoint) ped.Task.RunTo(alliedTeam ? spawnpointEnemies : spawnpointAllies);
                else ped.Task.FightAgainstHatedTargets(spawnpointsDistance);
            }
        }

        foreach (Ped ped in pedsRemove) {
            pedList.Remove(ped);
        }

        pedsRemove.Clear();
    }

    private void DefineSpawnpoint(bool alliedTeam) {
        Vector3 position = Game.Player.Character.Position;
        Blip blip = World.CreateBlip(position);

        if (alliedTeam) {
            spawnpointAllies = position;
            spawnpointBlipAllies = blip;
            blip.Sprite = BlipSprite.TargetA;
            blip.Color = BlipColor.Blue;
            blip.Name = "Ally spawnpoint";
        } else {
            spawnpointEnemies = position;
            spawnpointBlipEnemies = blip;
            blip.Sprite = BlipSprite.TargetE;
            blip.Color = BlipColor.Orange;
            blip.Name = "Enemy spawnpoint";
        }
    }

    private void TeardownPeds(List<Ped> pedList) {
        foreach (Ped ped in pedList) {
            if (ped.Exists()) ped.Delete();
        }
    }

    private void Teardown() {
        Interval = idleInterval;
        spawnpointBlipAllies.Remove();
        spawnpointBlipEnemies.Remove();

        TeardownPeds(spawnedAllies);
        TeardownPeds(spawnedEnemies);
        TeardownPeds(deadPeds);

        spawnedAllies.Clear();
        spawnedEnemies.Clear();
        deadPeds.Clear();

        if (noWantedLevel) Game.MaxWantedLevel = originalWantedLevel;
    }

    private void SetRelationshipBetweenGroups(Relationship relationship, int groupA, int groupB) {
        World.SetRelationshipBetweenGroups(relationship, groupA, groupB);
        World.SetRelationshipBetweenGroups(relationship, groupB, groupA);
    }

    private T RandomChoice<T>(T[] array) {
        return array[random.Next(0, array.Length)];
    }
}
