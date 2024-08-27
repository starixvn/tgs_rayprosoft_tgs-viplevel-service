using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Controllers;

[Route("[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
	[HttpGet("{playerCode}/viplevel", Name = "PlayerVipLevel_Get")]
	public async ValueTask<PlayerVipLevelInfoViewModel> PlayerVipLevelGetAsync(
		[FromServices] IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult> service,
		string playerCode)
	{
		var playerVipLevelInfo = await service.GetAsync(
			query: new PlayerVipLevelSingleQuery(
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(true);

		return new PlayerVipLevelInfoViewModel(
			ResellerId: playerVipLevelInfo.ResellerId,
			ResellerCode: playerVipLevelInfo.ResellerCode,
			LicenseeId: playerVipLevelInfo.LicenseeId,
			LicenseeCode: playerVipLevelInfo.LicenseeCode,
			BrandId: playerVipLevelInfo.BrandId,
			BrandCode: playerVipLevelInfo.BrandCode,
			PlayerCode: playerVipLevelInfo.PlayerCode,
			Username: playerVipLevelInfo.Username,
			VipLevelId: playerVipLevelInfo.VipLevelId,
			VipLevel: playerVipLevelInfo.VipLevel,
			AutoState: (Common.PlayerVipLevelAutoState)playerVipLevelInfo.AutoState,
			AccumDeposit: playerVipLevelInfo.AccumDeposit,
			AccumTurnover: playerVipLevelInfo.AccumTurnover,
			LevelUpAccumulateBetAmount: playerVipLevelInfo.LevelUpAccumulateBetAmount,
			LevelUpAccumulateDeposit: playerVipLevelInfo.LevelUpAccumulateDeposit,
			FullLevel: playerVipLevelInfo.FullLevel,
			NextVipLevelId: playerVipLevelInfo.NextVipLevelId,
			NextVipLevel: playerVipLevelInfo.NextVipLevel);
	}

	[HttpGet("{playerCode}/viplevelinfo", Name = "PlayerVipLevelInfo_Get") ]
	public async ValueTask<int> PlayerVipLevelInfoGetAsync(
		[FromServices] IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo> service,
		string playerCode)
	{
		var playerVipLevelInfo = await service.GetAsync(
			query: new PlayerVipLevelFixesQuery(
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(true);

		return playerVipLevelInfo.VipLevel;
	}

	[HttpPost("{playerCode}/viplevel/register", Name = "PlayerVipLevel_Register")]
	public ValueTask PlayerVipLevelRegisterAsync(
		[FromServices] ICommandService<PlayerVipLevelRegisterCommand> service,
		string playerCode,
		[FromBody] PlayerVipLevelRegisterViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelRegisterCommand(
				playerCode,
				source.VipLevelId,
				source.VipLevel,
				(Core.Protos.PlayerVipLevelAutoState)source.AutoState),
			cancellationToken: HttpContext.RequestAborted);

	[HttpPost("{playerCode}/viplevel/manual", Name = "PlayerVipLevel_Manual")]
	public ValueTask PlayerVipLevelManualAsync(
		[FromServices] ICommandService<PlayerVipLevelManualCommand> service,
		[FromHeader(Name = "ModifierUserId")] Guid modifierUserId,
		[FromHeader(Name = "ModifierUsername")] string modifierUsername,
		string playerCode,
		[FromBody] PlayerVipLevelManualViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelManualCommand(
				PlayerCode: playerCode,
				VipLevelId: source.VipLevelId,
				VipLevel: source.VipLevel,
				AutoState: (Core.Protos.PlayerVipLevelAutoState)source.AutoState,
				CreatedUserId: modifierUserId,
				CreatedUsername: modifierUsername,
				Remark: source.Remark),
			cancellationToken: HttpContext.RequestAborted);

	[HttpPost("{playerCode}/viplevel/system", Name = "PlayerVipLevel_System")]
	public ValueTask PlayerVipLevelSystemAsync(
		[FromServices] ICommandService<PlayerVipLevelSystemCommand> service,
		string playerCode,
		[FromBody] PlayerVipLevelSystemViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelSystemCommand(
				playerCode,
				source.VipLevelId,
				source.VipLevel),
			cancellationToken: HttpContext.RequestAborted);

	[HttpGet("{playerCode}/viplevel/log", Name = "PlayerVipLevelLog_Query")]
	public async ValueTask<PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>> PlayerVipLevelLogQueryAsync(
		[FromServices] IQueryService<PlayerVipLevelLogQuery, PlayerVipLevelLogResult> queryService,
		[FromServices] IGetService<PlayerVipLevelLogCountQuery, int> countService,
		string playerCode,
		[FromQuery] PlayerVipLevelLogViewModel source)
	{
		var playerVipLevelLogQuery = new PlayerVipLevelLogQuery(
			PlayerCode: playerCode,
			StartOn: source.StartOn,
			EndOn: source.EndOn,
			Page: source.Page,
			RowCount: source.RowCount);
		var playerVipLevelLogResults = await queryService.QueryAsync(
			query: playerVipLevelLogQuery,
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync(
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		if (playerVipLevelLogResults.Length == 0 && source.Page == 1)
		{
			return new PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>
			{
				Items = Enumerable.Empty<PlayerVipLevelLogInfoViewModel>(),
				Page = source.Page,
				Rows = source.RowCount,
				TotalCount = 0
			};
		}

		var playerVipLevelLogCountQuery = new PlayerVipLevelLogCountQuery(
			PlayerCode: playerCode,
			StartOn: source.StartOn,
			EndOn: source.EndOn);
		var count = await countService.GetAsync(
			query: playerVipLevelLogCountQuery,
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return new PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>
		{
			Items = playerVipLevelLogResults.Select(selector: log => new PlayerVipLevelLogInfoViewModel(
				Type: (Common.PlayerVipLevelLogType)log.Type,
				BeforeLevel: log.BeforeLevel,
				BeforeLevelId: log.BeforeLevelId,
				BeforeAutoState: (Common.PlayerVipLevelAutoState)log.BeforeAutoState,
				AfterLevel: log.AfterLevel,
				AfterLevelId: log.AfterLevelId,
				AfterAutoState: (Common.PlayerVipLevelAutoState)log.AfterAutoState,
				CreatedOn: log.CreatedOn,
				CreatedUserId: log.CreatedUserId,
				CreatedUsername: log.CreatedUsername,
				Remark: log.Remark)),
			Page = source.Page,
			Rows = source.RowCount,
			TotalCount = count
		};
	}
}
