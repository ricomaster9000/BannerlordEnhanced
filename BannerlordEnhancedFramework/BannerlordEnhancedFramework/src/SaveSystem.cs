using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BannerlordEnhancedFramework.utils;
using Newtonsoft.Json.Linq;

namespace BannerlordEnhancedFramework;

public class SaveSystem
{
    private static readonly string WindowsUsername = Environment.UserName;
    
    public static void SaveData() {
        IDictionary<String,JObject> listOfClassDataToSave = new IDictionary<String,JObject>();
        List<Type> relevantTypes = GetRelevantClassTypesForSyncingData();

        foreach(Type type in relevantTypes) {
            List<FieldInfo> relevantStaticFields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            JObject classDataHolder = new JObject();
            foreach(FieldInfo fieldInfo in relevantStaticFields) {
                classDataHolder.Add(fieldInfo.Name, new JValue(fieldInfo.GetValue(null)));
            }
            listOfClassDataToSave.Add(type.Name,classDataHolder);
        }
        JObject dataToSave = JObject.FromObject(listOfClassDataToSave);
        File.WriteAllText("C:\\Users\\"+WindowsUsername+"\\Documents\\Mount and Blade II Bannerlord\\enhancedFrameworkSaveData.json", dataToSave.ToString());
    }
    
    public static void LoadData() {
        JObject savedData = JObject.Parse(File.ReadAllText("C:\\Users\\"+WindowsUsername+"\\Documents\\Mount and Blade II Bannerlord\\enhancedFrameworkSaveData.json"));
        List<Type> relevantTypes = GetRelevantClassTypesForSyncingData();

        foreach(Type type in relevantTypes) {
            List<FieldInfo> relevantStaticFields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            JObject classDataHolder = savedData.GetValue(type.Name) as JObject;
            if (classDataHolder == null) {
                continue;
            }
            foreach(FieldInfo fieldInfo in relevantStaticFields) {
                Object staticFieldSavedValue = classDataHolder[fieldInfo.Name];
                fieldInfo.SetValue(null,staticFieldSavedValue);
            }
        }
    }

    private static List<Type> GetRelevantClassTypesForSyncingData() {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes())
            .Where(t =>
                t.IsClass &&
                t.Namespace == ApplicationInformation.GetEnhancedFrameworkNamespaceName() &&
                Assembly.GetCallingAssembly().GetTypes().Contains(t))
            .ToList();
    }
}