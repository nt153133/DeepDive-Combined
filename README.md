# [Deep Dive Combined][0]

[![Download Now!][1]][2]
[![Discord][3]][4]

**Deep Dive Combined** for [RebornBuddy][5] is a fork of [Deep Dive][6] with support for faster Palace of the Dead runs and Heaven on High. It also introduces quality-of-life settings like automatically switching jobs after reaching leveling goals.

## Installation

### Prerequisites

 * [RebornBuddy][5] with active license (paid)

### Setup

 1. Download the [latest version][2].
 2. On the `.zip` file, right click > `Properties` > `Unblock` > `Apply`.
 3. Unzip all contents into `RebornBuddy\BotBases\DeepDiveCombined\` so it looks like this:
```
RebornBuddy
└── BotBases
    └── DeepDiveCombined
        ├── Deep Dungeon.sln
        └── ...
```
 4. Start RebornBuddy as normal.
 5. From the BotBases dropdown, choose `Deep Dive Combined`.

⚠️ The original Deep Dive ships with Reborn Buddy itself, so take care not to extract this version into the existing `RebornBuddy\BotBases\DeepDive\` folder or Deep Dive Combined will get overwritten and disappear whenever RebornBuddy updates.

## Usage

Deep Dive Combined can be used solo or in a party. It repeats the chosen set of floors, starting from the closest entry floor if desired. Either the current job is leveled indefinitely, or a list of jobs can be configured with leveling goals.

To begin, choose the desired class and dungeon profile, invite any other characters to party, then press `Start` in RebornBuddy. The bot will automatically travel to the correct zone, handle save files and queuing, etc.

### Settings

![Settings Window](https://i.imgur.com/bd1FIYf.png)

ℹ️ When partying with other bots, use the same settings to avoid splitting up or extending the run.

 * **Profile:** Selects the dungeon and run strategy.
 * **Start at Floor 51/21:** Starts from the higher floors instead of floor 1 for better XP. Unlocked by clearing the story boss.
 * **Floor:** Target floor-set to run. Lower floors will be run to get here, if needed.
 * **Silver Chests:** Whether to loot silver chests, which boost stats and (in HoH) drop Magicite.
 * **Accursed Hoard:** Whether to loot the Accursed Hoard, which drops extra loot bags.
 * **Use Job List:** Level the listed jobs to specified levels, instead of current job indefinitely.
 * **Go Exit:** Prefer going directly to next floor instead of clearing everything.
 * **Stop Run:** Stops the bot after current run.

[0]: https://github.com/nt153133/DeepDive "Deep Dive Combined on GitHub"
[1]: https://img.shields.io/badge/-DOWNLOAD-success
[2]: https://github.com/nt153133/DeepDive/archive/master.zip "Download"
[3]: https://img.shields.io/badge/DISCORD-7389D8?logo=discord&logoColor=ffffff&labelColor=6A7EC2
[4]: https://discord.gg/bmgCq39 "Discord"
[5]: https://www.rebornbuddy.com/ "RebornBuddy"
[6]: https://github.com/zzi-zzi-zzi/DeepDive "Deep Dive"
