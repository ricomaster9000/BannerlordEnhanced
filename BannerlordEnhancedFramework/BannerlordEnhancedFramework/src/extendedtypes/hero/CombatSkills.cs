using System.Collections.Generic;

namespace BannerlordEnhancedFramework.src.extendedtypes
{
	public class CombatSkills : Skills
	{
		public CombatSkills(List<Skill> combatSkills) : base()
		{
			allSkills = combatSkills;
			OrderByDescending();
		}
	}
}
