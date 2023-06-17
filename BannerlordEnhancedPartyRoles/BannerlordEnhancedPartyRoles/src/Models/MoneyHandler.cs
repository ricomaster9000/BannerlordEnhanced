using TaleWorlds.CampaignSystem;

using TaleWorlds.CampaignSystem.GameComponents;

namespace BannerlordEnhancedPartyRoles
{
    // Example Test
    class MoneyHandler : DefaultClanFinanceModel
    {
        public override ExplainedNumber CalculateClanGoldChange(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
        {
            ExplainedNumber cash = base.CalculateClanGoldChange(clan, includeDescriptions, applyWithdrawals, includeDetails);
            cash.Add(300);
            return cash;
        }
    }
}
