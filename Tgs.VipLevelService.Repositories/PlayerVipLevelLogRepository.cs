using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelLogRepository(
	TgsContext context) : IPlayerVipLevelLogRepository
{
	public async ValueTask CreateAsync(Guid id, string playerCode, PlayerVipLevelLogType type, PlayerVipLevelInfo? before, PlayerVipLevelInfo after, Guid createdUserId = default, string createdUsername = "", CancellationToken cancellationToken = default)
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
		 .Select(selector: selector => new PlayerVipLevelLogInfo(
			 selector.Id,
			 selector.PlayerCode,
			 (PlayerVipLevelLogType)selector.Type,
			 selector.Before,
			 selector.After,
			 selector.CreatedOn,
			 selector.CreatedUserId,
			 selector.CreatedUsername))
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
