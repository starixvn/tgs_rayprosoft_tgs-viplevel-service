using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelSnapshotCurrentSnapshotIdGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelSnapshotRepository = Substitute.For<IVipLevelSnapshotRepository>();
		var fakeService = Substitute.For<ICommandService<VipLevelSnapshotCreateCommand>>();

		var query = new VipLevelSnapshotCurrentSnapshotIdQuery(
			BrandId: Guid.NewGuid());

		var snapshotId = Guid.NewGuid();
		_ = fakeVipLevelSnapshotRepository.GetCurrentSnapshotIdAsync(
			brandId: Arg.Is(query.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(snapshotId);

		var service = new VipLevelSnapshotCurrentSnapshotIdGetService(
			fakeVipLevelSnapshotRepository,
			fakeService);

		var actual = await service.GetAsync(query);

		Assert.Equal(snapshotId, actual);
	}

	[Fact]
	public async Task VipLevel_Snapshot_Create()
	{
		var fakeVipLevelSnapshotRepository = Substitute.For<IVipLevelSnapshotRepository>();
		var fakeService = Substitute.For<ICommandService<VipLevelSnapshotCreateCommand>>();

		var query = new VipLevelSnapshotCurrentSnapshotIdQuery(
			BrandId: Guid.NewGuid());

		var snapshotId = Guid.NewGuid();
		_ = fakeVipLevelSnapshotRepository.GetCurrentSnapshotIdAsync(
			brandId: Arg.Is(query.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(null, snapshotId);

		var service = new VipLevelSnapshotCurrentSnapshotIdGetService(
			fakeVipLevelSnapshotRepository,
			fakeService);

		var actual = await service.GetAsync(query);

		Assert.Equal(snapshotId, actual);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Any<VipLevelSnapshotCreateCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
