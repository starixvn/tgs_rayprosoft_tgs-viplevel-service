using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.VipLevels.Exp.Worker.Common;
using Tgs.VipLevels.Exp.Worker.Common.Protos;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelExpFormApprovedCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var command = new PlayerVipLevelExpFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 99m,
			Remark: "test",
			OperatorUserId: operatorUserId,
			OperatorUsername: operatorUsername);

		var form = new PlayerVipLevelExpFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4um1qf1p0v3z",
			VipLevel: 1,
			Type: Protos.PlayerVipLevelExpFormType.Turnover,
			AutoState: true,
			State: Protos.PlayerVipLevelExpFormState.Pending,
			Sign: 1,
			Amount: 1,
			ActualAmount: 1,
			Remark: "test",
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelExpFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelExpFormRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakePlayerVipLevelExpFormRepository
			.Received(1)
			.ApprovedAsync(
			id: Arg.Is(form.Id),
			amount: Arg.Is(command.Amount),
			remark: Arg.Is(command.Remark),
			updatedUserId: Arg.Is(command.OperatorUserId),
			updatedUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(1)
			.PublishAsync(
			subject: Arg.Is(VipLevelsExpWorkerNatsConst.ExpFormId),
			data: Arg.Is(new VipLevelsExpFormMessage
			{
				FormId = form.Id,
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_has_been_reviewed()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var command = new PlayerVipLevelExpFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 99m,
			Remark: "test",
			OperatorUserId: operatorUserId,
			OperatorUsername: operatorUsername);

		var form = new PlayerVipLevelExpFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4um1qf1p0v3z",
			VipLevel: 1,
			Type: Protos.PlayerVipLevelExpFormType.Turnover,
			AutoState: true,
			State: Protos.PlayerVipLevelExpFormState.Approved,
			Sign: 1,
			Amount: 1,
			ActualAmount: 1,
			Remark: "test",
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelExpFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelExpFormRepository,
			fakeMessageSender);

		await sut.ExecuteAsync(command);

		_ = fakePlayerVipLevelExpFormRepository
			.Received(0)
			.ApprovedAsync(
			id: Arg.Is(form.Id),
			amount: Arg.Is(command.Amount),
			remark: Arg.Is(command.Remark),
			updatedUserId: Arg.Is(command.OperatorUserId),
			updatedUsername: Arg.Is(command.OperatorUsername),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(VipLevelsExpWorkerNatsConst.ExpFormId),
			data: Arg.Is(new VipLevelsExpFormMessage
			{
				FormId = form.Id,
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_cannot_be_found()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFormApprovedCommandService>.Instance;
		var fakePlayerVipLevelExpFormRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var command = new PlayerVipLevelExpFormApprovedCommand(
			Id: Guid.NewGuid(),
			Amount: 99m,
			Remark: "test",
			OperatorUserId: operatorUserId,
			OperatorUsername: operatorUsername);

		_ = fakePlayerVipLevelExpFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelExpFormInfo?)null);

		var sut = new PlayerVipLevelExpFormApprovedCommandService(
			fakeLog,
			fakePlayerVipLevelExpFormRepository,
			fakeMessageSender);

		_ = await Assert.ThrowsAsync<NullReferenceException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakePlayerVipLevelExpFormRepository
			.Received(0)
			.ApprovedAsync(
			id: Arg.Any<Guid>(),
			amount: Arg.Any<decimal>(),
			remark: Arg.Any<string>(),
			updatedUserId: Arg.Any<Guid>(),
			updatedUsername: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakeMessageSender
			.Received(0)
			.PublishAsync(
			subject: Arg.Is(VipLevelsExpWorkerNatsConst.ExpFormId),
			data: Arg.Any<VipLevelsExpFormMessage>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
