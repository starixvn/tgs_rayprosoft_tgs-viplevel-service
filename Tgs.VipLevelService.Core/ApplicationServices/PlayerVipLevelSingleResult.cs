using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelSingleResult(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	string PlayerCode,
	string Username,
	Guid VipLevelId,
	int VipLevel,
	PlayerVipLevelAutoState AutoState,
	decimal AccumDeposit,
	decimal AccumTurnover,
	decimal LevelUpAccumulateBetAmount,
	decimal LevelUpAccumulateDeposit,
	bool FullLevel,
	Guid NextVipLevelId,
	int NextVipLevel);
