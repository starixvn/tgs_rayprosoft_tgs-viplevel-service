using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;
using Tgs.VipLevelService.ViewModels;

namespace Tgs.VipLevelService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
	[HttpGet("{playerCode}/viplevel")]
	public async ValueTask<PlayerVipLevelInfoViewModel> PlayerVipLevelPlayerCodeGetAsync(
		[FromServices] IGetService<PlayerVipLevelPlayerCodeQuery, PlayerVipLevelInfo> service,
		string playerCode)
	{
		var playerVipLevelInfo = await service.GetAsync(
			query: new PlayerVipLevelPlayerCodeQuery(
				PlayerCode: playerCode),
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(true);

		return new PlayerVipLevelInfoViewModel(
			playerVipLevelInfo.PlayerCode,
			playerVipLevelInfo.VipLevelId,
			playerVipLevelInfo.VipLevel,
			playerVipLevelInfo.AutoState);
	}

	[HttpPost("{playerCode}/viplevel/register")]
	public ValueTask PlayerVipLeveRegisterAsync(
		[FromServices] ICommandService<PlayerVipLeveRegisterCommand> service,
		string playerCode,
		[FromBody] PlayerVipLeveRegisterViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLeveRegisterCommand(
				playerCode,
				source.VipLevelId,
				source.VipLevel,
				source.AutoState),
			cancellationToken: HttpContext.RequestAborted);

	[HttpPost("{playerCode}/viplevel/manual")]
	public ValueTask PlayerVipLevelManualAsync(
		[FromServices] ICommandService<PlayerVipLevelManualCommand> service,
		string playerCode,
		[FromBody] PlayerVipLevelManualViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelManualCommand(
				playerCode,
				source.VipLevelId,
				source.VipLevel,
				source.AutoState,
				source.CreatedUserId,
				source.CreatedUsername,
				source.Remark),
			cancellationToken: HttpContext.RequestAborted);

	[HttpPost("{playerCode}/viplevel/system")]
	public ValueTask PlayerVipLevelSystemAsync(
		[FromServices] ICommandService<PlayerVipLevelSystemCommand> service,
		string playerCode,
		[FromBody] PlayerVipLevelSystemViewModel source)
		=> service.ExecuteAsync(
			command: new PlayerVipLevelSystemCommand(
				playerCode,
				source.VipLevelId,
				source.VipLevel,
				source.AutoState),
			cancellationToken: HttpContext.RequestAborted);

	[HttpGet("{playerCode}/viplevel/log")]
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
				Type: log.Type,
				BeforeLevel: log.BeforeLevel,
				BeforeLevelId: log.BeforeLevelId,
				BeforeAutoState: log.BeforeAutoState,
				AfterLevel: log.AfterLevel,
				AfterLevelId: log.AfterLevelId,
				AfterAutoState: log.AfterAutoState,
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
