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
public class PlayerVipLevelBirthdayFormAutoCommandServiceUnitTest
{
	[Fact]
	public async Task AutoState_Is_True()
	{
		var fakeLog = NullLogger<PlayerVipLevelBirthdayFormAutoCommandService>.Instance;
		var fakePlayerVipLevelBirthdayFormRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelBirthdayFormApprovedCommand>>();

		var command = new PlayerVipLevelBirthdayFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelBirthdayFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			Birthday: DateOnly.Parse("2024-02-06"),
			AutoState: true,
			State: Protos.PlayerVipLevelBirthdayFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelBirthdayFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelBirthdayFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelBirthdayFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelBirthdayFormRepository
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
		var fakeLog = NullLogger<PlayerVipLevelBirthdayFormAutoCommandService>.Instance;
		var fakePlayerVipLevelBirthdayFormRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelBirthdayFormApprovedCommand>>();

		var command = new PlayerVipLevelBirthdayFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelBirthdayFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			Birthday: DateOnly.Parse("2024-02-06"),
			AutoState: false,
			State: Protos.PlayerVipLevelBirthdayFormState.Create,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelBirthdayFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelBirthdayFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelBirthdayFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelBirthdayFormRepository
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
		var fakeLog = NullLogger<PlayerVipLevelBirthdayFormAutoCommandService>.Instance;
		var fakePlayerVipLevelBirthdayFormRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelBirthdayFormApprovedCommand>>();

		var command = new PlayerVipLevelBirthdayFormAutoCommand(
			Id: Guid.NewGuid());

		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";

		var form = new PlayerVipLevelBirthdayFormInfo(
			Id: command.Id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "4ucmqplxb029",
			VipLevel: 1,
			Birthday: DateOnly.Parse("2024-02-06"),
			AutoState: true,
			State: Protos.PlayerVipLevelBirthdayFormState.Approved,
			Amount: 99m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.Now,
			CreatedUserId: operatorUserId,
			CreatedUsername: operatorUsername,
			UpdatedOn: DateTimeOffset.Now,
			UpdatedUserId: operatorUserId,
			UpdatedUsername: operatorUsername);
		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);
		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(form);

		var sut = new PlayerVipLevelBirthdayFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelBirthdayFormRepository,
			fakeService);

		await sut.ExecuteAsync(command);

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelBirthdayFormApprovedCommand(
				Id: form.Id,
				Amount: form.Amount,
				Remark: form.Remark,
				OperatorUserId: form.CreatedUserId,
				OperatorUsername: form.CreatedUsername)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelBirthdayFormRepository
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
		var fakeLog = NullLogger<PlayerVipLevelBirthdayFormAutoCommandService>.Instance;
		var fakePlayerVipLevelBirthdayFormRepository = Substitute.For<IPlayerVipLevelBirthdayFormRepository>();
		var fakeService = Substitute.For<ICommandService<PlayerVipLevelBirthdayFormApprovedCommand>>();

		var command = new PlayerVipLevelBirthdayFormAutoCommand(
			Id: Guid.NewGuid());

		_ = fakePlayerVipLevelBirthdayFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelBirthdayFormInfo?)null);

		var sut = new PlayerVipLevelBirthdayFormAutoCommandService(
			fakeLog,
			fakePlayerVipLevelBirthdayFormRepository,
			fakeService);

		_ = await Assert.ThrowsAsync<NullReferenceException>(() => sut.ExecuteAsync(command).AsTask());

		_ = fakeService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Any<PlayerVipLevelBirthdayFormApprovedCommand>(),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelBirthdayFormRepository
			.Received(0)
			.PendingAsync(
			id: Arg.Any<Guid>(),
			updatedUserId: Arg.Any<Guid>(),
			updatedUsername: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
