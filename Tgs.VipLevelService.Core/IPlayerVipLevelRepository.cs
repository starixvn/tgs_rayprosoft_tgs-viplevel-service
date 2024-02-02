using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IPlayerVipLevelRepository
{
	public ValueTask MergeAsync(string playerCode, Guid vipLevelId, int vipLevel, PlayerVipLevelAutoState autoState, CancellationToken cancellationToken = default);

	public ValueTask UpdateSystemAsync(string playerCode, Guid vipLevelId, int vipLevel, CancellationToken cancellationToken = default);

	public ValueTask<PlayerVipLevelInfo?> GetAsync(string playerCode, CancellationToken cancellationToken = default);
}
