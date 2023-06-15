using System;
using System.Reflection;
using HarmonyLib;

namespace BannerlordEnhancedFramework.utils;

public class HarmonyPatcher
{
    private Harmony _harmony;

    public HarmonyPatcher(String id)
    {
        this._harmony = new Harmony(id);
    }
    
    public void PatchMethod(MethodInfo original, MethodInfo prefix)
    {
        this._harmony.Patch(original, new HarmonyMethod(prefix));
    }
    
    public void PatchMethodPostfix(MethodInfo original, MethodInfo postfix)
    {
        this._harmony.Patch(original, null, new HarmonyMethod(postfix));
    }
    
    public void PatchMethod(MethodInfo original, MethodInfo prefix, MethodInfo postfix)
    {
        this._harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
    }
}