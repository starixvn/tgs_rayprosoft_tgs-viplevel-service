using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelUpgradeFormAutoCommandServiceUnitTest
{
	[Fact]
	public async Task AutoState_Is_True()
	{
		var fakeLog = NullLogger<PlayerVipLevelUpgradeFormAutoCommandService>.Instance;
		var fakePlayerVipLevelUpgradeFormRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelUpgradeFormApprovedCommand>>();

		var command = new PlayerVipLevelUpgradeFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelUpgradeFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			AutoState: true,
			State: Protos.PlayerVipLevelUpgradeFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelUpgradeFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelUpgradeFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelUpgradeFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelUpgradeFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelUpgradeFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Is(form.Id),
			updatedUserId: Arg.Is(form.CreatedUserId),
			updatedUsername: Arg.Is(form.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task AutoState_Is_False()
	{
		var fakeLog = NullLogger<PlayerVipLevelUpgradeFormAutoCommandService>.Instance;
		var fakePlayerVipLevelUpgradeFormRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelUpgradeFormApprovedCommand>>();

		var command = new PlayerVipLevelUpgradeFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelUpgradeFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			AutoState: false,
			State: Protos.PlayerVipLevelUpgradeFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelUpgradeFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelUpgradeFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelUpgradeFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelUpgradeFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelUpgradeFormRepository
			.Received(1)
			.PendingAsync(
			id: Arg.Is(form.Id),
			updatedUserId: Arg.Is(form.CreatedUserId),
			updatedUsername: Arg.Is(form.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_status_is_not_created()
	{
		var fakeLog = NullLogger<PlayerVipLevelUpgradeFormAutoCommandService>.Instance;
		var fakePlayerVipLevelUpgradeFormRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelUpgradeFormApprovedCommand>>();

		var command = new PlayerVipLevelUpgradeFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelUpgradeFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			AutoState: true,
			State: Protos.PlayerVipLevelUpgradeFormState.Approved,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelUpgradeFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);
		_ = fakePlayerVipLevelUpgradeFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelUpgradeFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelUpgradeFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelUpgradeFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelUpgradeFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Is(form.Id),
			updatedUserId: Arg.Is(form.CreatedUserId),
			updatedUsername: Arg.Is(form.CreatedUsername),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_form_cannot_be_found()
	{
		var fakeLog = NullLogger<PlayerVipLevelUpgradeFormAutoCommandService>.Instance;
		var fakePlayerVipLevelUpgradeFormRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelUpgradeFormApprovedCommand>>();

		var command = new PlayerVipLevelUpgradeFormAutoCommand(
			Id: Guid.NewGuid());

		_ = fakePlayerVipLevelUpgradeFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelUpgradeFormInfo?)null);

		var sut = new PlayerVipLevelUpgradeFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelUpgradeFormRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<NullReferenceException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<PlayerVipLevelUpgradeFormApprovedCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelUpgradeFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Any<Guid>(),
			updatedUserId: Arg.Any<Guid>(),
			updatedUsername: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
