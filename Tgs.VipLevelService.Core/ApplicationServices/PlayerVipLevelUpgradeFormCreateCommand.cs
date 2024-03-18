using System.Text.Json.Nodes;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelUpgradeFormCreateCommand(
	Guid BrandId,
	string PlayerCode,
	int VipLevel,
	decimal AccumDeposit,
	decimal AccumTurnover,
	decimal Amount,
	Guid OperatorUserId,
	string OperatorUsername,
	VipLevelInfo VipLevelInfo);
