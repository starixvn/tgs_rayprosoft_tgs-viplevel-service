using System.Text.Json.Nodes;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelBirthdayFormCreateCommand(
	Guid BrandId,
	string PlayerCode,
	int VipLevel,
	DateOnly Birthday,
	decimal Amount,
	Guid OperatorUserId,
	string OperatorUsername,
	VipLevelInfo VipLevelInfo);
