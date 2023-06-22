using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BannerlordEnhancedFramework.utils;

public static class ConfigurationUtils
{
    private static readonly Dictionary<string, object> _configValues = new();
    public static bool GameStartedOrLoaded = false;
    public static bool GameAssetsLoaded = false;

    public static void LoadValues()
    {
        // DEFAULT MODIFICATIONS START
        var currentAssemblyFullPathDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        _configValues.Add("enable_debug_mode", false);

        // load configurations
        var configFileData = File.ReadAllLines(currentAssemblyFullPathDirectory + "\\Config.txt")
            .Select(x => x.Split('='))
            .ToDictionary(x => x[0], x => x[1]);

        if (configFileData == null) throw new Exception("Config.txt must be set in mod folder");
        AddDataToConfigurationsList(configFileData);
    }

    private static void AddDataToConfigurationsList(Dictionary<string, string> configFileData)
    {
        foreach (var entry in configFileData)
        {
            DebugUtils.LogAndPrintInfo("set configuration param: key - " + entry.Key + " value - " + entry.Value);
            int newIntVal;
            float newFloatVal;
            bool newBoolVal;
            if (int.TryParse(entry.Value, out newIntVal) && !entry.Value.Contains("."))
            {
                DebugUtils.LogAndPrintInfo("configuration param added as int");
                _configValues[entry.Key] = newIntVal;
            }
            else if (float.TryParse(entry.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out newFloatVal))
            {
                DebugUtils.LogAndPrintInfo("configuration param added as float");
                _configValues[entry.Key] = newFloatVal;
            }

            if (bool.TryParse(entry.Value, out newBoolVal))
            {
                DebugUtils.LogAndPrintInfo("configuration param added as boolean");
                _configValues[entry.Key] = newBoolVal;
            }
            else
            {
                _configValues[entry.Key] = entry.Value;
            }
        }
    }

    private static Dictionary<string, object> GetConfigValues()
    {
        return _configValues;
    }

    public static float GetValueAsFloat(string configKeyName)
    {
        object outValueRaw;
        float result;
        if (!GetConfigValues().TryGetValue(configKeyName, out outValueRaw) || !float.TryParse(outValueRaw.ToString(),
                NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            // DO NOT TRY TO BE SMART, METHOD SHOULD BE USED WHEN ONE IS CERTAIN THAT VALUE/KEY EXISTS, THIS IS MOSTLY STATIC DATA
            throw new InvalidCastException(configKeyName + " config value invalid");
        return result;
    }

    public static int GetValueAsInt(string configKeyName)
    {
        int result;
        object outValueRaw;
        if (!GetConfigValues().TryGetValue(configKeyName, out outValueRaw) || !int.TryParse(outValueRaw.ToString(),
                NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
            throw new InvalidCastException(configKeyName + " config value invalid");
        return result;
    }

    public static bool GetValueAsBool(string configKeyName)
    {
        bool result;
        object outValueRaw;
        if (!GetConfigValues().TryGetValue(configKeyName, out outValueRaw) ||
            !bool.TryParse(outValueRaw.ToString(), out result))
            throw new InvalidCastException(configKeyName + " config value invalid");
        return result;
    }

    public static bool IsKeySet(string key)
    {
        return GetConfigValues().ContainsKey(key);
    }
}