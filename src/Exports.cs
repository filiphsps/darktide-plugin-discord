using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DarktideEnginePlugin;

namespace DarktideDiscord;

/// <summary>
/// Native entry points exposed to the Darktide engine.
/// </summary>
public static unsafe class NativeExports
{
    private static readonly byte[] s_nameBuffer = Encoding.UTF8.GetBytes(DiscordPlugin.Identifier + '\0');
    private static NativePluginInterface s_interface;
    private static EngineApiResolver? s_resolver;
    private static bool s_ready;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static byte* GetName()
    {
        fixed (byte* p = s_nameBuffer)
            return p;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static void Loaded(nint resolverPtr)
    {
        s_resolver = Marshal.GetDelegateForFunctionPointer<EngineApiResolver>(resolverPtr);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static void SetupGame(nint resolverPtr)
    {
        if (s_ready)
            return;

        s_resolver ??= Marshal.GetDelegateForFunctionPointer<EngineApiResolver>(resolverPtr);

        var logApi = (NativeLoggerApi*)s_resolver((uint)EngineApiType.Logging);
        var luaApi = (NativeLuaApi*)s_resolver((uint)EngineApiType.Lua);

        DiscordPlugin.Bootstrap(logApi, luaApi);
        DiscordPlugin.OnGameStart();

        s_ready = true;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static void UpdateGame(float dt) => DiscordPlugin.OnGameTick(dt);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static void ShutdownGame()
    {
        DiscordPlugin.OnGameEnd();
        s_ready = false;
    }

    /// <summary>
    /// Main plugin entry point called by the engine.
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "get_plugin_api", CallConvs = [typeof(CallConvCdecl)])]
    public static NativePluginInterface* GetPluginApi(uint apiId)
    {
        if (apiId != (uint)EngineApiType.Plugin)
            return null;

        s_interface = new NativePluginInterface
        {
            Version = 128,
            Flags = 1,

            Loaded = (nint)(delegate* unmanaged[Cdecl]<nint, void>)&Loaded,
            SetupGame = (nint)(delegate* unmanaged[Cdecl]<nint, void>)&SetupGame,
            UpdateGame = (nint)(delegate* unmanaged[Cdecl]<float, void>)&UpdateGame,
            ShutdownGame = (nint)(delegate* unmanaged[Cdecl]<void>)&ShutdownGame,
            GetName = (nint)(delegate* unmanaged[Cdecl]<byte*>)&GetName,
        };

        fixed (NativePluginInterface* p = &s_interface)
            return p;
    }
}
