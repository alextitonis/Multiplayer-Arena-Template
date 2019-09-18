public enum LogType
{
    Info = 0,
    Warning,
    Error,
}

public enum Screen
{
    Starting = 0,
    Login,
    Registration,
    ForgotPassword,
    Lobby,
    InGame,
    Disconnected
}

public enum Gender
{
    Male = 0,
    Female = 1,
}

public enum GameMode
{
    _test = 0,
    _3Players = 1,
    _5Players = 2,
}

public enum AccountState
{
    All = -1,
    InLobby = 0,
    InGame = 1,
}