using System;
using System.Linq;
using TaleWorlds.Library;

namespace BannerlordEnhancedFramework.utils
{
    public class DebugUtils
    {
        
        public static bool IsDebugModeActive()
        {
            Boolean result = ConfigurationUtils.IsKeySet("enable_debug_mode") && ConfigurationUtils.GetValueAsBool("enable_debug_mode");
            // TODO - internal Bannerlord debug mode checking
            return result;
        }

        public static void LogAndPrintInfo(String message)
        {
            Debug.Print(message);
        }
        
        public static void LogAndPrintWarning(String message)
        {
            // TODO - I am not certain what logLevel does
            Debug.Print("WARNING: " + message, 0, Debug.DebugColor.Yellow);
        }
        
        public static void LogAndPrintError(String message)
        {
            Debug.Print("ERROR: " + message,0, Debug.DebugColor.Red);
        }

        public static void PrintMethodNames(Type classTypes)
        {
            foreach (var method in classTypes.GetMethods())
            {
                try
                {
                    var parameters = method.GetParameters();
                    var parameterDescriptions = string.Join(", ", method.GetParameters()
                        .Select(x => x.ParameterType + " " + x.Name)
                        .ToArray());

                    LogAndPrintInfo(method.ReturnType + " " + method.Name + " " + parameterDescriptions);
                } catch(Exception e) {}
            }
        }
    }
}