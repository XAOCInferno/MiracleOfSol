using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class eLogType
{

    public const byte Info = 0;
    public const byte Warning = 1;
    public const byte Error = 2;

}

public static class eLogVerbosity
{

    public const byte Full = 0;
    public const byte Verbose = 1;
    public const byte Simple = 2;

}

public static class Dbg
{

    public static void Log(string message, byte? logType, byte? logVerbosity)
    {
        message = InterpreteMessage(logVerbosity, message);

        switch (logType.Value)
        {

            case eLogType.Info:

                LogInfo(message);
                break;

            case eLogType.Warning:

                LogWarning(message);
                break;

            case eLogType.Error:

                LogError(message);
                break;

            default:

                LogInfo(message);
                break;

        }

    }

    private static string InterpreteMessage(byte? logVerbosity, string message)
    {
        string finalMessage;

        switch (logVerbosity.Value)
        {

            case eLogVerbosity.Full:

                finalMessage = message + "\n\n" + Environment.StackTrace;
                break;

            case eLogVerbosity.Verbose:

                finalMessage = message + "\n\n" + Environment.StackTrace;
                break;

            case eLogVerbosity.Simple:

                finalMessage = message;
                break;

            default:

                finalMessage = message;
                break;

        }


        return finalMessage;

    }

    private static void LogInfo(string message)
    {

        Debug.Log(message);
        DumpToFile(message);

    }

    private static void LogWarning(string message)
    {

        Debug.LogWarning(message);
        DumpToFile(message);

    }

    private static void LogError(string message)
    {

        Debug.LogError(message);
        DumpToFile(message);

    }

    private static void DumpToFile(string message)
    {

        //Add this later

    }
}
