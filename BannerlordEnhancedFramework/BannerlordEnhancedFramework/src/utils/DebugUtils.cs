using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils;

public class DebugUtils
{
    public static bool IsDebugModeActive()
    {
        var result = ConfigurationUtils.IsKeySet("enable_debug_mode") &&
                     ConfigurationUtils.GetValueAsBool("enable_debug_mode");
        // TODO - internal Bannerlord debug mode checking
        return result;
    }

    public static void LogAndPrintInfo(string message)
    {
        message = PrefixMessageWithModuleName(message);
        Debug.Print(message);
        DisplayMessageInGameConsole(message, Colors.White);
    }

    public static void LogAndPrintWarning(string message)
    {
        message = PrefixMessageWithModuleName(message);

        // TODO - I am not certain what logLevel does
        Debug.Print(message, 0, Debug.DebugColor.Yellow);
        DisplayMessageInGameConsole(message, Colors.Yellow);
    }

    public static void LogAndPrintError(string message)
    {
        message = PrefixMessageWithModuleName(message);
        DisplayMessageInGameConsole(message, Colors.Red);
        Debug.Print("ERROR: " + message, 0, Debug.DebugColor.Red);
    }

    private static string PrefixMessageWithModuleName(string message)
    {
        return Assembly.GetExecutingAssembly().GetName().Name + " - " + message;
    }

    private static void DisplayMessageInGameConsole(string msg, Color color)
    {
        var infoMsg = new InformationMessage(msg, color);
        InformationManager.DisplayMessage(infoMsg);
    }

    public static void PrintMethodNames(Type classTypes)
    {
        foreach (var method in classTypes.GetMethods())
            try
            {
                var parameters = method.GetParameters();
                var parameterDescriptions = string.Join(", ", method.GetParameters()
                    .Select(x => x.ParameterType + " " + x.Name)
                    .ToArray());

                LogAndPrintInfo(method.ReturnType + " " + method.Name + " " + parameterDescriptions);
            }
            catch (Exception e)
            {
            }
    }
}