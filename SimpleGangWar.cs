using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Windows.Forms;
using System.Collections.Generic;

public class SimpleGangWar : Script {

    // Peds: https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/PedHash.cs
    // Weapons: https://github.com/crosire/scripthookvdotnet/blob/d1827497495567d810986aa752f8d903853088fd/source/scripting_v2/GTA.Native/WeaponHash.cs
    private static readonly string[] pedsAllies = { "Families01GFY", "Famca01GMY", "Famdnf01GMY", "Famfor01GMY" };
    private static readonly string[] weaponsAllies = { "AssaultRifle", "CompactRifle", "DoubleBarrelShotgun", "SNSPistol" };
    private static readonly string[] pedsEnemies = { "BallaEast01GMY", "BallaOrig01GMY", "Ballas01GFY", "BallaSout01GMY" };
    private static readonly string[] weaponsEnemies = { "MicroSMG", "MachinePistol", "Gusenberg", "Musket", "SawnOffShotgun" };
    private static readonly int healthAllies = 75;
    private static readonly int armorAllies = 0;
    private static readonly int healthEnemies = 75;
    private static readonly int armorEnemies = 0;
    private static readonly int accuracyAllies = 5;
    private static readonly int accuracyEnemies = 5;

    private static readonly uint maxPedsPerTeam = 5;
    private static readonly Keys hotkey = Keys.B;
    private static readonly bool blips = true;
    private static readonly bool dropWeaponOnDead = false;
    private static readonly bool removeDeadPeds = true;

    private int relationshipGroupAllies;
    private int relationshipGroupEnemies;
    private List<Ped> spawnedAllies = new List<Ped>();
    private List<Ped> spawnedEnemies = new List<Ped>();
    private List<Ped> deadPeds = new List<Ped>();

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
        Interval = 500;

        relationshipGroupAllies = World.AddRelationshipGroup("simplegangwar_allies");
        relationshipGroupEnemies = World.AddRelationshipGroup("simplegangwar_enemies");
        World.SetRelationshipBetweenGroups(Relationship.Respect, relationshipGroupAllies, relationshipGroupAllies);
        World.SetRelationshipBetweenGroups(Relationship.Respect, relationshipGroupEnemies, relationshipGroupEnemies);
        World.SetRelationshipBetweenGroups(Relationship.Hate, relationshipGroupAllies, relationshipGroupEnemies);
        World.SetRelationshipBetweenGroups(Relationship.Respect, relationshipGroupAllies, Game.Player.Character.RelationshipGroup);
        World.SetRelationshipBetweenGroups(Relationship.Hate, relationshipGroupEnemies, Game.Player.Character.RelationshipGroup);

        UI.Notify("SimpleGangWar loaded");
    }

    private void OnTick(object sender, EventArgs e) {
        if (stage >= Stage.Running) {
            SpawnPeds(true);
            SpawnPeds(false);
            ProcessSpawnedPeds(spawnedAllies);
            ProcessSpawnedPeds(spawnedEnemies);
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
                spawnpointsDistance = spawnpointEnemies.DistanceTo(spawnpointAllies) * 1.5f;
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

    private void SpawnPeds(bool alliedTeam) {
        List<Ped> spawnedPedsList = alliedTeam ? spawnedAllies : spawnedEnemies;

        while (spawnedPedsList.Count < maxPedsPerTeam) {
            SpawnRandomPed(alliedTeam);
        }
    }

    private Ped SpawnRandomPed(bool alliedTeam) {
        Vector3 pedPosition = alliedTeam ? spawnpointAllies : spawnpointEnemies;
        string pedName = randomChoice<string>(alliedTeam ? pedsAllies : pedsEnemies);
        string weaponName = randomChoice<string>(alliedTeam ? weaponsAllies : weaponsEnemies);
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

        // TODO Set health, armor (they die on spawn if changed)
        //createdPed.Health = alliedTeam ? healthAllies : healthEnemies;
        //createdPed.Armor = createdPed.Armor = alliedTeam ? armorAllies : armorEnemies;
        ped.Money = 0;
        ped.Accuracy = alliedTeam ? accuracyAllies : accuracyEnemies;
        ped.RelationshipGroup = alliedTeam ? relationshipGroupAllies : relationshipGroupEnemies;
        ped.DropsWeaponsOnDeath = dropWeaponOnDead;
        Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 46, true);  // force peds to fight
        Function.Call(Hash.SET_PED_SEEING_RANGE, ped, spawnpointsDistance);
        //Function.Call(Hash.SET_PED_COMBAT_RANGE, ped, 2);  // combat range: Far

        if (blips) {
            Blip blip = ped.AddBlip();
            blip.Color = alliedTeam ? BlipColor.Blue : BlipColor.Orange;
            blip.Scale = 0.5f;
        }

        ped.Task.ClearAllImmediately();
        (alliedTeam ? spawnedAllies : spawnedEnemies).Add(ped);

        return ped;
    }

    private void ProcessSpawnedPeds(List<Ped> pedList) {
        List<Ped> pedsRemove = new List<Ped>();

        foreach (Ped ped in pedList) {
            if (ped.IsDead) {
                ped.CurrentBlip.Remove();
                pedsRemove.Add(ped);
                deadPeds.Add(ped);
                if (removeDeadPeds) ped.MarkAsNoLongerNeeded();
            } else if (ped.IsIdle) {
                // TODO peds can stuck at idle if spawnpointsDistance is too far; should have an optional task of RunTo
                ped.Task.FightAgainstHatedTargets(spawnpointsDistance);
            }
        }

        foreach (Ped ped in pedsRemove) {
            pedList.Remove(ped);
        }
    }

    private void DefineSpawnpoint(bool alliedTeam) {
        Vector3 position = Game.Player.Character.Position;
        Blip blip = World.CreateBlip(position);

        if (alliedTeam) {
            spawnpointAllies = position;
            spawnpointBlipAllies = blip;
            blip.Sprite = BlipSprite.TargetA;
            blip.Color = BlipColor.Blue;
        } else {
            spawnpointEnemies = position;
            spawnpointBlipEnemies = blip;
            blip.Sprite = BlipSprite.TargetE;
            blip.Color = BlipColor.Orange;
        }
    }

    private void TeardownPeds(List<Ped> pedList) {
        foreach (Ped ped in pedList) {
            if (ped.Exists()) ped.Delete();
        }
    }

    private void Teardown() {
        spawnpointBlipAllies.Remove();
        spawnpointBlipEnemies.Remove();

        TeardownPeds(spawnedAllies);
        TeardownPeds(spawnedEnemies);
        TeardownPeds(deadPeds);

        spawnedAllies.Clear();
        spawnedEnemies.Clear();
        deadPeds.Clear();
    }

    private T randomChoice<T>(T[] array) {
        return array[random.Next(0, array.Length)];
    }
}
