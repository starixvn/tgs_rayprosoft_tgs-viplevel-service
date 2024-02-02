using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelUpdateGetService(IVipLevelRepository repository) : IGetService<VipLevelUpdateQuery, VipLevelUpdate>
{
	public async ValueTask<VipLevelUpdate> GetAsync(VipLevelUpdateQuery query, CancellationToken cancellationToken = default)
	{
		var vipLevelInfo = await repository.GetAsync(
			id: query.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();

		return new VipLevelUpdate(
			Level: vipLevelInfo.Level,
			LevelUpAccumulateBetAmount: vipLevelInfo.LevelUpAccumulateBetAmount,
			LevelUpAccumulateDeposit: vipLevelInfo.LevelUpAccumulateDeposit,
			LevelKeepAccumulateBetAmount: vipLevelInfo.LevelKeepAccumulateBetAmount,
			LevelKeepAccumulateDeposit: vipLevelInfo.LevelKeepAccumulateDeposit,
			LevelKeepCondition: vipLevelInfo.LevelKeepCondition,
			BirthBenefit: vipLevelInfo.BirthBenefit,
			PromotionBenefit: vipLevelInfo.PromotionBenefit,
			QuarterlyBenefit: vipLevelInfo.QuarterlyBenefit,
			LiveDealerRebate: vipLevelInfo.LiveDealerRebate,
			SlotRebate: vipLevelInfo.SlotRebate,
			LotteryRebate: vipLevelInfo.LotteryRebate,
			CardGameRebate: vipLevelInfo.CardGameRebate,
			SportsRebate: vipLevelInfo.SportsRebate,
			VirtualSportsRebate: vipLevelInfo.VirtualSportsRebate,
			WithdrawTransactionsPerDay: vipLevelInfo.WithdrawTransactionsPerDay,
			WithdrawTotalAmountPerDay: vipLevelInfo.WithdrawTotalAmountPerDay);
	}
}
