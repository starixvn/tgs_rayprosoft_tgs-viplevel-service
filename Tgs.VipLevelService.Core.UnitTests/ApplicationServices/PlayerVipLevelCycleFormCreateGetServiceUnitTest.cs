using NSubstitute;
using Tgs.SerialNumberGenerator;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;

using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelCycleFormCreateGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

		var brandId = Guid.NewGuid();
		var command = new PlayerVipLevelCycleFormCreateCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			CycleType: PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			Amount: 99,
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled",
			VipLevelInfo: new(
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

		var service = new PlayerVipLevelCycleFormCreateGetService(
			fakeSerialNumberGenerator,
			fakeRepository);

		var actual = await service.GetAsync(command);

		Assert.Equal(id, actual);

		_ = fakeRepository.CreateAsync(
			id: Arg.Is(id),
			brandId: Arg.Is(command.BrandId),
			playerCode: Arg.Is(command.PlayerCode),
			vipLevel: Arg.Is(command.VipLevel),
			cycleType: Arg.Is(command.CycleType),
			cycleOn: Arg.Is(command.CycleOn),
			fromOn: Arg.Is(command.FromOn),
			toOn: Arg.Is(command.ToOn),
			amount: Arg.Is(command.Amount),
			snapshotVipLevel: Arg.Is(new PlayerVipLevelCycleFormSnapshotVipLevelInfo(
				VipLevelId: command.VipLevelInfo.Id,
				VipLevel: command.VipLevelInfo.Level,
				BrandId: command.VipLevelInfo.BrandId,
				QuarterlyBenefit: command.VipLevelInfo.QuarterlyBenefit)),
			createdUserId: Arg.Is(command.OperatorUserId),
			createdUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
