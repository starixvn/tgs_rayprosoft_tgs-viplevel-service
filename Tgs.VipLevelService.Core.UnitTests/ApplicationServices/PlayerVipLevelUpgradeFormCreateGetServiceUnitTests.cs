using NSubstitute;
using Tgs.SerialNumberGenerator;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelUpgradeFormCreateGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakeRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var brandId = Guid.NewGuid();
		var command = new PlayerVipLevelUpgradeFormCreateCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			1,
			100m,
			123m,
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
				BirthBenefit: 100.0m,
				PromotionBenefit: 99.0m,
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

		var service = new PlayerVipLevelUpgradeFormCreateGetService(
			fakeSerialNumberGenerator,
			fakeRepository,
			fakeMessageSender);

		var actual = await service.GetAsync(command);

		Assert.Equal(id, actual);

		_ = fakeRepository.CreateAsync(
			id: Arg.Is(id),
			brandId: Arg.Is(command.BrandId),
			playerCode: Arg.Is(command.PlayerCode),
			vipLevel: Arg.Is(command.VipLevel),
			autoState: Arg.Is(true),
			amount: Arg.Is(command.Amount),
			snapshotVipLevel: Arg.Is(new PlayerVipLevelUpgradeFormSnapshotVipLevelInfo(
				VipLevelId: command.VipLevelInfo.Id,
				VipLevel: command.VipLevelInfo.Level,
				BrandId: command.VipLevelInfo.BrandId,
				AccumDeposit: command.AccumDeposit,
				AccumTurnover: command.AccumTurnover,
				LevelUpAccumulateBetAmount: command.VipLevelInfo.LevelUpAccumulateBetAmount,
				LevelUpAccumulateDeposit: command.VipLevelInfo.LevelUpAccumulateDeposit,
				PromotionBenefit: command.VipLevelInfo.PromotionBenefit)),
			createdUserId: Arg.Is(command.OperatorUserId),
			createdUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(VipLevelServiceNatsConsts.PlayerVipLevelUpgradeFormAuto),
			data: Arg.Is(new VipLevelFormRequest
			{
				FormId = id,
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
