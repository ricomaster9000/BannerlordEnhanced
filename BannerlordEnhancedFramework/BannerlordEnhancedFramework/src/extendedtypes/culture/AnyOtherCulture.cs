using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes;

public class AnyOtherCulture : ExtendedCultureCode
{
    
    public AnyOtherCulture()
    {
        this.Name = "Other";
        this.LinkedNativeCode = CultureCode.AnyOtherCulture;
    }
    
    public override bool IsItemOfCulture(ItemRosterElement item)
    {
        return item.EquipmentElement.Item.Culture == null ||
               item.EquipmentElement.Item.Culture.GetCultureCode() == nativeCultureCode() ||
               item.EquipmentElement.Item.Culture.GetCultureCode() == CultureCode.Invalid;
    }
    
    
}