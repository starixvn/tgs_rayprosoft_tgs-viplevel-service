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
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelBirthdayFormApprovedCommandServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelBirthdayFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelBirthdayFormRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelBirthdayFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 99m,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled");

		var form = new PlayerVipLevelBirthdayFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			Birthday: DateOnly.Parse("2024-02-06"),
			State: PlayerVipLevelBirthdayFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin.enabled");
		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: form.PlayerCode,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var service = new PlayerVipLevelBirthdayFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelBirthdayFormRepository,
			fakePlayerRepository,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelBirthdayFormRepository
			.Received(1)
			.ApprovedAsync(
			id: form.Id,
			amount: Arg.Is(command.Amount),
			remark: Arg.Is(command.Remark),
			updatedUserId: Arg.Is(command.OperatorUserId),
			updatedUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerBucketClient
			.Received(1)
			.CreateBucketAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			amount: Arg.Is(command.Amount),
			type: Arg.Is(BucketType.Vip),
			externalId: Arg.Is(form.Id.ToString()),
			remark: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(1)
			.PromotionVipLevelBirthdayAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(form.Id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_birthday_bonus_is_0()
	{
		var fakeLog = NullLogger<PlayerVipLevelBirthdayFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelBirthdayFormRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelBirthdayFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 0.0m,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled");

		var form = new PlayerVipLevelBirthdayFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			Birthday: DateOnly.Parse("2024-02-06"),
			State: PlayerVipLevelBirthdayFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin.enabled");
		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: form.PlayerCode,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var service = new PlayerVipLevelBirthdayFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelBirthdayFormRepository,
			fakePlayerRepository,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelBirthdayFormRepository
			.Received(1)
			.ApprovedAsync(
			id: form.Id,
			amount: Arg.Is(command.Amount),
			remark: Arg.Is(command.Remark),
			updatedUserId: Arg.Is(command.OperatorUserId),
			updatedUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerBucketClient
			.Received(0)
			.CreateBucketAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			amount: Arg.Is(command.Amount),
			type: Arg.Is(BucketType.Vip),
			externalId: Arg.Is(form.Id.ToString()),
			remark: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.PromotionVipLevelBirthdayAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(form.Id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_has_been_reviewed()
	{
		var fakeLog = NullLogger<PlayerVipLevelBirthdayFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelBirthdayFormRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelBirthdayFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 99m,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled");

		var form = new PlayerVipLevelBirthdayFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			Birthday: DateOnly.Parse("2024-02-06"),
			State: PlayerVipLevelBirthdayFormState.Approved,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin.enabled");
		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "test",
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: form.PlayerCode,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var service = new PlayerVipLevelBirthdayFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelBirthdayFormRepository,
			fakePlayerRepository,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelBirthdayFormRepository
			.Received(0)
			.ApprovedAsync(
			id: form.Id,
			amount: Arg.Is(command.Amount),
			remark: Arg.Is(command.Remark),
			updatedUserId: Arg.Is(command.OperatorUserId),
			updatedUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerBucketClient
			.Received(0)
			.CreateBucketAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			amount: Arg.Is(command.Amount),
			type: Arg.Is(BucketType.Vip),
			externalId: Arg.Is(form.Id.ToString()),
			remark: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeCashServiceClient
			.Received(0)
			.PromotionVipLevelBirthdayAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(form.Id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
