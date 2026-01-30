using DarktideEnginePlugin;

namespace DarktideDiscord;

/// <summary>
/// Discord Rich Presence plugin for Darktide.
/// Exposes Lua functions to control Discord activity status.
/// </summary>
public static unsafe class DiscordPlugin
{
    public const string Identifier = "darktide-discord";
    public const string LuaModule = "DarktideDiscord";
    public const long DiscordAppId = 1111429477055090698;

    private static EngineLogger? s_logger;
    private static LuaEnvironment? s_lua;

    private static nint s_discordCore;
    private static DiscordSdk.IDiscordCore* s_coreInterface;
    private static DiscordSdk.IDiscordActivityManager* s_activityManager;
    private static DiscordSdk.Activity s_activity;
    private static bool s_initialized;

    public static EngineLogger? Log => s_logger;
    public static LuaEnvironment? LuaEnv => s_lua;

    public static void Bootstrap(NativeLoggerApi* loggerApi, NativeLuaApi* luaApi)
    {
        s_logger = new EngineLogger(loggerApi, Identifier);
        s_lua = new LuaEnvironment(luaApi);
    }

    public static void OnGameStart()
    {
        s_logger?.Info($"Initializing {Identifier}...");

        // Register Lua functions
        s_lua?.RegisterModuleMethod(LuaModule, "set_state", &LuaBindings.SetState);
        s_lua?.RegisterModuleMethod(LuaModule, "set_details", &LuaBindings.SetDetails);
        s_lua?.RegisterModuleMethod(LuaModule, "set_class", &LuaBindings.SetClass);
        s_lua?.RegisterModuleMethod(LuaModule, "set_party_size", &LuaBindings.SetPartySize);
        s_lua?.RegisterModuleMethod(LuaModule, "set_start_time", &LuaBindings.SetStartTime);
        s_lua?.RegisterModuleMethod(LuaModule, "update", &LuaBindings.Update);

        // Initialize Discord
        InitializeDiscord();
    }

    private static void InitializeDiscord()
    {
        try
        {
            var createParams = new DiscordSdk.DiscordCreateParams
            {
                ClientId = DiscordAppId,
                Flags = (ulong)DiscordSdk.CreateFlags.NoRequireDiscord,
            };

            var result = DiscordSdk.DiscordCreate(DiscordSdk.DiscordVersion, ref createParams, out s_discordCore);

            if (result != DiscordSdk.Result.Ok)
            {
                s_logger?.Warn($"Discord initialization failed: {result}");
                return;
            }

            s_coreInterface = (DiscordSdk.IDiscordCore*)s_discordCore;

            // Get activity manager
            var activityManagerPtr = s_coreInterface->GetActivityManager(s_discordCore);
            s_activityManager = (DiscordSdk.IDiscordActivityManager*)activityManagerPtr;

            // Set default activity
            s_activity = new DiscordSdk.Activity();
            fixed (byte* largeImage = s_activity.Assets.LargeImage)
            fixed (byte* largeText = s_activity.Assets.LargeText)
            {
                DiscordSdk.SetFixedString(largeImage, 128, "darktide");
                DiscordSdk.SetFixedString(largeText, 128, "Warhammer 40,000: Darktide");
            }

            s_initialized = true;
            s_logger?.Info("Discord initialized successfully");
        }
        catch (Exception ex)
        {
            s_logger?.Error($"Discord initialization error: {ex.Message}");
        }
    }

    public static void OnGameTick(float dt)
    {
        if (!s_initialized || s_coreInterface == null)
            return;

        try
        {
            s_coreInterface->RunCallbacks(s_discordCore);
        }
        catch
        {
            // Silently ignore callback errors
        }
    }

    public static void OnGameEnd()
    {
        s_logger?.Info($"Shutting down {Identifier}");

        // Properly destroy Discord core
        if (s_initialized && s_coreInterface != null && s_coreInterface->Destroy != nint.Zero)
        {
            var destroy = (delegate* unmanaged[Cdecl]<nint, void>)s_coreInterface->Destroy;
            destroy(s_discordCore);
        }

        s_initialized = false;
        s_discordCore = nint.Zero;
        s_coreInterface = null;
        s_activityManager = null;
        s_logger = null;
        s_lua = null;
    }

    public static void SetState(string state)
    {
        fixed (byte* ptr = s_activity.State)
        {
            DiscordSdk.SetFixedString(ptr, 128, state);
        }
    }

    public static void SetDetails(string details)
    {
        fixed (byte* ptr = s_activity.Details)
        {
            DiscordSdk.SetFixedString(ptr, 128, details);
        }
    }

    public static void SetClass(string archetype, string details)
    {
        fixed (byte* smallImage = s_activity.Assets.SmallImage)
        fixed (byte* smallText = s_activity.Assets.SmallText)
        {
            DiscordSdk.SetFixedString(smallImage, 128, archetype);
            DiscordSdk.SetFixedString(smallText, 128, details);
        }
    }

    public static void SetPartySize(int current, int max)
    {
        s_activity.Party.Size.CurrentSize = current;
        s_activity.Party.Size.MaxSize = max;
    }

    public static void SetStartTime()
    {
        s_activity.Timestamps.Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static void UpdateActivity()
    {
        if (!s_initialized || s_activityManager == null)
            return;

        try
        {
            fixed (DiscordSdk.Activity* activityPtr = &s_activity)
            {
                s_activityManager->UpdateActivity(
                    (nint)s_activityManager,
                    activityPtr,
                    nint.Zero,
                    nint.Zero);
            }
        }
        catch (Exception ex)
        {
            s_logger?.Error($"Failed to update Discord activity: {ex.Message}");
        }
    }
}
