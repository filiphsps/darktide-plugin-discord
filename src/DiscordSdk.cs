using System.Runtime.InteropServices;
using System.Text;

namespace DarktideDiscord;

/// <summary>
/// Discord Game SDK bindings.
/// The discord_game_sdk.dll must be placed alongside the plugin DLL.
/// </summary>
public static unsafe class DiscordSdk
{
    private const string DllName = "discord_game_sdk";

    public enum Result
    {
        Ok = 0,
        ServiceUnavailable = 1,
        InvalidVersion = 2,
        LockFailed = 3,
        InternalError = 4,
        InvalidPayload = 5,
        InvalidCommand = 6,
        InvalidPermissions = 7,
        NotFetched = 8,
        NotFound = 9,
        Conflict = 10,
        InvalidSecret = 11,
        InvalidJoinSecret = 12,
        NoEligibleActivity = 13,
        InvalidInvite = 14,
        NotAuthenticated = 15,
        InvalidAccessToken = 16,
        ApplicationMismatch = 17,
        InvalidDataUrl = 18,
        InvalidBase64 = 19,
        NotFiltered = 20,
        LobbyFull = 21,
        InvalidLobbySecret = 22,
        InvalidFilename = 23,
        InvalidFileSize = 24,
        InvalidEntitlement = 25,
        NotInstalled = 26,
        NotRunning = 27,
        InsufficientBuffer = 28,
        PurchaseCanceled = 29,
        InvalidGuild = 30,
        InvalidEvent = 31,
        InvalidChannel = 32,
        InvalidOrigin = 33,
        RateLimited = 34,
        OAuth2Error = 35,
        SelectChannelTimeout = 36,
        GetGuildTimeout = 37,
        SelectVoiceForceRequired = 38,
        CaptureShortcutAlreadyListening = 39,
        UnauthorizedForAchievement = 40,
        InvalidGiftCode = 41,
        PurchaseError = 42,
        TransactionAborted = 43,
    }

    [Flags]
    public enum CreateFlags : ulong
    {
        Default = 0,
        NoRequireDiscord = 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ActivityTimestamps
    {
        public long Start;
        public long End;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ActivityAssets
    {
        public fixed byte LargeImage[128];
        public fixed byte LargeText[128];
        public fixed byte SmallImage[128];
        public fixed byte SmallText[128];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PartySize
    {
        public int CurrentSize;
        public int MaxSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ActivityParty
    {
        public fixed byte Id[128];
        public PartySize Size;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ActivitySecrets
    {
        public fixed byte Match[128];
        public fixed byte Join[128];
        public fixed byte Spectate[128];
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Activity
    {
        public int ApplicationId;
        public fixed byte Name[128];
        public fixed byte State[128];
        public fixed byte Details[128];
        public ActivityTimestamps Timestamps;
        public ActivityAssets Assets;
        public ActivityParty Party;
        public ActivitySecrets Secrets;
        public int Instance;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DiscordCreateParams
    {
        public long ClientId;
        public ulong Flags;
        public nint Events;
        public nint EventData;
        public nint ActivityEvents;
        public nint ActivityEventData;
        public nint RelationshipEvents;
        public nint RelationshipEventData;
        public nint LobbyEvents;
        public nint LobbyEventData;
        public nint NetworkEvents;
        public nint NetworkEventData;
        public nint OverlayEvents;
        public nint OverlayEventData;
        public nint StorageEvents;
        public nint StorageEventData;
        public nint StoreEvents;
        public nint StoreEventData;
        public nint VoiceEvents;
        public nint VoiceEventData;
        public nint AchievementEvents;
        public nint AchievementEventData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IDiscordActivityManager
    {
        public nint RegisterCommand;
        public nint RegisterSteam;
        public delegate* unmanaged[Cdecl]<nint, Activity*, nint, nint, void> UpdateActivity;
        public nint ClearActivity;
        public nint SendRequestReply;
        public nint SendInvite;
        public nint AcceptInvite;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IDiscordCore
    {
        public nint Destroy;
        public delegate* unmanaged[Cdecl]<nint, Result> RunCallbacks;
        public nint SetLogHook;
        public nint GetApplicationManager;
        public nint GetUserManager;
        public nint GetImageManager;
        public delegate* unmanaged[Cdecl]<nint, nint> GetActivityManager;
        public nint GetRelationshipManager;
        public nint GetLobbyManager;
        public nint GetNetworkManager;
        public nint GetOverlayManager;
        public nint GetStorageManager;
        public nint GetStoreManager;
        public nint GetVoiceManager;
        public nint GetAchievementManager;
    }

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Result DiscordCreate(int version, ref DiscordCreateParams parameters, out nint core);

    public const int DiscordVersion = 2;

    // Helper methods for setting fixed buffer strings
    public static void SetFixedString(byte* buffer, int maxLen, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            buffer[0] = 0;
            return;
        }

        // Use stackalloc to avoid heap allocation
        var maxBytes = Encoding.UTF8.GetMaxByteCount(value.Length);
        Span<byte> bytes = maxBytes <= 256
            ? stackalloc byte[maxBytes]
            : new byte[maxBytes];

        var written = Encoding.UTF8.GetBytes(value.AsSpan(), bytes);
        var len = Math.Min(written, maxLen - 1);

        for (int i = 0; i < len; i++)
            buffer[i] = bytes[i];
        buffer[len] = 0;
    }
}
