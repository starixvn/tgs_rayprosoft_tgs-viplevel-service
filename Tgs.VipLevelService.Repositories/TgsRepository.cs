using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.Repositories;

internal class TgsRepository : ITgsRepository
{
	private readonly TgsContext _tgsContext;

	public TgsRepository(TgsContext tgsContext)
	{
		_tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
	}

	public IAsyncEnumerable<VipLevelInfo> QueryAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default)
		=> _tgsContext.VipLevels
			.Where(vl => vl.BrandId == brandId)
			.Select(vl => new VipLevelInfo(
				vl.Id,
				vl.Level,
				vl.BrandId,
				vl.LevelUpAccumulateBetAmount,
				vl.LevelUpAccumulateDeposit,
				vl.LevelKeepAccumulateBetAmount,
				vl.LevelKeepAccumulateDeposit,
				vl.LevelKeepCondition,
				vl.BirthReward,
				vl.PromotionReward,
				vl.QuarterlyReward,
				vl.LiveDealerRebatePercentage,
				vl.SlotRebatePercentage,
				vl.LotteryRebatePercentage,
				vl.CardGameRebatePercentage,
				vl.SlotRebatePercentage,
				vl.VirtualSportsRebatePercentage))
			.ToAsyncEnumerable();

	public async ValueTask<VipLevelModifyModel?> GetVipLevelBaseModifyModelAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _tgsContext.VipLevels
			.FindAsync(new object[] { id }, cancellationToken: cancellationToken)
			.AsTask()
			.ToAsyncEnumerable()
			.Where(vl => vl is not null)
			.Select(vl => new VipLevelModifyModel(
				vl!.Level,
				vl.LevelUpAccumulateBetAmount,
				vl.LevelUpAccumulateDeposit,
				vl.LevelKeepAccumulateBetAmount,
				vl.LevelKeepAccumulateDeposit,
				vl.LevelKeepCondition,
				vl.BirthReward,
				vl.PromotionReward,
				vl.QuarterlyReward,
				vl.LiveDealerRebatePercentage,
				vl.SlotRebatePercentage,
				vl.LotteryRebatePercentage,
				vl.CardGameRebatePercentage,
				vl.SlotRebatePercentage,
				vl.VirtualSportsRebatePercentage))
			.FirstOrDefaultAsync(cancellationToken)
			.ConfigureAwait(false);

	public async ValueTask ModifyVipLevelAsync(Guid id, VipLevelModifyModel modifyModel, string modifier, CancellationToken cancellationToken = default)
	{
		var viplevel = await _tgsContext.VipLevels
			.FindAsync(new object[] { id }, cancellationToken)
			.ConfigureAwait(false);

		if (viplevel is null)
			return;

		var currentDate = DateTime.UtcNow;

		viplevel.Level = modifyModel.Level;
		viplevel.LevelUpAccumulateBetAmount = modifyModel.LevelUpAccumulateBetAmount;
		viplevel.LevelUpAccumulateDeposit = modifyModel.LevelUpAccumulateDeposit;
		viplevel.LevelKeepAccumulateBetAmount = modifyModel.LevelKeepAccumulateBetAmount;
		viplevel.LevelKeepAccumulateDeposit = modifyModel.LevelKeepAccumulateDeposit;
		viplevel.LevelKeepCondition = modifyModel.LevelKeepCondition;
		viplevel.BirthReward = modifyModel.BirthBenefit;
		viplevel.PromotionReward = modifyModel.PromotionBenefit;
		viplevel.QuarterlyReward = modifyModel.QuarterlyBenefit;
		viplevel.LiveDealerRebatePercentage = modifyModel.LiveDealerRebate;
		viplevel.SlotRebatePercentage = modifyModel.SportsRebate;
		viplevel.LotteryRebatePercentage = modifyModel.LotteryRebate;
		viplevel.CardGameRebatePercentage = modifyModel.CardGameRebate;
		viplevel.SportsRebatePercentage = modifyModel.SportsRebate;
		viplevel.VirtualSportsRebatePercentage = modifyModel.VirtualSportsRebate;
		viplevel.UpdatedByUsername = modifier;
		viplevel.UpdatedOn = currentDate;

		_ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask CreateVipLevelAsync(Guid id, Guid brandId, VipLevelModifyModel createModel, string modifier, CancellationToken cancellationToken = default)
	{
		var currentDate = DateTime.UtcNow;

		var newVipLevel = new VipLevel
		{
			Id = id,
			BrandId = brandId,
			Level = createModel.Level,
			LevelUpAccumulateBetAmount = createModel.LevelUpAccumulateBetAmount,
			LevelUpAccumulateDeposit = createModel.LevelUpAccumulateDeposit,
			LevelKeepAccumulateBetAmount = createModel.LevelKeepAccumulateBetAmount,
			LevelKeepAccumulateDeposit = createModel.LevelKeepAccumulateDeposit,
			LevelKeepCondition = createModel.LevelKeepCondition,
			BirthReward = createModel.BirthBenefit,
			PromotionReward = createModel.PromotionBenefit,
			QuarterlyReward = createModel.QuarterlyBenefit,
			LiveDealerRebatePercentage = createModel.LiveDealerRebate,
			SlotRebatePercentage = createModel.SportsRebate,
			LotteryRebatePercentage = createModel.LotteryRebate,
			CardGameRebatePercentage = createModel.CardGameRebate,
			SportsRebatePercentage = createModel.SportsRebate,
			VirtualSportsRebatePercentage = createModel.VirtualSportsRebate,
			CreatedByUsername = modifier,
			UpdatedByUsername = modifier,
			CreatedOn = currentDate,
			UpdatedOn = currentDate
		};

		_ = await _tgsContext.VipLevels
			.AddAsync(newVipLevel, cancellationToken)
			.ConfigureAwait(false);

		_ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}
}
