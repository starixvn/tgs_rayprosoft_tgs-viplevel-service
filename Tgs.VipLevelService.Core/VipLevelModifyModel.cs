namespace Tgs.VipLevelService;

public record VipLevelModifyModel(
	int Level,
	decimal LevelUpAccumulateBetAmount,
	decimal LevelUpAccumulateDeposit,
	decimal LevelKeepAccumulateBetAmount,
	decimal LevelKeepAccumulateDeposit,
	bool LevelKeepCondition,
	decimal BirthBenefit,
	decimal PromotionBenefit,
	decimal QuarterlyBenefit,
	decimal LiveDealerRebate,
	decimal SlotRebate,
	decimal LotteryRebate,
	decimal CardGameRebate,
	decimal SportsRebate,
	decimal VirtualSportsRebate,
	int WithdrawTransactionsPerDay,
	decimal WithdrawTotalAmountPerDay);
