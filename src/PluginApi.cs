using System.Runtime.InteropServices;

namespace DarktideEnginePlugin;

/// <summary>
/// API IDs for engine services.
/// Based on rawray PluginApi128.h
/// </summary>
public enum EngineApiType : uint
{
    Plugin = 0,
    Lua = 1,
    DataCompiler = 2,
    DataCompileParameters = 3,
    Allocator = 4,
    ResourceManager = 5,
    Logging = 6,
    // 7 is unused
    Unit = 8,
    SceneGraph = 9,
    FutureInputArchive = 10,
    InputArchive = 11,
    Application = 12,
    ApplicationOptions = 13,
    UnitReference = 14,
    ErrorContext = 15,
    RenderInterface = 16,
    Raycast = 17,
    RenderCallbacks = 18,
    RenderOverrides = 19,
    Filesystem = 20,
    PluginManager = 21,
    World = 22,
    LineObjectDrawer = 23,
    Profiler = 24,
    Error = 25,
    RenderBuffer = 26,
    Mesh = 27,
    InputBuffer = 28,
    RenderSceneGraph = 29,
    SoundStreamSource = 30,
    CApi = 31,
    Thread = 32,
    Timer = 33,
    Material = 34,
    SceneDatabase = 35,
    StreamCapture = 36,
    FlowNodes = 37,
    Camera = 38,
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void* EngineApiResolver(uint apiId);

/// <summary>
/// Main plugin interface for Darktide/Vermintide 2.
/// Based on rawray PluginApi128.h (version 128)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct NativePluginInterface
{
    public uint Version;  // = 128
    public uint Flags;    // = 1 for lua plugin

    public nint Loaded;
    public nint StartReload;
    public nint Unloaded;
    public nint FinishReload;
    public nint SetupResources;
    public nint ShutdownResources;
    public nint SetupGame;
    public nint UpdateGame;
    public nint ShutdownGame;
    public nint UnregisterWorld;
    public nint RegisterWorld;
    public nint GetHash;
    public nint GetName;
    public nint UnkFunc13;
    public nint UnkFunc14;
    public nint UnkFunc15;
    public nint DebugDraw;
}

/// <summary>
/// Logging API
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct NativeLoggerApi
{
    public delegate* unmanaged[Cdecl]<byte*, byte*, void> LogInfo;
    public delegate* unmanaged[Cdecl]<byte*, byte*, void> LogWarn;
    public delegate* unmanaged[Cdecl]<byte*, byte*, void> LogError;
}

/// <summary>
/// Lua API for Darktide/Vermintide 2.
/// Based on rawray PluginApi128.h (LuaApi128)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct NativeLuaApi
{
    // ---- Stingray Lua extensions ----
    public delegate* unmanaged[Cdecl]<byte*, byte*, nint, void> AddModuleFunction;
    public delegate* unmanaged[Cdecl]<byte*, byte*, int, void> SetModuleBool;
    public delegate* unmanaged[Cdecl]<byte*, byte*, double, void> SetModuleNumber;
    public delegate* unmanaged[Cdecl]<byte*, byte*, byte*, void> SetModuleString;
    public delegate* unmanaged[Cdecl]<byte*, byte*, nint, byte*, void> DeprecatedWarning;
    public nint AddConsoleCommand; // varargs

    // ---- State manipulation ----
    public nint NewState;
    public delegate* unmanaged[Cdecl]<nint, void> Close;
    public delegate* unmanaged[Cdecl]<nint, nint> NewThread;
    public delegate* unmanaged[Cdecl]<nint, nint, nint> AtPanic;

    // ---- Basic stack manipulation ----
    public delegate* unmanaged[Cdecl]<nint, int> GetTop;
    public delegate* unmanaged[Cdecl]<nint, int, void> SetTop;
    public delegate* unmanaged[Cdecl]<nint, int, void> PushValue;
    public delegate* unmanaged[Cdecl]<nint, int, void> Remove;
    public delegate* unmanaged[Cdecl]<nint, int, void> Insert;
    public delegate* unmanaged[Cdecl]<nint, int, void> Replace;
    public delegate* unmanaged[Cdecl]<nint, int, int> CheckStack;
    public delegate* unmanaged[Cdecl]<nint, nint, int, void> XMove;

    // ---- Access functions ----
    public delegate* unmanaged[Cdecl]<nint, int, int> IsNumber;
    public delegate* unmanaged[Cdecl]<nint, int, int> IsString;
    public delegate* unmanaged[Cdecl]<nint, int, int> IsCFunction;
    public delegate* unmanaged[Cdecl]<nint, int, int> IsUserData;
    public delegate* unmanaged[Cdecl]<nint, int, int> Type;
    public delegate* unmanaged[Cdecl]<nint, int, byte*> TypeName;

    public delegate* unmanaged[Cdecl]<nint, int, int, int> Equal;
    public delegate* unmanaged[Cdecl]<nint, int, int, int> RawEqual;
    public delegate* unmanaged[Cdecl]<nint, int, int, int> LessThan;

    public delegate* unmanaged[Cdecl]<nint, int, double> ToNumber;
    public delegate* unmanaged[Cdecl]<nint, int, nint> ToInteger;
    public delegate* unmanaged[Cdecl]<nint, int, int> ToBoolean;
    public delegate* unmanaged[Cdecl]<nint, int, nuint*, byte*> ToLString;
    public delegate* unmanaged[Cdecl]<nint, int, nuint> ObjLen;
    public delegate* unmanaged[Cdecl]<nint, int, nint> ToCFunction;
    public delegate* unmanaged[Cdecl]<nint, int, void*> ToUserData;
    public delegate* unmanaged[Cdecl]<nint, int, nint> ToThread;
    public delegate* unmanaged[Cdecl]<nint, int, void*> ToPointer;

    // ---- Push functions ----
    public delegate* unmanaged[Cdecl]<nint, void> PushNil;
    public delegate* unmanaged[Cdecl]<nint, double, void> PushNumber;
    public delegate* unmanaged[Cdecl]<nint, nint, void> PushInteger;
    public delegate* unmanaged[Cdecl]<nint, byte*, nuint, void> PushLString;
    public delegate* unmanaged[Cdecl]<nint, byte*, void> PushString;
    public nint PushVFString; // varargs
    public nint PushFString;  // varargs
    public delegate* unmanaged[Cdecl]<nint, nint, int, void> PushCClosure;
    public delegate* unmanaged[Cdecl]<nint, int, void> PushBoolean;
    public delegate* unmanaged[Cdecl]<nint, void*, void> PushLightUserData;
    public delegate* unmanaged[Cdecl]<nint, int> PushThread;

    // ---- Get functions ----
    public delegate* unmanaged[Cdecl]<nint, int, void> GetTable;
    public delegate* unmanaged[Cdecl]<nint, int, byte*, void> GetField;
    public delegate* unmanaged[Cdecl]<nint, int, void> RawGet;
    public delegate* unmanaged[Cdecl]<nint, int, int, void> RawGetI;
    public delegate* unmanaged[Cdecl]<nint, int, int, void> CreateTable;
    public delegate* unmanaged[Cdecl]<nint, nuint, void*> NewUserData;
    public delegate* unmanaged[Cdecl]<nint, int, int> GetMetaTable;
    public delegate* unmanaged[Cdecl]<nint, int, void> GetFEnv;

    // ---- Set functions ----
    public delegate* unmanaged[Cdecl]<nint, int, void> SetTable;
    public delegate* unmanaged[Cdecl]<nint, int, byte*, void> SetField;
    public delegate* unmanaged[Cdecl]<nint, int, void> RawSet;
    public delegate* unmanaged[Cdecl]<nint, int, int, void> RawSetI;
    public delegate* unmanaged[Cdecl]<nint, int, int> SetMetaTable;
    public delegate* unmanaged[Cdecl]<nint, int, int> SetFEnv;

    // ---- Load and call functions ----
    public delegate* unmanaged[Cdecl]<nint, int, int, void> Call;
    public delegate* unmanaged[Cdecl]<nint, int, int, int, int> PCall;
    public delegate* unmanaged[Cdecl]<nint, nint, void*, int> CPCall;
    public nint Load;
    public nint Dump;

    // ---- Coroutine functions ----
    public delegate* unmanaged[Cdecl]<nint, int, int> Yield;
    public delegate* unmanaged[Cdecl]<nint, int, int> Resume;
    public delegate* unmanaged[Cdecl]<nint, int> Status;

    // ---- Garbage collection ----
    public delegate* unmanaged[Cdecl]<nint, int, int, int> GC;

    // ---- Miscellaneous ----
    public delegate* unmanaged[Cdecl]<nint, int> Error;
    public delegate* unmanaged[Cdecl]<nint, int, int> Next;
    public delegate* unmanaged[Cdecl]<nint, int, void> Concat;

    // ---- Debugging ----
    public nint GetStack;
    public nint GetInfo;
    public nint GetLocal;
    public nint SetLocal;
    public nint GetUpvalue;
    public nint SetUpvalue;
    public nint SetHook;
    public nint GetHook;
    public nint GetHookMask;
    public nint GetHookCount;

    // ---- Library functions ----
    public nint LibOpenLib;
    public nint LibRegister;
    public nint LibGetMetaField;
    public nint LibCallMeta;
    public nint LibTypeError;
    public nint LibArgError;
    public nint LibCheckLString;
    public nint LibOptLString;
    public nint LibCheckNumber;
    public nint LibOptNumber;
    public nint LibCheckInteger;
    public nint LibOptInteger;
    public nint LibCheckStack;
    public nint LibCheckType;
    public nint LibCheckAny;
    public nint LibNewMetaTable;
    public nint LibCheckUData;
    public nint LibWhere;
    public nint LibError;
    public nint LibCheckOption;
    public nint LibRef;
    public nint LibUnref;
    public nint LibLoadFile;
    public nint LibLoadBuffer;
    public nint LibLoadString;
    public nint LibNewState;
    public nint LibGSub;
    public nint LibFindTable;
    public nint LibOpenLibs;

    // ---- Stingray extensions (continued) ----
    public delegate* unmanaged[Cdecl]<nint, int, int> GetIndex;
    public delegate* unmanaged[Cdecl]<nint, int, void> PushIndex;
    public delegate* unmanaged[Cdecl]<nint, int, int> GetIndex01Based;
    public delegate* unmanaged[Cdecl]<nint, int, void> PushIndex01Based;

    // Vector/Quaternion/Matrix operations
    public delegate* unmanaged[Cdecl]<nint, float*, void> PushVector3;
    public delegate* unmanaged[Cdecl]<nint, float*, void> PushQuaternion;
    public delegate* unmanaged[Cdecl]<nint, float*, void> PushMatrix4x4;
    public delegate* unmanaged[Cdecl]<nint, float*, void> PushVector3Box;
    public delegate* unmanaged[Cdecl]<nint, int, float*> GetVector3;
    public delegate* unmanaged[Cdecl]<nint, int, float*> GetVector4;
    public delegate* unmanaged[Cdecl]<nint, int, float*> GetQuaternion;
    public delegate* unmanaged[Cdecl]<nint, int, float*> GetVector3Box;
    public delegate* unmanaged[Cdecl]<nint, int, void*> GetUnit;
    public delegate* unmanaged[Cdecl]<nint> GetScriptEnvironmentState;

    // Type checks
    public delegate* unmanaged[Cdecl]<nint, int, int> IsTable;
    public delegate* unmanaged[Cdecl]<nint, int, int> IsVector3_;
    public delegate* unmanaged[Cdecl]<nint, int, int> IsVector3;
    public delegate* unmanaged[Cdecl]<nint, int, int> IsQuaternion;
    public delegate* unmanaged[Cdecl]<nint, int, int> IsMatrix4x4;
}
