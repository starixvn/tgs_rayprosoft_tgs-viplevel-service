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

	ValueTask<PlayerVipLevelInfoViewModel> PlayerVipLevelGetAsync(string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelRegisterAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelManualAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid modifierUserId, string modifierUsername, string remark, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelSystemAsync(string playerCode, int? vipLevel, Guid? vipLevelId = null, CancellationToken cancellationToken = default);

	ValueTask<PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>> PlayerVipLevelLogQueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int page, int rowCount, CancellationToken cancellationToken = default);

	IAsyncEnumerable<VipLevelInfoViewModel> VipLevelSnapshotQueryVipLevelAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Guid> VipLevelSnapshotCurrentSnapshotIdAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelBirthdayFormCreateAsync(string playerCode, DateOnly birthday, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelBirthdayFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerVipLevelBirthdayFormExistAsync(string playerCode, DateOnly birthday, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelBirthdayFormViewModel?> PlayerVipLevelBirthdayFormGetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelCycleFormCreateAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, DateOnly fromOn, DateOnly toOn, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelCycleFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelCycleFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelCycleFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerVipLevelCycleFormExistAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelCycleFormViewModel?> PlayerVipLevelCycleFormGetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelUpgradeFormCreateAsync(string playerCode, int vipLevel, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelUpgradeFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelUpgradeFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelUpgradeFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerVipLevelUpgradeFormExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelUpgradeFormViewModel?> PlayerVipLevelUpgradeFormGetAsync(Guid id, CancellationToken cancellationToken = default);

	public ValueTask PlayerVipLevelExpAddTurnoverAsync(string playerCode, Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, long wagerId, decimal amount, CancellationToken cancellationToken = default);

	public ValueTask PlayerVipLevelExpAddTurnoverCreditAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	public ValueTask PlayerVipLevelExpAddTurnoverDebitAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	public ValueTask PlayerVipLevelExpAddDepositAsync(string playerCode, string formId, decimal amount, CancellationToken cancellationToken = default);

	public ValueTask PlayerVipLevelExpAddDepositCreditAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	public ValueTask PlayerVipLevelExpAddDepositDebitAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<PlayerVipLevelExpFormViewModel?> PlayerVipLevelExpFormGetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelExpFormCreateDepositCreditAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelExpFormCreateDepositDebitAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelExpFormCreateTurnoverCreditAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask<Guid> PlayerVipLevelExpFormCreateTurnoverDebitAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelExpFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelExpFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);

	ValueTask PlayerVipLevelExpFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default);
}
