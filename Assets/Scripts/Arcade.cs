using System;

public static class Arcade
{
    public static bool IsRunningInArcadeMode()
    {
        return Environment.GetEnvironmentVariable("ARCADE_MODE") != null;
    }
}