namespace Tgs.VipLevelService;

public interface IPlayerVipLevelBirthdayPromotionRepository
{
	ValueTask AddAsync(Guid id, Guid snapshotId, Guid vipLevelId, Guid brandId, DateOnly birthday, string playerCode, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<bool> ExistAsync(DateOnly birthday, string playerCode, CancellationToken cancellationToken = default);
}
