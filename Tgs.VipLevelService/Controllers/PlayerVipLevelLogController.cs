using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;
using Tgs.VipLevelService.ViewModels;

namespace Tgs.VipLevelService.Controllers;

[Route("player-log")]
[ApiController]
public class PlayerVipLevelLogController : ControllerBase
{
	[HttpGet("{playerCode}", Name = "PlayerVipLevel_Log_Query")]
	public async IAsyncEnumerable<PlayerVipLevelLogQueryViewModel> QueryAsync(
		[FromServices] IPlayerVipLevelLogRepository repository,
		string playerCode,
		[FromQuery] PlayerVipLevelLogFromQuery source)
	{
		var infos = await repository.QueryAsync(
			playerCode: playerCode,
			startOn: source.StartOn,
			endOn: source.EndOn,
			offset: source.Page - 1,
			limit: source.RowCount,
			cancellationToken: HttpContext.RequestAborted)
			.ToArrayAsync()
			.ConfigureAwait(false)
			?? Enumerable.Empty<PlayerVipLevelLogInfo>();

		if (!infos.Any())
		{
			yield break;
		}

		foreach (var info in infos)
		{
			var before = JsonSerializer.Deserialize<PlayerVipLevelInfo>(info.Before);
			var after = JsonSerializer.Deserialize<PlayerVipLevelInfo>(info.After);

			int beforeLevel = 0;
			Guid beforeLevelId = Guid.Empty;
			var beforeAutoState = Core.Protos.PlayerVipLevelAutoState.Enable;
			if (before is not null)
			{
				beforeLevel = before.VipLevel;
				beforeLevelId = before.VipLevelId;
				beforeAutoState = before.AutoState;
			}

			var afterLevel = 0;
			var afterLevelId = Guid.Empty;
			var afterAutoState = Core.Protos.PlayerVipLevelAutoState.Enable;
			if (after is not null)
			{
				afterLevel = after.VipLevel;
				afterLevelId = after.VipLevelId;
				afterAutoState = after.AutoState;
			}

			yield return new PlayerVipLevelLogQueryViewModel(
				Type: (Common.PlayerVipLevelLogType)info.Type,
				BeforeLevel: beforeLevel,
				BeforeLevelId: beforeLevelId,
				BeforeAutoState: (Common.PlayerVipLevelAutoState)beforeAutoState,
				AfterLevel: afterLevel,
				AfterLevelId: afterLevelId,
				AfterAutoState: (Common.PlayerVipLevelAutoState)afterAutoState,
				CreatedOn: info.CreatedOn,
				CreatedUserId: info.CreatedUserId,
				CreatedUsername: info.CreatedUsername,
				Remark: info.Remark);
		}
	}

	[HttpGet("{playerCode}/count", Name = "PlayerVipLevel_Log_Query_Count")]
	public ValueTask<int> QueryCountAsync(
		[FromServices] IPlayerVipLevelLogRepository repository,
		string playerCode,
		[FromQuery] PlayerVipLevelLogFromQueryCount source)
	{
		return repository.QueryCountAsync(
			playerCode: playerCode,
			startOn: source.StartOn,
			endOn: source.EndOn,
			cancellationToken: HttpContext.RequestAborted);
	}
}
