# Darktide Discord Rich Presence

A native plugin for Warhammer 40,000: Darktide that displays your in-game activity on Discord.

## Features

- Shows current activity (mission, lobby, etc.) in Discord status
- Displays character class with icon
- Shows party size
- Tracks session duration
- Works without requiring Discord to be running (graceful fallback)
- Lightweight ~19KB native DLL

## Requirements

- Warhammer 40,000: Darktide
- Discord desktop application (for presence display)
- `discord_game_sdk.dll` (Discord Game SDK library)

## Installation

1. Download the latest `DarktideDiscord.dll` from releases
2. Download `discord_game_sdk.dll` from the [Discord Developer Portal](https://discord.com/developers/docs/game-sdk/sdk-starter-guide) (or use the one bundled with release)
3. Copy both files to your Darktide plugins directory:
   ```
   [Darktide Installation]/binaries/plugins/
   ```

## Building from Source

### Prerequisites

- .NET 10 SDK
- Windows x64 (or Docker for cross-compilation)

### Build Commands

**Windows:**
```bash
dotnet publish -c Release -r win-x64
```

**Linux/macOS (via Docker):**
```bash
docker build -t darktide-discord-builder .
docker create --name extract darktide-discord-builder
docker cp extract:/DarktideDiscord.dll ./
docker rm extract
```

Output: `bin/Release/net8.0/win-x64/publish/DarktideDiscord.dll`

## Lua API for Mod Authors

This plugin exposes a `DarktideDiscord` module to Lua scripts. Use these functions to update the player's Discord presence from your mods.

### Functions

#### `DarktideDiscord.set_details(text)`
Sets the first line of the Discord presence (main activity description).

```lua
DarktideDiscord.set_details("Mission: Assassination")
```

#### `DarktideDiscord.set_state(text)`
Sets the second line of the Discord presence (additional context).

```lua
DarktideDiscord.set_state("Difficulty: Heresy")
```

#### `DarktideDiscord.set_class(archetype, description)`
Sets the small icon and tooltip showing the player's class. The `archetype` parameter should match your Discord application's asset names.

```lua
DarktideDiscord.set_class("psyker", "Psyker - Adept")
```

#### `DarktideDiscord.set_party_size(current, max)`
Displays party information (e.g., "2 of 4").

```lua
DarktideDiscord.set_party_size(2, 4)
```

#### `DarktideDiscord.set_start_time()`
Records the current time as the activity start. Discord will display elapsed time automatically.

```lua
DarktideDiscord.set_start_time()
```

#### `DarktideDiscord.update()`
Sends all pending changes to Discord. Call this after setting your desired values.

```lua
DarktideDiscord.update()
```

### Example Usage

```lua
-- Called when entering a mission
local function update_discord_presence(mission_name, difficulty, class_name, party_count)
    DarktideDiscord.set_details("Mission: " .. mission_name)
    DarktideDiscord.set_state("Difficulty: " .. difficulty)
    DarktideDiscord.set_class(class_name:lower(), class_name)
    DarktideDiscord.set_party_size(party_count, 4)
    DarktideDiscord.set_start_time()
    DarktideDiscord.update()
end

-- Called when returning to hub
local function clear_mission_presence()
    DarktideDiscord.set_details("In the Mourningstar")
    DarktideDiscord.set_state("")
    DarktideDiscord.update()
end
```

## Discord Application Setup

To use your own Discord application:

1. Create an application at [Discord Developer Portal](https://discord.com/developers/applications)
2. Note your Application ID
3. Upload Rich Presence assets (images for classes, game logo, etc.)
4. Modify the `DiscordAppId` constant in `Plugin.cs`
5. Rebuild the plugin

### Default Assets

The plugin expects these Discord assets to be configured:
- `darktide` - Large image (game logo)
- Class icons matching the archetype names passed to `set_class()`

## Technical Details

### Architecture

```
Darktide Engine
    ↓
Exports.cs (native entry points)
    ↓
Plugin.cs (lifecycle management)
    ↓
LuaBindings.cs (Lua function exports)
    ↓
DiscordSdk.cs (P/Invoke bindings)
    ↓
discord_game_sdk.dll
    ↓
Discord Client
```

### Plugin Lifecycle

1. **Loaded** - DLL loaded by engine, resolver stored
2. **SetupGame** - Plugin initialized, Discord SDK created, Lua functions registered
3. **UpdateGame** - Called each frame, processes Discord callbacks
4. **ShutdownGame** - Cleanup on game exit

### Build Configuration

- **Framework:** .NET 10.0
- **Output:** Native Windows x64 DLL via NativeAOT
- **Size optimizations:** Symbol stripping, metadata trimming
- **Calling convention:** Cdecl (C ABI compatible)

## Troubleshooting

**Discord status not updating:**
- Ensure Discord desktop app is running
- Check that `discord_game_sdk.dll` is in the plugins folder
- Verify the plugin loaded (check game logs)

**Plugin not loading:**
- Confirm DLL is in `[Darktide]/binaries/plugins/`
- Ensure you're using the Windows x64 build

**Crashes on startup:**
- Missing `discord_game_sdk.dll`
- Version mismatch with Discord SDK

## Credits

- Plugin API structures based on [rawray](https://github.com/thewhitegoatcb/rawray) by thewhitegoatcb
- Built using the [Discord Game SDK](https://discord.com/developers/docs/game-sdk/sdk-starter-guide)

## License

MIT License
