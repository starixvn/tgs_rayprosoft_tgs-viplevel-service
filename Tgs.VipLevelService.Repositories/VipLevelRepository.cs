using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class VipLevelRepository(
	TgsContext context) : IVipLevelRepository
{
	public async ValueTask CreateAsync(Guid id, Guid brandId, VipLevelCreate create, string modifier, CancellationToken cancellationToken = default)
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
			LevelUpAccumulateBetAmount = create.LevelUpAccumulateBetAmount,
			LevelUpAccumulateDeposit = create.LevelUpAccumulateDeposit,
			LevelKeepAccumulateBetAmount = create.LevelKeepAccumulateBetAmount,
			LevelKeepAccumulateDeposit = create.LevelKeepAccumulateDeposit,
			LevelKeepCondition = create.LevelKeepCondition,
			BirthReward = create.BirthBenefit,
			PromotionReward = create.PromotionBenefit,
			QuarterlyReward = create.QuarterlyBenefit,
			LiveDealerRebatePercentage = create.LiveDealerRebate,
			SlotRebatePercentage = create.SlotRebate,
			LotteryRebatePercentage = create.LotteryRebate,
			CardGameRebatePercentage = create.CardGameRebate,
			SportsRebatePercentage = create.SportsRebate,
			VirtualSportsRebatePercentage = create.VirtualSportsRebate,
			WithdrawTotalAmountPerDay = create.WithdrawTotalAmountPerDay,
			WithdrawTransactionsPerDay = create.WithdrawTransactionsPerDay,
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

	public async IAsyncEnumerable<VipLevelInfo> QueryAsync(Guid brandId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var vipLevelInfos = await context.VipLevels
			.Where(predicate: vl => vl.BrandId == brandId)
			.OrderBy(keySelector: vl => vl.Level)
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
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (vipLevelInfos.Length == 0)
		{
			yield break;
		}

		foreach (var vipLevelInfo in vipLevelInfos)
		{
			yield return vipLevelInfo;
		}
	}

	public async ValueTask UpdateAsync(Guid id, Guid brandId, VipLevelUpdate update, string modifier, CancellationToken cancellationToken = default)
	{
		var viplevel = await context.VipLevels
			.FindAsync(
			keyValues: new object[] { id },
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (viplevel is null)
			return;

		var currentDate = DateTime.UtcNow;

		viplevel.Level = update.Level;
		viplevel.LevelUpAccumulateBetAmount = update.LevelUpAccumulateBetAmount;
		viplevel.LevelUpAccumulateDeposit = update.LevelUpAccumulateDeposit;
		viplevel.LevelKeepAccumulateBetAmount = update.LevelKeepAccumulateBetAmount;
		viplevel.LevelKeepAccumulateDeposit = update.LevelKeepAccumulateDeposit;
		viplevel.LevelKeepCondition = update.LevelKeepCondition;
		viplevel.BirthReward = update.BirthBenefit;
		viplevel.PromotionReward = update.PromotionBenefit;
		viplevel.QuarterlyReward = update.QuarterlyBenefit;
		viplevel.LiveDealerRebatePercentage = update.LiveDealerRebate;
		viplevel.SlotRebatePercentage = update.SlotRebate;
		viplevel.LotteryRebatePercentage = update.LotteryRebate;
		viplevel.CardGameRebatePercentage = update.CardGameRebate;
		viplevel.SportsRebatePercentage = update.SportsRebate;
		viplevel.VirtualSportsRebatePercentage = update.VirtualSportsRebate;
		viplevel.WithdrawTransactionsPerDay = update.WithdrawTransactionsPerDay;
		viplevel.WithdrawTotalAmountPerDay = update.WithdrawTotalAmountPerDay;
		viplevel.UpdatedByUsername = modifier;
		viplevel.UpdatedOn = currentDate;

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
