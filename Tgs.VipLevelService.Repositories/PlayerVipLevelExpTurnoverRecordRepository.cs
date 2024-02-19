using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.Repositories;
internal class PlayerVipLevelExpTurnoverRecordRepository(
	TgsContext context) : IPlayerVipLevelExpTurnoverRecordRepository
{
	public async ValueTask AddAsync(string hash, string playerCode, PlayerVipLevelExpTurnoverRecordType type, int sign, decimal amount, decimal balance, JsonNode additionalData, string remark, DateTimeOffset createdOn, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		var record = new PlayerVipLevelExpTurnoverRecord
		{
			Hash = hash,
			PlayerCode = playerCode,
			Type = (int)type,
			Sign = sign,
			Amount = amount,
			Balance = balance,
			AdditionalData = additionalData.ToJsonString(),
			Remark = remark,
			CreatedOn = createdOn.UtcDateTime,
			CreatedUserId = createdUserId,
			CreatedUsername = createdUsername
		};

		_ = await context.PlayerVipLevelExpTurnoverRecords.AddAsync(
			entity: record,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
	public async ValueTask<bool> ExistAsync(string hash, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelExpTurnoverRecords
		.AnyAsync(
			predicate: record => record.SystemOn >= DateTime.UtcNow.AddDays(-30) && record.SystemOn < DateTime.UtcNow && record.Hash == hash,
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
