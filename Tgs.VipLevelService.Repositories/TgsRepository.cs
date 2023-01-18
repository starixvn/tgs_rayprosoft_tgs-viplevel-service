using Microsoft.EntityFrameworkCore;
using Tgs.Paging;
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
				vl.SportsRebatePercentage,
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
				vl.SportsRebatePercentage,
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
		viplevel.SlotRebatePercentage = modifyModel.SlotRebate;
		viplevel.LotteryRebatePercentage = modifyModel.LotteryRebate;
		viplevel.CardGameRebatePercentage = modifyModel.CardGameRebate;
		viplevel.SportsRebatePercentage = modifyModel.SportsRebate;
		viplevel.VirtualSportsRebatePercentage = modifyModel.VirtualSportsRebate;
		viplevel.UpdatedByUsername = modifier;
		viplevel.UpdatedOn = currentDate;

		_ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask CreateVipLevelAsync(Guid id, Guid brandId, VipLevelCreateModel createModel, string modifier, CancellationToken cancellationToken = default)
	{
		var currentDate = DateTime.UtcNow;

		var maxVipLevel = await _tgsContext.VipLevels
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

	public IAsyncEnumerable<PlayerVipAdjustInfo> QueryPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		Paging<PlayerVipAdjustInfo> paging,
		CancellationToken cancellationToken = default)
	{
		var dataQuery = MakePlayerVipAdjustQuery(conditions)
			.Select(row => new PlayerVipAdjustInfo(row.CreatedOn, row.OldLevel, row.NewLevel))
			.ToAsyncEnumerable();

		dataQuery = dataQuery.ApplyOrderBy(paging);

		if (paging.RowCount > 0)
			dataQuery = dataQuery.ApplyPaging(paging);

		return dataQuery;
	}

	public async ValueTask<int> CountPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		CancellationToken cancellationToken = default)
	{
		var dataQuery = MakePlayerVipAdjustQuery(conditions);

		return await dataQuery
			.CountAsync(cancellationToken)
			.ConfigureAwait(false);
	}

	private IQueryable<PlayerVipAdjustInfo> MakePlayerVipAdjustQuery(PlayerVipAdjustQueryConditions conditions)
	{
		var currentDate = DateTimeOffset.UtcNow;
		var limitDate = currentDate.AddDays(-90);

		var dataQuery =
			from pr in _tgsContext.PlayerVipLevelAdjustRecords.AsQueryable()
			join vl in _tgsContext.VipLevels.AsQueryable() on pr.OldVipId equals vl.Id into vlj
			from v1 in vlj.DefaultIfEmpty()
			join vl2 in _tgsContext.VipLevels.AsQueryable() on pr.NewVipId equals vl2.Id into vlj2
			from v2 in vlj2.DefaultIfEmpty()
			where pr.PlayerCode == conditions.PlayerCode
				&& pr.CreatedOn >= limitDate
				&& pr.CreatedOn < currentDate
				&& v1.BrandId == conditions.BrandId
				&& v2.BrandId == conditions.BrandId
			select new { pr.CreatedOn, OldLevel = v1.Level, NewLevel = v2.Level };

		if (conditions.CreateOnRange != null)
			dataQuery = dataQuery.Where(x
				=> x.CreatedOn >= conditions.CreateOnRange.Value.Begin
				&& x.CreatedOn < conditions.CreateOnRange.Value.End);

		return dataQuery.Select(row
			=> new PlayerVipAdjustInfo(
				row.CreatedOn,
				row.OldLevel,
				row.NewLevel));
	}
}
