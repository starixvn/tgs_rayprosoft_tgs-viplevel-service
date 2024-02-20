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
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;
using Tgs.VipLevelService.Core.Protos;
using Tgs.SerialNumberGenerator;
using NSubstitute.Core;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelExpFormCreateTurnoverCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process_Credit()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerVipLevelExpFormCreateTurnoverCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4um1qf1p0v3z",
			VipLevel: 1,
			Sign: 1,
			Amount: 100,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled",
			PlayerVipLevel: new PlayerVipLevelSingleResult(
				ResellerId: Guid.NewGuid(),
				ResellerCode: "Demo",
				LicenseeId: Guid.NewGuid(),
				LicenseeCode: "TGS",
				BrandId: Guid.NewGuid(),
				BrandCode: "GPI",
				PlayerCode: "test",
				Username: "test",
				VipLevelId: Guid.NewGuid(),
				VipLevel: 1,
				AutoState: PlayerVipLevelAutoState.Enable,
				AccumDepositId: Guid.NewGuid(),
				AccumDeposit: 998,
				AccumTurnoverId: Guid.NewGuid(),
				AccumTurnover: 1998,
				LevelUpAccumulateBetAmount: 999,
				LevelUpAccumulateDeposit: 1999,
				FullLevel: false,
				NextVipLevelId: Guid.NewGuid(),
				NextVipLevel: 2));

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new PlayerVipLevelExpFormCreateTurnoverCommandService(
			fakeSerialNumberGenerator,
			fakePlayerVipLevelExpFormRepository,
			fakeMessageSender);

		var actual = await sut.GetAsync(command);

		Assert.Equal(id, actual);

		_ = fakePlayerVipLevelExpFormRepository
			.Received(1)
			.CreateAsync(
			id: id,
			brandId: command.BrandId,
			playerCode: command.PlayerCode,
			vipLevel: command.VipLevel,
			type: PlayerVipLevelExpFormType.Turnover,
			autoState: true,
			sign: command.Sign,
			amount: command.Amount,
			snapshotExp: new PlayerVipLevelExpFormSnapshotExpInfo(
				PlayerCode: command.PlayerVipLevel.PlayerCode,
				AccumDepositId: command.PlayerVipLevel.AccumDepositId,
				AccumDeposit: command.PlayerVipLevel.AccumDeposit,
				AccumTurnoverId: command.PlayerVipLevel.AccumTurnoverId,
				AccumTurnover: command.PlayerVipLevel.AccumTurnover),
			remark: command.Remark,
			createdUserId: command.OperatorUserId,
			createdUsername: command.OperatorUsername,
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(VipLevelServiceNatsConsts.PlayerVipLevelExpFormAuto),
			data: Arg.Is(new VipLevelsExpFormMessage
			{
				FormId = id,
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process_Debit()
	{
		var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var command = new PlayerVipLevelExpFormCreateTurnoverCommand(
			BrandId: Guid.NewGuid(),
			PlayerCode: "4um1qf1p0v3z",
			VipLevel: 1,
			Sign: -1,
			Amount: 100,
			Remark: "test",
			OperatorUserId: Guid.NewGuid(),
			OperatorUsername: "admin.enabled",
			PlayerVipLevel: new PlayerVipLevelSingleResult(
				ResellerId: Guid.NewGuid(),
				ResellerCode: "Demo",
				LicenseeId: Guid.NewGuid(),
				LicenseeCode: "TGS",
				BrandId: Guid.NewGuid(),
				BrandCode: "GPI",
				PlayerCode: "test",
				Username: "test",
				VipLevelId: Guid.NewGuid(),
				VipLevel: 1,
				AutoState: PlayerVipLevelAutoState.Enable,
				AccumDepositId: Guid.NewGuid(),
				AccumDeposit: 998,
				AccumTurnoverId: Guid.NewGuid(),
				AccumTurnover: 1998,
				LevelUpAccumulateBetAmount: 999,
				LevelUpAccumulateDeposit: 1999,
				FullLevel: false,
				NextVipLevelId: Guid.NewGuid(),
				NextVipLevel: 2));

		var id = Guid.NewGuid();
		_ = fakeSerialNumberGenerator.GenerateAsync(
			Arg.Any<CancellationToken>())
			.Returns(id);

		var sut = new PlayerVipLevelExpFormCreateTurnoverCommandService(
			fakeSerialNumberGenerator,
			fakePlayerVipLevelExpFormRepository,
			fakeMessageSender);

		var actual = await sut.GetAsync(command);

		Assert.Equal(id, actual);

		_ = fakePlayerVipLevelExpFormRepository
			.Received(1)
			.CreateAsync(
			id: id,
			brandId: command.BrandId,
			playerCode: command.PlayerCode,
			vipLevel: command.VipLevel,
			type: PlayerVipLevelExpFormType.Turnover,
			autoState: true,
			sign: command.Sign,
			amount: command.Amount,
			snapshotExp: new PlayerVipLevelExpFormSnapshotExpInfo(
				PlayerCode: command.PlayerVipLevel.PlayerCode,
				AccumDepositId: command.PlayerVipLevel.AccumDepositId,
				AccumDeposit: command.PlayerVipLevel.AccumDeposit,
				AccumTurnoverId: command.PlayerVipLevel.AccumTurnoverId,
				AccumTurnover: command.PlayerVipLevel.AccumTurnover),
			remark: command.Remark,
			createdUserId: command.OperatorUserId,
			createdUsername: command.OperatorUsername,
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(VipLevelServiceNatsConsts.PlayerVipLevelExpFormAuto),
			data: Arg.Is(new VipLevelsExpFormMessage
			{
				FormId = id,
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
