namespace Tgs.VipLevelService.ApplicationServices;
public record VipLevelSnapshotCreateCommand(
	Guid BrandId,
	DateTimeOffset EffectiveOn,
	Guid SnapshotUserId = default,
	string SnapshotUsername = "");
