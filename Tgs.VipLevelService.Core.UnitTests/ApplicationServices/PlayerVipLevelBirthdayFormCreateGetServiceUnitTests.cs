using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.VipLevels.Exp.Worker.Common.Protos;
using Tgs.VipLevels.Exp.Worker.Common;
using Tgs.VipLevelService.ApplicationServices;
using Valhalla.MessageQueue;
using Tgs.VipLevelService.Models;
using Tgs.SerialNumberGenerator;
using NSubstitute.Core;
using System.Threading;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelBirthdayFormCreateGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();

		var brandId = Guid.NewGuid();
		var command = new PlayerVipLevelBirthdayFormCreateCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			1,
			DateOnly.Parse("2024-02-06"),
			99,
			Guid.NewGuid(),
			"admin.enabled",
			new(
				Id: Guid.NewGuid(),
				Level: 1,
				BrandId: brandId,
				LevelUpAccumulateBetAmount: 0,
				LevelUpAccumulateDeposit: 0,
				LevelKeepAccumulateBetAmount: 0,
				LevelKeepAccumulateDeposit: 0,
				LevelKeepCondition: true,
				BirthBenefit: 99.0m,
				PromotionBenefit: 100.0m,
				QuarterlyBenefit: 100.0m,
				LiveDealerRebate: 0.0m,
				SlotRebate: 0.0m,
				LotteryRebate: 0.0m,
				CardGameRebate: 0.0m,
				SportsRebate: 0.0m,
				VirtualSportsRebate: 0.0m,
				WithdrawTransactionsPerDay: -1,
				WithdrawTotalAmountPerDay: -1));

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

		var service = new PlayerVipLevelBirthdayFormCreateGetService(
			fakeSerialNumberGenerator,
			fakeRepository);

		var actual = await service.GetAsync(command);

		Assert.Equal(id, actual);

		_ = fakeRepository.CreateAsync(
			id: Arg.Is(id),
			brandId: Arg.Is(command.BrandId),
			playerCode: Arg.Is(command.PlayerCode),
			vipLevel: Arg.Is(command.VipLevel),
			birthday: Arg.Is(command.Birthday),
			amount: Arg.Is(command.Amount),
			snapshotVipLevel: Arg.Is(new PlayerVipLevelBirthdaySnapshotVipLevelInfo(
				VipLevelId: command.VipLevelInfo.Id,
				VipLevel: command.VipLevelInfo.Level,
				BrandId: command.VipLevelInfo.BrandId,
				BirthBenefit: command.VipLevelInfo.BirthBenefit)),
			createdUserId: Arg.Is(command.OperatorUserId),
			createdUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
