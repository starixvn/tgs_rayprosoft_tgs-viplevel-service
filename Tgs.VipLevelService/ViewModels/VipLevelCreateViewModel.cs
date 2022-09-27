namespace Tgs.VipLevelService.ViewModels;

/// <summary>
/// VipLevelCreateViewModel
/// </summary>
public class VipLevelCreateViewModel
{
	/// <summary>
	/// Gets the level up accumulate bet amount.
	/// </summary>
	public decimal LevelUpAccumulateBetAmount { get; init; }

	/// <summary>
	/// Gets the level up accumulate deposit.
	/// </summary>
	public decimal LevelUpAccumulateDeposit { get; init; }

	/// <summary>
	/// Gets the level keep accumulate bet amount.
	/// </summary>
	public decimal LevelKeepAccumulateBetAmount { get; init; }

	/// <summary>
	/// Gets the level keep accumulate deposit.
	/// </summary>
	public decimal LevelKeepAccumulateDeposit { get; init; }

	/// <summary>
	/// Gets a value indicating whether [level keep condition].
	/// </summary>
	public bool LevelKeepCondition { get; init; }

	/// <summary>
	/// Gets the birth benefit.
	/// </summary>
	public decimal BirthBenefit { get; init; }

	/// <summary>
	/// Gets the promotion benefit.
	/// </summary>
	public decimal PromotionBenefit { get; init; }

	/// <summary>
	/// Gets the quarterly benefit.
	/// </summary>
	public decimal QuarterlyBenefit { get; init; }

	/// <summary>
	/// Gets the live dealer rebate.
	/// </summary>
	public decimal LiveDealerRebate { get; init; }

	/// <summary>
	/// Gets the slot rebate.
	/// </summary>
	public decimal SlotRebate { get; init; }

	/// <summary>
	/// Gets the lottery rebate.
	/// </summary>
	public decimal LotteryRebate { get; init; }

	/// <summary>
	/// Gets the card game rebate.
	/// </summary>
	public decimal CardGameRebate { get; init; }

	/// <summary>
	/// Gets the sports rebate.
	/// </summary>
	public decimal SportsRebate { get; init; }

	/// <summary>
	/// Gets the virtual sports rebate.
	/// </summary>
	public decimal VirtualSportsRebate { get; init; }
}
