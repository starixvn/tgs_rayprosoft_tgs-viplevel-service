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

public class PlayerVipLevelCycleFormApprovedCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelCycleFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelCycleFormRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelCycleFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 99m,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled");

		var form = new PlayerVipLevelCycleFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			CycleType: PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			State: PlayerVipLevelCycleFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin.enabled");
		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
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

		var service = new PlayerVipLevelCycleFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelCycleFormRepository,
			fakePlayerRepository,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelCycleFormRepository
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
			.PromotionVipLevelSeasonAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(form.Id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_birthday_bonus_is_0()
	{
		var fakeLog = NullLogger<PlayerVipLevelCycleFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelCycleFormRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelCycleFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 0.0m,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled");

		var form = new PlayerVipLevelCycleFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			CycleType: PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			State: PlayerVipLevelCycleFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin.enabled");
		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
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

		var service = new PlayerVipLevelCycleFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelCycleFormRepository,
			fakePlayerRepository,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelCycleFormRepository
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
			.PromotionVipLevelSeasonAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(form.Id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_has_been_reviewed()
	{
		var fakeLog = NullLogger<PlayerVipLevelCycleFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelCycleFormRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();
		var fakeCashServiceClient = Substitute.For<ICashServiceClient>();

		var command = new PlayerVipLevelCycleFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 99m,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled");

		var form = new PlayerVipLevelCycleFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			CycleType: PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			State: PlayerVipLevelCycleFormState.Approved,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin.enabled");
		_ = fakePlayerVipLevelCycleFormRepository.GetAsync(
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

		var service = new PlayerVipLevelCycleFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelCycleFormRepository,
			fakePlayerRepository,
			fakePlayerBucketClient,
			fakeCashServiceClient);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelCycleFormRepository
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
			.PromotionVipLevelSeasonAsync(
			walletId: Arg.Is(playerInfo.WalletId),
			amount: Arg.Is(command.Amount),
			description: Arg.Any<string>(),
			externalId: Arg.Is(form.Id.ToString()),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
