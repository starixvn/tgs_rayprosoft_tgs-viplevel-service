using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.Models;

public record PlayerVipLevelLogInfo(
	   Guid Id,
	   string PlayerCode,
	   PlayerVipLevelLogType Type,
	   string Before,
	   string After,
	   DateTimeOffset CreatedOn,
	   Guid CreatedUserId,
	   string CreatedUsername,
	   string Remark);
