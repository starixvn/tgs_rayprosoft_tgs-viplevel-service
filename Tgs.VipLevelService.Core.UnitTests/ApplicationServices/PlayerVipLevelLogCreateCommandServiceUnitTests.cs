using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelLogCreateCommandServiceUnitTests
{
	[Fact]
	public async Task Normal_Process_Register()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelLogRepository = Substitute.For<IPlayerVipLevelLogRepository>();

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			Type: PlayerVipLevelLogType.Register,
			BeforeLevel: 0,
			BeforeAutoState: PlayerVipLevelAutoState.ENABLE,
			AfterLevel: 1,
			AfterAutoState: PlayerVipLevelAutoState.ENABLE,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			Remark: string.Empty);

		_ = fakeVipLevelRepository
			.GetAsync(
			brandId: Arg.Is(command.BrandId),
			level: Arg.Is(command.BeforeLevel!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((VipLevelInfo?)null);

		var afterVipLevelInfoInfo = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: command.AfterLevel,
			BrandId: command.BrandId,
			LevelUpAccumulateBetAmount: 100,
			LevelUpAccumulateDeposit: 100,
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
			WithdrawTotalAmountPerDay: -1);
		_ = fakeVipLevelRepository.GetAsync(
			brandId: Arg.Is(command.BrandId),
			level: Arg.Is(command.AfterLevel),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(afterVipLevelInfoInfo);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var service = new PlayerVipLevelLogCreateCommandService(
			fakeSerialNumberGenerator,
			fakeVipLevelRepository,
			fakePlayerVipLevelLogRepository);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelLogRepository
			.Received(1)
			.CreateAsync(
			id: Arg.Is(id),
			playerCode: Arg.Is(command.PlayerCode),
			type: Arg.Is(PlayerVipLevelLogType.Register),
			before: Arg.Is((PlayerVipLevelInfo?)null),
			after: Arg.Is(new PlayerVipLevelInfo(
				PlayerCode: command.PlayerCode,
				VipLevelId: afterVipLevelInfoInfo.Id,
				VipLevel: afterVipLevelInfoInfo.Level,
				AutoState: command.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE)),
			createdUserId: Arg.Is(command.CreatedUserId),
			createdUsername: Arg.Is(command.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process_System()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelLogRepository = Substitute.For<IPlayerVipLevelLogRepository>();

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			Type: PlayerVipLevelLogType.System,
			BeforeLevel: 1,
			BeforeAutoState: PlayerVipLevelAutoState.ENABLE,
			AfterLevel: 2,
			AfterAutoState: PlayerVipLevelAutoState.ENABLE,
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			Remark: string.Empty);

		var beforeVipLevelInfoInfo = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: command.BeforeLevel!.Value,
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
			WithdrawTotalAmountPerDay: -1);
		_ = fakeVipLevelRepository.GetAsync(
			brandId: Arg.Is(command.BrandId),
			level: Arg.Is(command.BeforeLevel!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(beforeVipLevelInfoInfo);

		var afterVipLevelInfoInfo = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: command.AfterLevel,
			BrandId: command.BrandId,
			LevelUpAccumulateBetAmount: 100,
			LevelUpAccumulateDeposit: 100,
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
			WithdrawTotalAmountPerDay: -1);
		_ = fakeVipLevelRepository.GetAsync(
			brandId: Arg.Is(command.BrandId),
			level: Arg.Is(command.AfterLevel),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(afterVipLevelInfoInfo);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var service = new PlayerVipLevelLogCreateCommandService(
			fakeSerialNumberGenerator,
			fakeVipLevelRepository,
			fakePlayerVipLevelLogRepository);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelLogRepository
			.Received(1)
			.CreateAsync(
			id: Arg.Is(id),
			playerCode: Arg.Is(command.PlayerCode),
			type: Arg.Is(PlayerVipLevelLogType.System),
			before: Arg.Is(new PlayerVipLevelInfo(
				PlayerCode: command.PlayerCode,
				VipLevelId: beforeVipLevelInfoInfo.Id,
				VipLevel: beforeVipLevelInfoInfo.Level,
				AutoState: command.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE)),
			after: Arg.Is(new PlayerVipLevelInfo(
				PlayerCode: command.PlayerCode,
				VipLevelId: afterVipLevelInfoInfo.Id,
				VipLevel: afterVipLevelInfoInfo.Level,
				AutoState: command.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE)),
			createdUserId: Arg.Is(command.CreatedUserId),
			createdUsername: Arg.Is(command.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process_Manual()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelLogRepository = Substitute.For<IPlayerVipLevelLogRepository>();

		var command = new PlayerVipLevelLogCreateCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			Type: PlayerVipLevelLogType.Manual,
			BeforeLevel: 1,
			BeforeAutoState: PlayerVipLevelAutoState.ENABLE,
			AfterLevel: 2,
			AfterAutoState: PlayerVipLevelAutoState.ENABLE,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			Remark:"test");

		var beforeVipLevelInfoInfo = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: command.BeforeLevel!.Value,
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
			WithdrawTotalAmountPerDay: -1);
		_ = fakeVipLevelRepository.GetAsync(
			brandId: Arg.Is(command.BrandId),
			level: Arg.Is(command.BeforeLevel!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(beforeVipLevelInfoInfo);

		var afterVipLevelInfoInfo = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: command.AfterLevel,
			BrandId: command.BrandId,
			LevelUpAccumulateBetAmount: 100,
			LevelUpAccumulateDeposit: 100,
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
			WithdrawTotalAmountPerDay: -1);
		_ = fakeVipLevelRepository.GetAsync(
			brandId: Arg.Is(command.BrandId),
			level: Arg.Is(command.AfterLevel),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(afterVipLevelInfoInfo);

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var service = new PlayerVipLevelLogCreateCommandService(
			fakeSerialNumberGenerator,
			fakeVipLevelRepository,
			fakePlayerVipLevelLogRepository);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelLogRepository
			.Received(1)
			.CreateAsync(
			id: Arg.Is(id),
			playerCode: Arg.Is(command.PlayerCode),
			type: Arg.Is(PlayerVipLevelLogType.Manual),
			before: Arg.Is(new PlayerVipLevelInfo(
				PlayerCode: command.PlayerCode,
				VipLevelId: beforeVipLevelInfoInfo.Id,
				VipLevel: beforeVipLevelInfoInfo.Level,
				AutoState: command.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE)),
			after: Arg.Is(new PlayerVipLevelInfo(
				PlayerCode: command.PlayerCode,
				VipLevelId: afterVipLevelInfoInfo.Id,
				VipLevel: afterVipLevelInfoInfo.Level,
				AutoState: command.BeforeAutoState ?? PlayerVipLevelAutoState.ENABLE)),
			createdUserId: Arg.Is(command.CreatedUserId),
			createdUsername: Arg.Is(command.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
