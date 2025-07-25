# RimWorld Performance
All RimWorld performance tweaks in one place!

## Disclaimer
- Mod can be unistalled mid-game. But after saving the game, pawns can sometimes 'get lost' while work is in progress because the mod is designed to prevent corruption of the save file by removing mod-specific data each time the game saves by any means. The only exception is the mod settings, as these are irrelevant to the save files.

## Functionality
- Redesigned hauling is capable of handling tens of thousands of things at once. Hauling 10k things takes ~3 mins for 10 pawns.
- Tick rate, MergeHaul tweaks that improve late-game performance by reducing the number of game operations.

## Compatible mods:
- Allow Tools - mod automatically adjusts to haul urgently, no need for Haul+
- Combat Extended
- Any Extended Storage mod
## Incompatible mods:
- Pick Up and Haul - similar idea, but PUAH isn't late game optimized or huge scale
- Any mod that changes tick functionality
- Any "smart" construction/deconstruction mod

## Troubleshooting
- Check first in mod (if installed) **Dubs Performance Analyzer->Modders->RimWorldPerformance** for performance problems
- Enable debug mode in **Options->Mod Options->RimWorld Performance**.
- Play until the problem occurs
- Find **player.log** in **Options->General->Log file folder**, close game and report it on GitHub.
- Remember to disable debug mode afterward. Due to the nature of the mod, it doesn't produce logs; instead, it only enables verbose logging.