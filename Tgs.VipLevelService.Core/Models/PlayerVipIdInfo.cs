namespace Tgs.VipLevelService.Models;
public record PlayerVipIdInfo(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	string PlayerCode,
	string Username,
	Guid VipId);
