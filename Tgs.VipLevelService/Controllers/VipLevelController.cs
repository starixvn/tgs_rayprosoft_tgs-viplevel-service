using Json.Patch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

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
	[HttpGet("{brandId:guid}", Name = "VipLevel_Query")]
	public IAsyncEnumerable<VipLevelInfoViewModel> VipLevelQueryAsync(
		[FromServices] IQueryService<VipLevelQuery, VipLevelInfo> queryService,
		Guid brandId)
	{
		var query = new VipLevelQuery(brandId);

		return queryService.QueryAsync(query, HttpContext.RequestAborted)
			.Select(vipLevelInfo => new VipLevelInfoViewModel(
				Id: vipLevelInfo.Id,
				Level: vipLevelInfo.Level,
				BrandId: vipLevelInfo.BrandId,
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
				WithdrawTotalAmountPerDay: vipLevelInfo.WithdrawTotalAmountPerDay,
				WithdrawTransactionsPerDay: vipLevelInfo.WithdrawTransactionsPerDay
			));
	}

	/// <summary>
	/// Vips the level by level query asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="brandId">The brand identifier.</param>
	/// <param name="level">The level.</param>
	/// <returns></returns>
	[HttpGet("{brandId:guid}/{level:int}", Name = "VipLevel_ByLevel_Query")]
	public async ValueTask<VipLevelInfoViewModel?> VipLevelByLevelQueryAsync(
		[FromServices] IGetService<VipLevelByLevelQuery, VipLevelInfo?> service,
		Guid brandId,
		int level)
	{
		var query = new VipLevelByLevelQuery(brandId, level);

		var vipLevelInfo = await service.GetAsync(
			query,
			HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return vipLevelInfo is null
			? null
			: new VipLevelInfoViewModel(
				Id: vipLevelInfo.Id,
				Level: vipLevelInfo.Level,
				BrandId: vipLevelInfo.BrandId,
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
				WithdrawTotalAmountPerDay: vipLevelInfo.WithdrawTotalAmountPerDay,
				WithdrawTransactionsPerDay: vipLevelInfo.WithdrawTransactionsPerDay);
	}

	/// <summary>
	/// Vips the level by identifier query asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="id">The identifier.</param>
	/// <returns></returns>
	[HttpGet("{id:guid}/levl-id", Name = "VipLevel_ById_Query")]
	public async ValueTask<VipLevelInfoViewModel?> VipLevelByIdQueryAsync(
		[FromServices] IGetService<VipLevelByIdQuery, VipLevelInfo?> service,
		Guid id)
	{
		var query = new VipLevelByIdQuery(id);

		var vipLevelInfo = await service.GetAsync(
			query,
			HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return vipLevelInfo is null
			? null
			: new VipLevelInfoViewModel(
				Id: vipLevelInfo.Id,
				Level: vipLevelInfo.Level,
				BrandId: vipLevelInfo.BrandId,
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
				WithdrawTotalAmountPerDay: vipLevelInfo.WithdrawTotalAmountPerDay,
				WithdrawTransactionsPerDay: vipLevelInfo.WithdrawTransactionsPerDay);
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
	[HttpPatch("{id:guid}", Name = "VipLevel_Update")]
	public async Task VipLevelUpdateAsync(
		[FromServices] IGetService<VipLevelUpdateQuery, VipLevelUpdate> getModelService,
		[FromServices] ICommandService<VipLevelUpdateCommand> modifyService,
		[FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
		[FromHeader(Name = "Modifier")] string modifier,
		Guid id,
		[FromBody] JsonPatch patch)
	{
		var query = new VipLevelUpdateQuery(id);

		var vipLevelUpdate = await getModelService
			.GetAsync(query, HttpContext.RequestAborted)
			.ConfigureAwait(false);

		var modifiedModel = patch.Apply(vipLevelUpdate, jsonOptionsAccessor.Value.JsonSerializerOptions);

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
	[HttpPut("{brandId:guid}", Name = "VipLevel_Create")]
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
}
