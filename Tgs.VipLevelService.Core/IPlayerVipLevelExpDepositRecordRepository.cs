using System.Text.Json.Nodes;
using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelExpDepositRecordRepository
{
	ValueTask AddAsync(string hash, string playerCode, PlayerVipLevelExpDepositRecordType type, int sign, decimal amount, decimal balance, JsonNode additionalData, string remark, DateTimeOffset createdOn, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(string hash, CancellationToken cancellationToken = default);
}
