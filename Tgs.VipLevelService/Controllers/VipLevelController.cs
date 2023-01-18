using Json.Patch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.Paging;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
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
	[HttpGet("{brandId}", Name = "GetBrandAllVipLevels")]
	public IAsyncEnumerable<VipLevelViewModel> GetBrandAllVipLevelsAsync(
		[FromServices] IQueryService<AllVipLevelsQuery, VipLevelInfo> queryService,
		Guid brandId)
	{
		var query = new AllVipLevelsQuery(brandId);

		return queryService.QueryAsync(query, HttpContext.RequestAborted)
			.Select(info => new VipLevelViewModel
			{
				Id = info.Id,
				BrandId = info.BrandId,
				BirthBenefit = info.BirthBenefit,
				CardGameRebate = info.CardGameRebate,
				Level = info.Level,
				LevelKeepAccumulateBetAmount = info.LevelKeepAccumulateBetAmount,
				LevelKeepAccumulateDeposit = info.LevelKeepAccumulateDeposit,
				LevelKeepCondition = info.LevelKeepCondition,
				LevelUpAccumulateBetAmount = info.LevelUpAccumulateBetAmount,
				LevelUpAccumulateDeposit = info.LevelUpAccumulateDeposit,
				LiveDealerRebate = info.LiveDealerRebate,
				LotteryRebate = info.LotteryRebate,
				PromotionBenefit = info.PromotionBenefit,
				QuarterlyBenefit = info.QuarterlyBenefit,
				SlotRebate = info.SlotRebate,
				SportsRebate = info.SportsRebate,
				VirtualSportsRebate = info.VirtualSportsRebate
			});
	}

	/// <summary>
	/// Patches the vip level asynchronous.
	/// </summary>
	/// <param name="getModelService">The get model service.</param>
	/// <param name="patchService">The patch service.</param>
	/// <param name="jsonOptionsAccessor">The json options accessor.</param>
	/// <param name="modifier">The modifier.</param>
	/// <param name="id">The identifier.</param>
	/// <param name="patch">The patch.</param>
	[HttpPatch("{id}", Name = "PatchVipLevel")]
	public async Task PatchVipLevelAsync(
		[FromServices] IGetService<GetVipLevelBaseModifyModelQuery, VipLevelModifyModel?> getModelService,
		[FromServices] ICommandService<PatchVipLevelCommand> patchService,
		[FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
		[FromHeader(Name = "Modifier")] string modifier,
		Guid id,
		[FromBody] JsonPatch patch)
	{
		var query = new GetVipLevelBaseModifyModelQuery(id);

		var baseModel = await getModelService
			.GetAsync(query, HttpContext.RequestAborted)
			.ConfigureAwait(false);

		var modifiedModel = patch.Apply(baseModel, jsonOptionsAccessor.Value.JsonSerializerOptions);

		var patchCommand = new PatchVipLevelCommand(id, modifiedModel!, modifier);

		await patchService
			.ExecuteAsync(patchCommand, HttpContext.RequestAborted)
			.ConfigureAwait(false);
	}

	/// <summary>
	/// Creates the vip level asynchronous.
	/// </summary>
	/// <param name="createService">The create service.</param>
	/// <param name="modifier">The modifier.</param>
	/// <param name="brandId">The brand identifier.</param>
	/// <param name="createModel">The create model.</param>
	/// <returns></returns>
	[HttpPut("{brandId}", Name = "CreateVipLevel")]
	public ValueTask<Guid> CreateVipLevelAsync(
		[FromServices] IGetService<CreateVipLevelCommand, Guid> createService,
		[FromHeader(Name = "Modifier")] string modifier,
		Guid brandId,
		[FromBody] VipLevelCreateViewModel createModel)
	{
		var command = new CreateVipLevelCommand(
			brandId,
			new VipLevelCreateModel(
				createModel.LevelUpAccumulateBetAmount,
				createModel.LevelUpAccumulateDeposit,
				createModel.LevelKeepAccumulateBetAmount,
				createModel.LevelKeepAccumulateDeposit,
				createModel.LevelKeepCondition,
				createModel.BirthBenefit,
				createModel.PromotionBenefit,
				createModel.QuarterlyBenefit,
				createModel.LiveDealerRebate,
				createModel.SlotRebate,
				createModel.LotteryRebate,
				createModel.CardGameRebate,
				createModel.SportsRebate,
				createModel.VirtualSportsRebate),
			modifier);

		return createService.GetAsync(command, HttpContext.RequestAborted);
	}

	[HttpGet("playerAdjustInfo", Name = "QueryPlayerVipAdjustInfos")]
	public async Task<PagingResponseViewModel<PlayerVipAdjustViewModel>> QueryPlayerVipAdjustInfosAsync(
		[FromServices] IGetService<PlayerVipAdjustQuery, int> countService,
		[FromServices] IQueryService<PlayerVipAdjustQuery, PlayerVipAdjustInfo> queryService,
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
			source.BrandId,
			source.PlayerCode,
			createOnRange,
			sourcePaging);

		var totalCount = await countService.GetAsync(query).ConfigureAwait(false);

		var items = queryService.QueryAsync(query)
			.Select(row => new PlayerVipAdjustViewModel
			{
				OldLevel = row.OldLevel,
				NewLevel = row.NewLevel,
				CreatedOn = row.CreatedOn,
			});

		return new PagingResponseViewModel<PlayerVipAdjustViewModel>()
		{
			TotalCount = totalCount,
			Page = 1,
			Rows = -1,
			Items = items.ToEnumerable(),
		};
	}
}
