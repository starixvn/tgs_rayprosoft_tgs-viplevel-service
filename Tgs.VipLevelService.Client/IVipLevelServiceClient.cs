using System.Net.Http;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

public interface IVipLevelServiceClient
{
	ValueTask<Guid> VipLevelCreateAsync(Guid brandId, VipLevelCreateViewModel source, string modifier, CancellationToken cancellationToken = default);

	IAsyncEnumerable<VipLevelInfoViewModel> VipLevelQueryAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<VipLevelInfoViewModel?> VipLevelGetAsync(Guid brandId, int level, CancellationToken cancellationToken = default);

	ValueTask<VipLevelInfoViewModel?> VipLevelGetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask VipLevelUpdateAsync(Guid vipId, VipLevelUpdateViewModel source, string modifier, CancellationToken cancellationToken = default);

	ValueTask<PagingResponseViewModel<PlayerVipAdjustInfo>> QueryPlayerVipAdjustInfosAsync(PlayerVipAdjustQueryConditions conditions, Paging paging, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelInfoViewModel> PlayerVipLevelGetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelRegisterAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelManualAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid modifierUserId, string modifierUsername, string remark, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelSystemAsync(string playerCode, int? vipLevel, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask<PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>> PlayerVipLevelLogQueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int page, int rowCount, CancellationToken cancellationToken = default);
}
