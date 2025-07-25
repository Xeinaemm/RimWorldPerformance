# RimWorld Performance
All RimWorld performance tweaks in one place!

## Functionality
- Redesigned hauling is capable of handling tens of thousands of things at once. Hauling 12k things takes ~5 mins for 10 pawns.
- Tick tweaks, that can save >100 TPS in late game by reducing number of game operations.
- (WIP) Construction/Deconstruction

## Compatible mods:
- Allow Tools - mod automatically adjust to haul urgently, no need for Haul+
- Combat Extended
- Any Extended Storage mod
## Incompatible mods:
- Pick Up and Haul - similar idea, but PUAH isn't late game optimized or huge scale
- Any mod that changes tick functionality
- Any "smart" construction/deconstruction mod

## Troubleshooting
- Enable debug mode in **Options->Mod Options->RimWorld Performance**.
- Extract **player.log** from **Options->General->Log file folder** and report it on GitHub.
- Remember to disable debug mode aftewards.