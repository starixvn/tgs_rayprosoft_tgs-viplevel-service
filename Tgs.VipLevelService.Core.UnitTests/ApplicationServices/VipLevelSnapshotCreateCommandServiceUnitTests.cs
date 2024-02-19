using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.SerialNumberGenerator;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelSnapshotCreateCommandServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakeVipLevelSnapshotRepository = Substitute.For<IVipLevelSnapshotRepository>();

		var command = new VipLevelSnapshotCreateCommand(
			BrandId: Guid.NewGuid(),
			EffectiveOn: DateTimeOffset.Now,
			SnapshotUserId: Guid.NewGuid(),
			SnapshotUsername: "admin.enabled");

		var vipLevelInfos = new[] { new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: 1,
			BrandId: command.BrandId,
			LevelUpAccumulateBetAmount: 0,
			LevelUpAccumulateDeposit: 0,
			LevelKeepAccumulateBetAmount: 0,
			LevelKeepAccumulateDeposit: 0,
			LevelKeepCondition: true,
			BirthBenefit: 0.0m,
			PromotionBenefit: 0.0m,
			QuarterlyBenefit: 0.0m,
			LiveDealerRebate: 0.0m,
			SlotRebate: 0.0m,
			LotteryRebate: 0.0m,
			CardGameRebate: 0.0m,
			SportsRebate: 0.0m,
			VirtualSportsRebate: 0.0m,
			WithdrawTransactionsPerDay: -1,
			WithdrawTotalAmountPerDay: -1)
		};
		_ = fakeVipLevelRepository.QueryAsync(
			brandId: Arg.Is(command.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfos.ToAsyncEnumerable());

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var service = new VipLevelSnapshotCreateCommandService(
			fakeSerialNumberGenerator,
			fakeVipLevelRepository,
			fakeVipLevelSnapshotRepository);

		await service.ExecuteAsync(command);

		await fakeVipLevelSnapshotRepository
			.Received(1)
			.CreateAsync(
			id: Arg.Is(id),
			brandId: Arg.Is(command.BrandId),
			data: Arg.Any<IEnumerable<VipLevelInfo>>(),
			effectiveOn: Arg.Is(command.EffectiveOn),
			snapshotUserId: Arg.Is(command.SnapshotUserId),
			snapshotUsername: Arg.Is(command.SnapshotUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task NullReferenceException()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakeVipLevelSnapshotRepository = Substitute.For<IVipLevelSnapshotRepository>();

		var command = new VipLevelSnapshotCreateCommand(
			BrandId: Guid.NewGuid(),
			EffectiveOn: DateTimeOffset.Now,
			SnapshotUserId: Guid.NewGuid(),
			SnapshotUsername: "admin.enabled");

		_ = fakeVipLevelRepository.QueryAsync(
			brandId: Arg.Is(command.BrandId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<VipLevelInfo>().ToAsyncEnumerable());

		var service = new VipLevelSnapshotCreateCommandService(
			fakeSerialNumberGenerator,
			fakeVipLevelRepository,
			fakeVipLevelSnapshotRepository);

		await Assert.ThrowsAsync<NullReferenceException>(() => service.ExecuteAsync(command).AsTask());
	}
}
