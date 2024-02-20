using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;
using Tgs.VipLevelService.ViewModels;

namespace Tgs.VipLevelService.Controllers;

[Route("vip-snap")]
[ApiController]
public class VipLevelSnapshotController : ControllerBase
{
	/// <summary>
	/// Vips the level snapshot create asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///			{
	///				"brandId": "4e86719f-243f-46f6-b010-5c758b7285af",
	///				"snapshotUserId": "00000000-0000-0000-0000-000000000000",
	///				"snapshotUsername": "admin.enabled"
	///			}
	/// </remarks>
	[HttpPost(Name = "VipLevelSnapshot_Create")]
	public ValueTask VipLevelSnapshotCreateAsync(
		[FromServices] ICommandService<VipLevelSnapshotCreateCommand> service,
		VipLevelSnapshotCreateViewModel source)
		=> service.ExecuteAsync(
		command: new VipLevelSnapshotCreateCommand(
			BrandId: source.BrandId,
			EffectiveOn: source.EffectiveOn,
			SnapshotUserId: source.SnapshotUserId,
			SnapshotUsername: source.SnapshotUsername),
		cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Vips the level snapshot query vip level asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="id">The identifier.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Get
	///			"id": "018d6398-b786-0000-0001-07e869564c64"
	/// </remarks>
	[HttpGet("{id:guid}", Name = "VipLevelSnapshot_Query_VipLevel")]
	public IAsyncEnumerable<VipLevelInfoViewModel> VipLevelSnapshotQueryVipLevelAsync(
		[FromServices] IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo> service,
		Guid id)
		=> service.QueryAsync(
		query: new VipLevelSnapshotQueryVipLevelQuery(
			Id: id),
		cancellationToken: HttpContext.RequestAborted)
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

	/// <summary>
	/// Vips the level snapshot current snapshot identifier asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="brandId">The brand identifier.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Get
	///			"brandId": "4e86719f-243f-46f6-b010-5c758b7285af"
	/// </remarks>
	[HttpGet("{brandId:guid}/current/snapshot-id", Name = "VipLevelSnapshot_Current_SnapshotId")]
	public ValueTask<Guid> VipLevelSnapshotCurrentSnapshotIdAsync(
		[FromServices] IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid> service,
		Guid brandId)
		=> service.GetAsync(
		new VipLevelSnapshotCurrentSnapshotIdQuery(
			BrandId: brandId),
		cancellationToken: HttpContext.RequestAborted);
}
