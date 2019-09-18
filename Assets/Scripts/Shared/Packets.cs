public class Packets
{
    public static readonly ushort Welcome = 0;

    public static readonly ushort Login = 1;
    public static readonly ushort Registration = 2;

    public static readonly ushort OnlinePlayersCount = 3;
    public static readonly ushort Play = 4;
    public static readonly ushort MatchmakingDone = 7;
    public static readonly ushort GameEnded = 8;
    public static readonly ushort PlayResponse = 10;
    public static readonly ushort WelcomeClient = 12;
    public static readonly ushort GameserverReady = 11;
    public static readonly ushort PlayerJoinedGameServer = 13;
    public static readonly ushort GameIsEmpty = 14;

    public static readonly ushort Spawn = 5;
    public static readonly ushort Despawn = 6;

    public static readonly ushort Move = 9;
    public static readonly ushort Jump = 15;
    public static readonly ushort AnimatePlayer = 16;
}