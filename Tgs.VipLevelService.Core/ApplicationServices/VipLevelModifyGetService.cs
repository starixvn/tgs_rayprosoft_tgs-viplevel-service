using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;
internal class VipLevelModifyGetService(IVipLevelRepository repository) : IGetService<VipLevelModifyQuery, VipLevelModifyModel>
{
	public async ValueTask<VipLevelModifyModel> GetAsync(VipLevelModifyQuery query, CancellationToken cancellationToken = default)
	{
		var vipLevelInfo = await repository.GetAsync(
			id: query.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException();

		return new VipLevelModifyModel(
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
