namespace Tgs.VipLevelService.Common.ViewModels;
public record VipLevelUpdateViewModel(
	int? Level,
	decimal? LevelUpAccumulateBetAmount,
	decimal? LevelUpAccumulateDeposit,
	decimal? LevelKeepAccumulateBetAmount,
	decimal? LevelKeepAccumulateDeposit,
	bool? LevelKeepCondition,
	decimal? BirthBenefit,
	decimal? PromotionBenefit,
	decimal? QuarterlyBenefit,
	decimal? LiveDealerRebate,
	decimal? SlotRebate,
	decimal? LotteryRebate,
	decimal? CardGameRebate,
	decimal? SportsRebate,
	decimal? VirtualSportsRebate,
	int? WithdrawTransactionsPerDay,
	decimal? WithdrawTotalAmountPerDay)
{
	public static readonly VipLevelUpdateViewModel Default = new(
		Level: null,
		LevelUpAccumulateBetAmount: null,
		LevelUpAccumulateDeposit: null,
		LevelKeepAccumulateBetAmount: null,
		LevelKeepAccumulateDeposit: null,
		LevelKeepCondition: null,
		BirthBenefit: null,
		PromotionBenefit: null,
		QuarterlyBenefit: null,
		LiveDealerRebate: null,
		SlotRebate: null,
		LotteryRebate: null,
		CardGameRebate: null,
		SportsRebate: null,
		VirtualSportsRebate: null,
		WithdrawTransactionsPerDay: null,
		WithdrawTotalAmountPerDay: null);
}
