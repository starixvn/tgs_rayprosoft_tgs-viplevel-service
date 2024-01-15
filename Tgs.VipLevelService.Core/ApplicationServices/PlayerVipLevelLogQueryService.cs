using System.Runtime.CompilerServices;
using System.Text.Json;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelLogQueryService(IPlayerVipLevelLogRepository repository)
	: IQueryService<PlayerVipLevelLogQuery, PlayerVipLevelLogResult>,
	IGetService<PlayerVipLevelLogCountQuery, int>

{
	public ValueTask<int> GetAsync(PlayerVipLevelLogCountQuery query, CancellationToken cancellationToken = default)
		=> repository.QueryCountAsync(
			playerCode: query.PlayerCode,
			startOn: query.StartOn,
			endOn: query.EndOn,
			cancellationToken: cancellationToken);

	public async IAsyncEnumerable<PlayerVipLevelLogResult> QueryAsync(PlayerVipLevelLogQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var playerVipLevelLogInfos = await repository.QueryAsync(
			playerCode: query.PlayerCode,
			startOn: query.StartOn,
			endOn: query.EndOn,
			offset: query.Page - 1,
			limit: query.RowCount,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (playerVipLevelLogInfos.Length == 0)
		{
			yield break;
		}

		foreach (var playerVipLevelLogInfo in playerVipLevelLogInfos)
		{
			if (playerVipLevelLogInfo is null)
			{
				continue;
			}

			var before = JsonSerializer.Deserialize<PlayerVipLevelInfo>(playerVipLevelLogInfo.Before);
			var after = JsonSerializer.Deserialize<PlayerVipLevelInfo>(playerVipLevelLogInfo.After);

			int BeforeLevel = 0;
			Guid BeforeLevelId = Guid.Empty;
			PlayerVipLevelAutoState BeforeAutoState = PlayerVipLevelAutoState.ENABLE;
			if (before is not null)
			{
				BeforeLevel = before.VipLevel;
				BeforeLevelId = before.VipLevelId;
				BeforeAutoState = before.AutoState;
			}

			var AfterLevel = 0;
			var AfterLevelId = Guid.Empty;
			var AfterAutoState = PlayerVipLevelAutoState.ENABLE;
			if (after is not null)
			{
				AfterLevel = after.VipLevel;
				AfterLevelId = after.VipLevelId;
				AfterAutoState = after.AutoState;
			}

			yield return new PlayerVipLevelLogResult(
				Type: playerVipLevelLogInfo.Type,
				BeforeLevel: BeforeLevel,
				BeforeLevelId: BeforeLevelId,
				BeforeAutoState: BeforeAutoState,
				AfterLevel: AfterLevel,
				AfterLevelId: AfterLevelId,
				AfterAutoState: AfterAutoState,
				CreatedOn: playerVipLevelLogInfo.CreatedOn,
				CreatedUserId: playerVipLevelLogInfo.CreatedUserId,
				CreatedUsername: playerVipLevelLogInfo.CreatedUsername,
				Remark: playerVipLevelLogInfo.Remark);
		}
	}
}
