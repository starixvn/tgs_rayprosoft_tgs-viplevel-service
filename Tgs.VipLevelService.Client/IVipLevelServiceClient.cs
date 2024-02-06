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

	[Obsolete("已過時,請改使用PlayerVipLevelLogQueryAsync")]
	ValueTask<PagingResponseViewModel<PlayerVipAdjustInfo>> QueryPlayerVipAdjustInfosAsync(PlayerVipAdjustQueryConditions conditions, Paging paging, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelInfoViewModel> PlayerVipLevelGetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelExpInfoViewModel> PlayerVipLevelExpGetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelRegisterAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelManualAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid modifierUserId, string modifierUsername, string remark, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelSystemAsync(string playerCode, int? vipLevel, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask<PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>> PlayerVipLevelLogQueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int page, int rowCount, CancellationToken cancellationToken = default);

	IAsyncEnumerable<VipLevelInfoViewModel> VipLevelSnapshotQueryVipLevelAsync(Guid id, CancellationToken cancellationToken = default);
	ValueTask<Guid> VipLevelSnapshotCurrentSnapshotIdAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelUpgradePromotionAsync(string playerCode, Guid vipLevelId, Guid snapshotId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelBirthdayFormCreateAsync(string playerCode, DateOnly birthday, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);
	ValueTask<bool> PlayerVipLevelBirthdayFormExistAsync(string playerCode, DateOnly birthday, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelBirthdayFormViewModel?> PlayerVipLevelBirthdayFormGetAsync(Guid id, CancellationToken cancellationToken = default);
}
