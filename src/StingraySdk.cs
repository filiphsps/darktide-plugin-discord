using System.Runtime.InteropServices;
using System.Text;

namespace DarktideEnginePlugin;

/// <summary>
/// Managed wrapper for engine logging functionality.
/// </summary>
public sealed unsafe class EngineLogger
{
    private readonly NativeLoggerApi* _native;
    private readonly byte[] _tag;

    public EngineLogger(NativeLoggerApi* native, string tag)
    {
        _native = native;
        _tag = Encoding.UTF8.GetBytes(tag + '\0');
    }

    public void Info(string text)
    {
        if (_native is null || _native->LogInfo is null)
            return;

        WriteLog(_native->LogInfo, text);
    }

    public void Warn(string text)
    {
        if (_native is null || _native->LogWarn is null)
            return;

        WriteLog(_native->LogWarn, text);
    }

    public void Error(string text)
    {
        if (_native is null || _native->LogError is null)
            return;

        WriteLog(_native->LogError, text);
    }

    private void WriteLog(delegate* unmanaged[Cdecl]<byte*, byte*, void> fn, string text)
    {
        var encoded = Encoding.UTF8.GetBytes(text + '\0');
        fixed (byte* tagPtr = _tag)
        fixed (byte* textPtr = encoded)
        {
            fn(tagPtr, textPtr);
        }
    }
}

/// <summary>
/// Managed wrapper for Lua scripting integration.
/// </summary>
public sealed unsafe class LuaEnvironment
{
    private readonly NativeLuaApi* _native;

    public LuaEnvironment(NativeLuaApi* native) => _native = native;

    public void RegisterModuleMethod(
        string module,
        string method,
        delegate* unmanaged[Cdecl]<nint, int> handler)
    {
        if (_native is null || _native->AddModuleFunction is null)
            return;

        var moduleBytes = Encoding.UTF8.GetBytes(module + '\0');
        var methodBytes = Encoding.UTF8.GetBytes(method + '\0');

        fixed (byte* m = moduleBytes)
        fixed (byte* f = methodBytes)
        {
            _native->AddModuleFunction(m, f, (nint)handler);
        }
    }

    public string? GetString(nint state, int position)
    {
        if (_native is null || _native->ToLString is null)
            return null;

        nuint length = 0;
        var ptr = _native->ToLString(state, position, &length);

        if (ptr is null || length == 0)
            return null;

        return Encoding.UTF8.GetString(ptr, (int)length);
    }

    public void PushString(nint state, string value)
    {
        if (_native is null || _native->PushString is null)
            return;

        var encoded = Encoding.UTF8.GetBytes(value + '\0');
        fixed (byte* ptr = encoded)
        {
            _native->PushString(state, ptr);
        }
    }

    public void PushNil(nint state)
    {
        if (_native is not null && _native->PushNil is not null)
            _native->PushNil(state);
    }

    public void PushNumber(nint state, double value)
    {
        if (_native is not null && _native->PushNumber is not null)
            _native->PushNumber(state, value);
    }

    public void PushBool(nint state, bool value)
    {
        if (_native is not null && _native->PushBoolean is not null)
            _native->PushBoolean(state, value ? 1 : 0);
    }

    public double GetNumber(nint state, int position)
    {
        if (_native is null || _native->ToNumber is null)
            return 0;

        return _native->ToNumber(state, position);
    }

    public bool GetBool(nint state, int position)
    {
        if (_native is null || _native->ToBoolean is null)
            return false;

        return _native->ToBoolean(state, position) != 0;
    }

    public int GetStackDepth(nint state)
    {
        if (_native is null || _native->GetTop is null)
            return 0;

        return _native->GetTop(state);
    }

    public int GetValueType(nint state, int position)
    {
        if (_native is null || _native->Type is null)
            return -1;

        return _native->Type(state, position);
    }
}
