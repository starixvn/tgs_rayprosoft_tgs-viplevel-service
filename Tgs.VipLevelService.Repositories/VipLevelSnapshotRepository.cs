using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class VipLevelSnapshotRepository(
	TgsContext context) : IVipLevelSnapshotRepository
{
	public async ValueTask CreateAsync(Guid id, Guid brandId, IEnumerable<VipLevelInfo> data, DateTimeOffset effectiveOn, Guid snapshotUserId = default, string snapshotUsername = "", CancellationToken cancellationToken = default)
	{
		var entity = new VipLevelSnapshot
		{
			Id = id,
			BrandId = brandId,
			Data = JsonSerializer.Serialize(data),
			EffectiveOn = effectiveOn.UtcDateTime,
			SnapshotOn = DateTimeOffset.UtcNow.UtcDateTime,
			SnapshotUserId = snapshotUserId,
			SnapshotUsername = snapshotUsername
		};

		_ = await context.VipLevelSnapshots.AddAsync(
			entity,
			cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public ValueTask<VipLevelSnapshotInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> context.VipLevelSnapshots
		.Where(predicate: vls => vls.Id == id && vls.EffectiveOn <= DateTime.UtcNow)
		.Select(selector: vls => new VipLevelSnapshotInfo(
			vls.Id,
			vls.BrandId,
			vls.Data,
			vls.EffectiveOn,
			vls.SnapshotOn,
			vls.SnapshotUserId,
			vls.SnapshotUsername))
		.ToAsyncEnumerable()
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken);

	public async ValueTask<Guid?> GetCurrentSnapshotIdAsync(Guid brandId, CancellationToken cancellationToken = default)
		=> await context.VipLevelSnapshots.AsQueryable()
		.Where(predicate: vls => vls.BrandId == brandId && vls.EffectiveOn <= DateTime.UtcNow)
		.Select(selector: vls => (Guid?)vls.Id)
		.OrderByDescending(keySelector: id => id)
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
