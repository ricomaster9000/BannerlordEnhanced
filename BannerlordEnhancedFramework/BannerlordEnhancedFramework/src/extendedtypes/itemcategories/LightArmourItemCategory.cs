using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.extendedtypes.itemcategories;

public class LightArmourItemCategory : ArmourItemCategory
{
    public override string Name
    {
        get { return "Light Armour"; }
    }
    public override bool isType(ItemRosterElement itemRosterElement)
    {
        return base.isType(itemRosterElement) && itemRosterElement.EquipmentElement.Item.Weight < 10; // TODO change number
    }
}