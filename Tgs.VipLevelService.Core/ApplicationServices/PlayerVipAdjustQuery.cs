using Tgs.Paging;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipAdjustQuery(
	Guid BrandId,
	string PlayerCode,
	Paging<PlayerVipAdjustInfo> Paging);
