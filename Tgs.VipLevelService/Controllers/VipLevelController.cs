using Json.Patch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Paging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;
using Tgs.VipLevelService.ViewModels;

namespace Tgs.VipLevelService.Controllers;

/// <summary>
/// VipLevel
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
[Route("vip")]
[ApiController]
public class VipLevelController : ControllerBase
{
	/// <summary>
	/// Gets the brand all vip levels asynchronous.
	/// </summary>
	/// <param name="queryService">The query service.</param>
	/// <param name="brandId">The brand identifier.</param>
	/// <returns></returns>
	[HttpGet("{brandId}", Name = "VipLevel_Query")]
	public IAsyncEnumerable<VipLevelInfoViewModel> VipLevelQueryAsync(
		[FromServices] IQueryService<VipLevelQuery, VipLevelInfo> queryService,
		Guid brandId)
	{
		var query = new VipLevelQuery(brandId);

		return queryService.QueryAsync(query, HttpContext.RequestAborted)
			.Select(info => new VipLevelInfoViewModel(
				Id: info.Id,
				Level: info.Level,
				BrandId: info.BrandId,
				LevelUpAccumulateBetAmount: info.LevelUpAccumulateBetAmount,
				LevelUpAccumulateDeposit: info.LevelUpAccumulateDeposit,
				LevelKeepAccumulateBetAmount: info.LevelKeepAccumulateBetAmount,
				LevelKeepAccumulateDeposit: info.LevelKeepAccumulateDeposit,
				LevelKeepCondition: info.LevelKeepCondition,
				BirthBenefit: info.BirthBenefit,
				PromotionBenefit: info.PromotionBenefit,
				QuarterlyBenefit: info.QuarterlyBenefit,
				LiveDealerRebate: info.LiveDealerRebate,
				SlotRebate: info.SlotRebate,
				LotteryRebate: info.LotteryRebate,
				CardGameRebate: info.CardGameRebate,
				SportsRebate: info.SportsRebate,
				VirtualSportsRebate: info.VirtualSportsRebate,
				WithdrawTotalAmountPerDay: info.WithdrawTotalAmountPerDay,
				WithdrawTransactionsPerDay: info.WithdrawTransactionsPerDay
			));
	}

	/// <summary>
	/// Patches the vip level asynchronous.
	/// </summary>
	/// <param name="getModelService">The get model service.</param>
	/// <param name="modifyService">The modify service.</param>
	/// <param name="jsonOptionsAccessor">The json options accessor.</param>
	/// <param name="modifier">The modifier.</param>
	/// <param name="id">The identifier.</param>
	/// <param name="patch">The patch.</param>
	[HttpPatch("{id}", Name = "VipLevel_Update")]
	public async Task VipLevelUpdateAsync(
		[FromServices] IGetService<VipLevelUpdateQuery, VipLevelUpdate> getModelService,
		[FromServices] ICommandService<VipLevelUpdateCommand> modifyService,
		[FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
		[FromHeader(Name = "Modifier")] string modifier,
		Guid id,
		[FromBody] JsonPatch patch)
	{
		var query = new VipLevelUpdateQuery(id);

		var baseModel = await getModelService
			.GetAsync(query, HttpContext.RequestAborted)
			.ConfigureAwait(false);

		var modifiedModel = patch.Apply(baseModel, jsonOptionsAccessor.Value.JsonSerializerOptions);

		var patchCommand = new VipLevelUpdateCommand(id, modifiedModel!, modifier);

		await modifyService
			.ExecuteAsync(patchCommand, HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Vips the level create asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="modifier">The modifier.</param>
	/// <param name="brandId">The brand identifier.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPut("{brandId}", Name = "VipLevel_Create")]
	public ValueTask<Guid> VipLevelCreateAsync(
		[FromServices] IGetService<VipLevelCreateCommand, Guid> service,
		[FromHeader(Name = "Modifier")] string modifier,
		Guid brandId,
		[FromBody] VipLevelCreateViewModel source)
	{
		var command = new VipLevelCreateCommand(
			brandId,
			new VipLevelCreate(
				source.LevelUpAccumulateBetAmount,
				source.LevelUpAccumulateDeposit,
				source.LevelKeepAccumulateBetAmount,
				source.LevelKeepAccumulateDeposit,
				source.LevelKeepCondition,
				source.BirthBenefit,
				source.PromotionBenefit,
				source.QuarterlyBenefit,
				source.LiveDealerRebate,
				source.SlotRebate,
				source.LotteryRebate,
				source.CardGameRebate,
				source.SportsRebate,
				source.VirtualSportsRebate,
				source.WithdrawTransactionsPerDay,
				source.WithdrawTotalAmountPerDay),
			modifier);

		return service.GetAsync(command, HttpContext.RequestAborted);
	}

	[HttpGet("playerAdjustInfo", Name = "QueryPlayerVipAdjustInfos")]
	public async Task<PagingResponseViewModel<PlayerVipAdjustViewModel>> QueryPlayerVipAdjustInfosAsync(
		[FromServices] IGetService<PlayerVipAdjustQuery, int> countService,
		[FromServices] IQueryService<PlayerVipAdjustQuery, PlayerVipAdjustInfo> queryService,
		[FromServices] IQueryService<AllVipLevelsQuery, VipLevelInfo> vipQueryService,
		[FromQuery] PlayerVipAdjustQueryViewModel source,
		[FromQuery] PagingViewModel paging)
	{
		var createOnRange = ((DateTimeOffset Begin, DateTimeOffset End)?)null;

		if (source.BeginCreatedOn != null && source.EndCreatedOn != null)
			createOnRange = (source.BeginCreatedOn.Value, source.EndCreatedOn.Value);

		var sourcePaging = new Paging<PlayerVipAdjustInfo>
		{
			Page = paging.Page ?? 1,
			RowCount = paging.Rows ?? 10
		};

		if (paging.ShowAll == true)
		{
			sourcePaging.Page = -1;
			sourcePaging.RowCount = -1;
		}

		if (!string.IsNullOrWhiteSpace(paging.SortBy))
			sourcePaging.SortByPropertyName(paging.SortBy, paging.IsDesc ?? false);

		var query = new PlayerVipAdjustQuery(
			source.PlayerCode,
			createOnRange,
			sourcePaging);

		var allVips = await vipQueryService
			.QueryAsync(new AllVipLevelsQuery(source.BrandId))
			.ToDictionaryAsync(x => x.Id, x => x.Level, HttpContext.RequestAborted)
			.ConfigureAwait(false);

		var totalCount = await countService
			.GetAsync(query, HttpContext.RequestAborted)
			.ConfigureAwait(false);

		var items = await queryService
			.QueryAsync(query)
			.ToArrayAsync(HttpContext.RequestAborted)
			.ConfigureAwait(false);

		var result = items
			.Select(row => new PlayerVipAdjustViewModel
			{
				OldLevel = allVips[row.OldVipId],
				NewLevel = allVips[row.NewVipId],
				CreatedOn = row.CreatedOn,
			});

		return new PagingResponseViewModel<PlayerVipAdjustViewModel>()
		{
			TotalCount = totalCount,
			Page = 1,
			Rows = -1,
			Items = result,
		};
	}
}
