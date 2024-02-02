using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelLogRepository
{
	ValueTask CreateAsync(Guid id, string playerCode, PlayerVipLevelLogType type, PlayerVipLevelInfo? before, PlayerVipLevelInfo after, Guid createdUserId = default, string createdUsername = "", string remark = "", CancellationToken cancellationToken = default);

	IAsyncEnumerable<PlayerVipLevelLogInfo> QueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int offset = 0, int limit = 20, CancellationToken cancellationToken = default);

	ValueTask<int> QueryCountAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);
}
