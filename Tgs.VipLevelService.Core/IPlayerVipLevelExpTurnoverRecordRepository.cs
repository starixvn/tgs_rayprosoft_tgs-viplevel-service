using System.Text.Json.Nodes;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelExpTurnoverRecordRepository
{
	ValueTask AddAsync(string hash, string playerCode, PlayerVipLevelExpTurnoverRecordType type, int sign, decimal amount, decimal balance, JsonNode additionalData, string remark, DateTimeOffset createdOn, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(string hash, CancellationToken cancellationToken = default);
}
