using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordEnhancedFramework.src.utils;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

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
