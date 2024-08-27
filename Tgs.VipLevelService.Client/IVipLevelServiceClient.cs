namespace Tgs.VipLevelService.Clients;

public interface IVipLevelServiceClient
{
	ValueTask<Guid> VipLevelCreateAsync(Guid brandId, Common.ViewModels.VipLevelCreateViewModel source, string modifier, CancellationToken cancellationToken = default);

	IAsyncEnumerable<Common.ViewModels.VipLevelInfoViewModel> VipLevelQueryAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.VipLevelInfoViewModel?> VipLevelGetAsync(Guid brandId, int level, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.VipLevelInfoViewModel?> VipLevelGetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask VipLevelUpdateAsync(Guid vipId, Common.ViewModels.VipLevelUpdateViewModel source, string modifier, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.PlayerVipLevelInfoViewModel> PlayerVipLevelGetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask<int> PlayerVipLevelInfoGetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelRegisterAsync(string playerCode, int? vipLevel, Common.PlayerVipLevelAutoState autoState, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelManualAsync(string playerCode, int? vipLevel, Common.PlayerVipLevelAutoState autoState, Guid modifierUserId, string modifierUsername, string remark, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelSystemAsync(string playerCode, int? vipLevel, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	IAsyncEnumerable<Common.ViewModels.PlayerVipLevelLogQueryViewModel> PlayerVipLevelLogQueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int page, int rowCount, CancellationToken cancellationToken = default);

	ValueTask<int> PlayerVipLevelLogQueryCountAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default);

	IAsyncEnumerable<Common.ViewModels.VipLevelInfoViewModel> VipLevelSnapshotQueryVipLevelAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Guid> VipLevelSnapshotCurrentSnapshotIdAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelBirthdayFormCreateAsync(string playerCode, DateOnly birthday, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerVipLevelBirthdayFormExistAsync(string playerCode, DateOnly birthday, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.PlayerVipLevelBirthdayFormViewModel?> PlayerVipLevelBirthdayFormGetAsync(Guid id, CancellationToken cancellationToken = default);

	IAsyncEnumerable<Common.ViewModels.PlayerVipLevelBirthdayFormQueryViewModel?> PlayerVipLevelBirthdayFormQueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelBirthdayFormState> states, int page = 1, int rowCount = 20, CancellationToken cancellationToken = default);

	ValueTask<int> PlayerVipLevelBirthdayFormQueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelBirthdayFormState> states, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelCycleFormCreateAsync(string playerCode, Common.PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, DateOnly fromOn, DateOnly toOn, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelCycleFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelCycleFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelCycleFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerVipLevelCycleFormExistAsync(string playerCode, Common.PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.PlayerVipLevelCycleFormViewModel?> PlayerVipLevelCycleFormGetAsync(Guid id, CancellationToken cancellationToken = default);

	IAsyncEnumerable<Common.ViewModels.PlayerVipLevelCycleFormQueryViewModel?> PlayerVipLevelCycleFormQueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelCycleFormState> states, int page = 1, int rowCount = 20, CancellationToken cancellationToken = default);

	ValueTask<int> PlayerVipLevelCycleFormQueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelCycleFormState> states, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelUpgradeFormCreateAsync(string playerCode, int vipLevel, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelUpgradeFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelUpgradeFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelUpgradeFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerVipLevelUpgradeFormExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default);

	ValueTask<Common.ViewModels.PlayerVipLevelUpgradeFormViewModel?> PlayerVipLevelUpgradeFormGetAsync(Guid id, CancellationToken cancellationToken = default);

	IAsyncEnumerable<Common.ViewModels.PlayerVipLevelUpgradeFormQueryViewModel?> PlayerVipLevelUpgradeFormQueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelUpgradeFormState> states, int page = 1, int rowCount = 20, CancellationToken cancellationToken = default);

	ValueTask<int> PlayerVipLevelUpgradeFormQueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelUpgradeFormState> states, CancellationToken cancellationToken = default);
}
