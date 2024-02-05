using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.Repositories;
internal class PlayerVipLevelBirthdayPromotionRepository(TgsContext context) : IPlayerVipLevelBirthdayPromotionRepository
{
	public async ValueTask AddAsync(Guid id, Guid snapshotId, Guid vipLevelId, Guid brandId, DateOnly birthday, string playerCode, decimal amount, CancellationToken cancellationToken = default)
	{
		var promotion = new PlayerVipLevelBirthdayPromotion
		{
			Id = id,
			BrandId = brandId,
			Birthday = birthday.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
			ExecuteYear = birthday.Year,
			PlayerVipLevelSnapshotId = snapshotId,
			VipId = vipLevelId,
			PlayerCode = playerCode,
			Amount = amount,
			CreatedOn = DateTime.UtcNow
		};
		_ = await context.PlayerVipLevelBirthdayPromotions
			.AddAsync(
			entity: promotion,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context
			.SaveChangesAsync(
			cancellationToken: cancellationToken).ConfigureAwait(false);
	}
	public async ValueTask<bool> ExistAsync(DateOnly birthday, string playerCode, CancellationToken cancellationToken = default)
	=> await context.PlayerVipLevelBirthdayPromotions
		.Where(predicate: vlp => vlp.ExecuteYear == birthday.Year && vlp.PlayerCode == playerCode)
		.Select(selector: vlp => vlp.Id)
		.AnyAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
