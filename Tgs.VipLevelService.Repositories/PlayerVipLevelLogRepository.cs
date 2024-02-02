using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelLogRepository(
	TgsContext context) : IPlayerVipLevelLogRepository
{
	public async ValueTask CreateAsync(Guid id, string playerCode, PlayerVipLevelLogType type, PlayerVipLevelInfo? before, PlayerVipLevelInfo after, Guid createdUserId = default, string createdUsername = "", string remark = "", CancellationToken cancellationToken = default)
	{
		string beforeJsonString = (before is null) ? "{}" : JsonSerializer.Serialize(before);

		var entity = new PlayerVipLevelLog
		{
			Id = id,
			PlayerCode = playerCode,
			Type = (int)type,
			Before = beforeJsonString,
			After = JsonSerializer.Serialize(after),
			CreatedUserId = createdUserId,
			CreatedUsername = createdUsername,
			Remark = remark
		};

		_ = await context.PlayerVipLevelLogs
			.AddAsync(
			entity: entity,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public IAsyncEnumerable<PlayerVipLevelLogInfo> QueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
	{
		var query = context.PlayerVipLevelLogs.Where(predicate => predicate.CreatedOn >= startOn.UtcDateTime
			 && predicate.CreatedOn < endOn.UtcDateTime
			 && predicate.PlayerCode == playerCode);

		return query
		 .OrderByDescending(keySelector: keySelector => keySelector.CreatedOn)
		 .Skip(limit * offset)
		 .Take(limit)
		 .Select(selector: log => new PlayerVipLevelLogInfo(
			 log.Id,
			 log.PlayerCode,
			 (PlayerVipLevelLogType)log.Type,
			 log.Before,
			 log.After,
			 log.CreatedOn,
			 log.CreatedUserId,
			 log.CreatedUsername,
			 log.Remark))
		 .ToAsyncEnumerable();
	}

	public async ValueTask<int> QueryCountAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default)
	{
		var query = context.PlayerVipLevelLogs.Where(predicate => predicate.CreatedOn >= startOn.UtcDateTime
			 && predicate.CreatedOn < endOn.UtcDateTime
			 && predicate.PlayerCode == playerCode);

		return await query
			.CountAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
