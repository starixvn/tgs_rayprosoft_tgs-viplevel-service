using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.SerialNumberGenerator;
using Tgs.Service.PlayerBuckets;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelUpgradePromotionCommandServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelUpgradePromotionCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelUpgradePromotionRepository = Substitute.For<IPlayerVipLevelUpgradePromotionRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelUpgradePromotionCommand(
			SnapshotId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevelId: Guid.NewGuid(),
			Amount: 100m);

		var playerInfo = new PlayerInfo(
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"GPI",
			"4ucmqplxb029",
			"4ucmqplxb029",
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		_ = fakePlayerVipLevelUpgradePromotionRepository.ExistAsync(
			vipLevelId: Arg.Is(command.VipLevelId),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var service = new PlayerVipLevelUpgradePromotionCommandService(
			fakeLog,
			fakePlayerRepository,
			fakePlayerVipLevelUpgradePromotionRepository,
			fakeSerialNumberGenerator,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelUpgradePromotionRepository
			.Received(1)
			.AddAsync(
			id: id,
			snapshotId: Arg.Is(command.SnapshotId),
			vipLevelId: Arg.Is(command.VipLevelId),
			playerCode: Arg.Is(command.PlayerCode),
			amount: Arg.Is(command.Amount),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerBucketClient
			.Received(1)
			.CreateBucketAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			amount: Arg.Is(command.Amount),
			type: Arg.Is(BucketType.Vip),
			externalId: Arg.Is(id.ToString()),
			remark: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(1)
			.PromotionVipLevelUpgradeAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_Player_Has_Already_Been_Sent_Promotions()
	{
		var fakeLog = NullLogger<PlayerVipLevelUpgradePromotionCommandService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelUpgradePromotionRepository = Substitute.For<IPlayerVipLevelUpgradePromotionRepository>();
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelUpgradePromotionCommand(
			SnapshotId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevelId: Guid.NewGuid(),
			Amount: 100m);

		var playerInfo = new PlayerInfo(
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"GPI",
			"4ucmqplxb029",
			"4ucmqplxb029",
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		_ = fakePlayerVipLevelUpgradePromotionRepository.ExistAsync(
			vipLevelId: Arg.Is(command.VipLevelId),
			playerCode: Arg.Is(playerInfo.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var service = new PlayerVipLevelUpgradePromotionCommandService(
			fakeLog,
			fakePlayerRepository,
			fakePlayerVipLevelUpgradePromotionRepository,
			fakeSerialNumberGenerator,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelUpgradePromotionRepository
			.Received(0)
			.AddAsync(
			id: id,
			snapshotId: Arg.Is(command.SnapshotId),
			vipLevelId: Arg.Is(command.VipLevelId),
			playerCode: Arg.Is(command.PlayerCode),
			amount: Arg.Is(command.Amount),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerBucketClient
			.Received(0)
			.CreateBucketAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			amount: Arg.Is(command.Amount),
			type: Arg.Is(BucketType.Vip),
			externalId: Arg.Is(id.ToString()),
			remark: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.PromotionVipLevelUpgradeAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
