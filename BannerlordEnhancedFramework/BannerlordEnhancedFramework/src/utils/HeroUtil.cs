using System.Collections.Generic;
using BannerlordEnhancedFramework.src.extendedtypes;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace BannerlordEnhancedFramework.src.utils
{
	internal class HeroUtil
	{
		public static List<Skill> GetCombatSkills(Hero hero)
		{
			return new List<Skill>()
			{
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.OneHanded), DefaultSkills.OneHanded),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.TwoHanded), DefaultSkills.TwoHanded),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Polearm), DefaultSkills.Polearm),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Throwing), DefaultSkills.Throwing),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Crossbow), DefaultSkills.Crossbow),
			new Skill(hero.CharacterObject.GetSkillValue(DefaultSkills.Bow), DefaultSkills.Bow),
		};
		}
	}
}
