using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class ExtendedCultureCode
{

    public static readonly Dictionary<string, ExtendedCultureCode> byName = new Dictionary<string, ExtendedCultureCode>()
        {
            { CultureCode.Invalid.ToString(), new ExtendedCultureCode(CultureCode.Invalid) },
            { CultureCode.Empire.ToString(), new ExtendedCultureCode(CultureCode.Empire) },
            { CultureCode.Sturgia.ToString(), new ExtendedCultureCode(CultureCode.Sturgia) },
            { CultureCode.Aserai.ToString(), new ExtendedCultureCode(CultureCode.Aserai) },
            { CultureCode.Vlandia.ToString(), new ExtendedCultureCode(CultureCode.Vlandia) },
            { CultureCode.Khuzait.ToString(), new ExtendedCultureCode(CultureCode.Khuzait) },
            { CultureCode.Battania.ToString(), new ExtendedCultureCode(CultureCode.Battania) },
            { CultureCode.Nord.ToString(), new ExtendedCultureCode(CultureCode.Nord) },
            { CultureCode.Darshi.ToString(), new ExtendedCultureCode(CultureCode.Darshi) },
            { CultureCode.Vakken.ToString(), new ExtendedCultureCode(CultureCode.Vakken) },
            { CultureCode.AnyOtherCulture.ToString(), new ExtendedCultureCode(CultureCode.AnyOtherCulture) }
        };

    public static List<ExtendedCultureCode> values()
    {
        return byName.Values.ToList();
    }

    public static ExtendedCultureCode get(string name)
    {
        return byName[name];
    }
    
    public static ExtendedCultureCode get(CultureCode name)
    {
        return byName[name.ToString()];
    }


    private string Name;
    private CultureCode LinkedNativeCode;

    public ExtendedCultureCode(string name)
    {
        this.Name = name;
    }
    
    public ExtendedCultureCode(CultureCode LinkedNativeCode)
    {
        this.Name = LinkedNativeCode.ToString();
        this.LinkedNativeCode = LinkedNativeCode;
    }

    public string getName()
    {
        return this.Name;
    }

    public CultureCode nativeCultureCode()
    {
        return this.LinkedNativeCode;
    }

}