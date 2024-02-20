using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Repositories;

internal class VipLevelMessageRepository(
	IVipLevelRepository repository,
	IMessageSender messageSender) : IVipLevelRepository
{
	public async ValueTask CreateAsync(Guid id, Guid brandId, VipLevelCreate create, string modifier, CancellationToken cancellationToken = default)
	{
		await repository.CreateAsync(
			id: id,
			brandId: brandId,
			create: create,
			modifier: modifier,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await messageSender.PublishAsync(
			subject: VipLevelServiceNatsConsts.VipLevelSnapshotCreate,
			data: new VipLevelSnapshotRequest
			{
				BrandId = brandId,
				EffectiveOn = DateTimeOffset.UtcNow.ToTimestamp(),
				SnapshotUserId = Guid.Empty,
				SnapshotUsername = modifier
			}.ToByteArray(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public ValueTask<VipLevelInfo?> GetAsync(Guid brandId, Guid id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(brandId, id, cancellationToken);

	public ValueTask<VipLevelInfo?> GetAsync(Guid brandId, int level, CancellationToken cancellationToken = default)
		=> repository.GetAsync(brandId, level, cancellationToken);

	public ValueTask<VipLevelInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> repository.GetAsync(id, cancellationToken);

	public IAsyncEnumerable<VipLevelInfo> QueryAsync(Guid brandId, CancellationToken cancellationToken = default)
		=> repository.QueryAsync(brandId, cancellationToken);

	public async ValueTask UpdateAsync(Guid id, Guid brandId, VipLevelUpdate update, string modifier, CancellationToken cancellationToken = default)
	{
		await repository.UpdateAsync(
			id: id,
			brandId: brandId,
			update: update,
			modifier: modifier,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await messageSender.PublishAsync(
			subject: VipLevelServiceNatsConsts.VipLevelSnapshotCreate,
			data: new VipLevelSnapshotRequest
			{
				BrandId = brandId,
				EffectiveOn = DateTimeOffset.UtcNow.ToTimestamp(),
				SnapshotUserId = Guid.Empty,
				SnapshotUsername = modifier
			}.ToByteArray(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
