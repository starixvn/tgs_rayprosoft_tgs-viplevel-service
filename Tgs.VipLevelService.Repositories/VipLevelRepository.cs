using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.Repositories;

internal class VipLevelRepository(
	TgsContext context) : IVipLevelRepository
{
	public async ValueTask CreateAsync(Guid id, Guid brandId, VipLevelCreateModel createModel, string modifier, CancellationToken cancellationToken = default)
	{
		var currentDate = DateTime.UtcNow;

		var maxVipLevel = await context.VipLevels
			.Where(vip => vip.BrandId == brandId)
			.MaxAsync(vip => (int?)vip.Level, cancellationToken)
			.ConfigureAwait(false) ?? 0;

		var newVipLevel = new VipLevel
		{
			Id = id,
			BrandId = brandId,
			Level = maxVipLevel + 1,
			LevelUpAccumulateBetAmount = createModel.LevelUpAccumulateBetAmount,
			LevelUpAccumulateDeposit = createModel.LevelUpAccumulateDeposit,
			LevelKeepAccumulateBetAmount = createModel.LevelKeepAccumulateBetAmount,
			LevelKeepAccumulateDeposit = createModel.LevelKeepAccumulateDeposit,
			LevelKeepCondition = createModel.LevelKeepCondition,
			BirthReward = createModel.BirthBenefit,
			PromotionReward = createModel.PromotionBenefit,
			QuarterlyReward = createModel.QuarterlyBenefit,
			LiveDealerRebatePercentage = createModel.LiveDealerRebate,
			SlotRebatePercentage = createModel.SlotRebate,
			LotteryRebatePercentage = createModel.LotteryRebate,
			CardGameRebatePercentage = createModel.CardGameRebate,
			SportsRebatePercentage = createModel.SportsRebate,
			VirtualSportsRebatePercentage = createModel.VirtualSportsRebate,
			WithdrawTotalAmountPerDay = createModel.WithdrawTotalAmountPerDay,
			WithdrawTransactionsPerDay = createModel.WithdrawTransactionsPerDay,
			CreatedByUsername = modifier,
			UpdatedByUsername = modifier,
			CreatedOn = currentDate,
			UpdatedOn = currentDate
		};

		_ = await context.VipLevels
			.AddAsync(
			entity: newVipLevel,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public ValueTask<VipLevelInfo?> GetAsync(Guid brandId, Guid id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

	public ValueTask<VipLevelInfo?> GetAsync(Guid brandId, int level, CancellationToken cancellationToken = default) => throw new NotImplementedException();

	public async ValueTask<VipLevelInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await context.VipLevels
		.FindAsync(new object[] { id }, cancellationToken: cancellationToken)
		.AsTask()
		.ToAsyncEnumerable()
		.Where(vl => vl is not null)
		.Select(vl => new VipLevelInfo(
			vl!.Id,
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
			vl.SportsRebatePercentage,
			vl.VirtualSportsRebatePercentage,
			vl.WithdrawTransactionsPerDay,
			vl.WithdrawTotalAmountPerDay))
		.FirstOrDefaultAsync(cancellationToken)
		.ConfigureAwait(false);

	public IAsyncEnumerable<VipLevelInfo> QueryAsync(Guid brandId, CancellationToken cancellationToken = default)
		=> context.VipLevels
		.Where(predicate: vl => vl.BrandId == brandId)
		.Select(selector: vl => new VipLevelInfo(
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
			vl.SportsRebatePercentage,
			vl.VirtualSportsRebatePercentage,
			vl.WithdrawTransactionsPerDay,
			vl.WithdrawTotalAmountPerDay))
		.ToAsyncEnumerable();

	public async ValueTask UpdateAsync(Guid id, Guid brandId, VipLevelModifyModel modifyModel, string modifier, CancellationToken cancellationToken = default)
	{
		var viplevel = await context.VipLevels
			.FindAsync(
			keyValues: new object[] { id },
			cancellationToken: cancellationToken)
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
		viplevel.SlotRebatePercentage = modifyModel.SlotRebate;
		viplevel.LotteryRebatePercentage = modifyModel.LotteryRebate;
		viplevel.CardGameRebatePercentage = modifyModel.CardGameRebate;
		viplevel.SportsRebatePercentage = modifyModel.SportsRebate;
		viplevel.VirtualSportsRebatePercentage = modifyModel.VirtualSportsRebate;
		viplevel.WithdrawTransactionsPerDay = modifyModel.WithdrawTransactionsPerDay;
		viplevel.WithdrawTotalAmountPerDay = modifyModel.WithdrawTotalAmountPerDay;
		viplevel.UpdatedByUsername = modifier;
		viplevel.UpdatedOn = currentDate;

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
