using System.Collections.Generic;
using BannerlordEnhancedFramework.extendedtypes.itemcategories;
using BannerlordEnhancedFramework.src.extendedtypes;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;

namespace BannerlordEnhancedFramework.extendedtypes;

public class CavalryRiderClass : BaseHeroClass
{
    public CavalryRiderClass(Hero hero, HeroEquipmentCustomization heroEquipmentCustomization) : base(hero, heroEquipmentCustomization)
    {
        this.mainItemCategories = new List<ExtendedItemCategory>() {
            ExtendedItemCategories.MountItemCategory,
        };

        this.combatSkills = new CombatSkills(HeroUtil.GetCombatSkills(hero));
    }
    public override bool isClass(Hero hero)
    {
        return true;
    }
}