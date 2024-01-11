using Tgs.Paging;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipAdjustQuery(
	string PlayerCode,
	(DateTimeOffset Begin, DateTimeOffset End)? CreateOnRange,
	Paging<PlayerVipAdjustInfo> Paging);
