using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DarktideDiscord;

/// <summary>
/// Lua-callable functions for Discord Rich Presence.
/// </summary>
public static class LuaBindings
{
    /// <summary>
    /// DarktideDiscord.set_state(state)
    /// Sets the activity state text (second line).
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static int SetState(nint state)
    {
        var lua = DiscordPlugin.LuaEnv;
        if (lua is null)
            return 0;

        var value = lua.GetString(state, 1);
        if (!string.IsNullOrEmpty(value))
        {
            DiscordPlugin.SetState(value);
            lua.PushBool(state, true);
        }
        else
        {
            lua.PushBool(state, false);
        }

        return 1;
    }

    /// <summary>
    /// DarktideDiscord.set_details(details)
    /// Sets the activity details text (first line).
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static int SetDetails(nint state)
    {
        var lua = DiscordPlugin.LuaEnv;
        if (lua is null)
            return 0;

        var value = lua.GetString(state, 1);
        if (!string.IsNullOrEmpty(value))
        {
            DiscordPlugin.SetDetails(value);
            lua.PushBool(state, true);
        }
        else
        {
            lua.PushBool(state, false);
        }

        return 1;
    }

    /// <summary>
    /// DarktideDiscord.set_class(archetype, details)
    /// Sets the small image (class icon) and its tooltip.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static int SetClass(nint state)
    {
        var lua = DiscordPlugin.LuaEnv;
        if (lua is null)
            return 0;

        var archetype = lua.GetString(state, 1);
        var details = lua.GetString(state, 2);

        if (!string.IsNullOrEmpty(archetype) && !string.IsNullOrEmpty(details))
        {
            DiscordPlugin.SetClass(archetype, details);
            lua.PushBool(state, true);
        }
        else
        {
            lua.PushBool(state, false);
        }

        return 1;
    }

    /// <summary>
    /// DarktideDiscord.set_party_size(current, max)
    /// Sets the party size display (e.g., "2 of 4").
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static int SetPartySize(nint state)
    {
        var lua = DiscordPlugin.LuaEnv;
        if (lua is null)
            return 0;

        var current = (int)lua.GetNumber(state, 1);
        var max = (int)lua.GetNumber(state, 2);

        if (current > 0 && max > 0)
        {
            DiscordPlugin.SetPartySize(current, max);
            lua.PushBool(state, true);
        }
        else
        {
            lua.PushBool(state, false);
        }

        return 1;
    }

    /// <summary>
    /// DarktideDiscord.set_start_time()
    /// Sets the activity start timestamp to the current time.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static int SetStartTime(nint state)
    {
        var lua = DiscordPlugin.LuaEnv;
        if (lua is null)
            return 0;

        DiscordPlugin.SetStartTime();
        lua.PushBool(state, true);

        return 1;
    }

    /// <summary>
    /// DarktideDiscord.update()
    /// Sends the current activity to Discord.
    /// </summary>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static int Update(nint state)
    {
        DiscordPlugin.UpdateActivity();
        return 0;
    }
}
