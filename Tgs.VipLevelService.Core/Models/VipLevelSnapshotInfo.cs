namespace Tgs.VipLevelService.Models;
public record VipLevelSnapshotInfo(
	Guid Id,
	Guid BrandId,
	string Data,
	DateTimeOffset EffectiveOn,
	DateTimeOffset SnapshotOn,
	Guid SnapshotUserId,
	string SnapshotUsername);
