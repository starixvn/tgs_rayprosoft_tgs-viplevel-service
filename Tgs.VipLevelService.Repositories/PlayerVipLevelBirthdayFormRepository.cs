using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelBirthdayFormRepository(TgsContext context) : IPlayerVipLevelBirthdayFormRepository
{
	public async ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelBirthdayForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelBirthdayFormState.Approved;
		form.ActualAmount = amount;
		form.Remark = remark;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, DateOnly birthday, bool autoState, decimal amount, PlayerVipLevelBirthdaySnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		var form = new PlayerVipLevelBirthdayForm
		{
			Id = id,
			BrandId = brandId,
			PlayerCode = playerCode,
			VipLevel = vipLevel,
			Birthday = birthday,
			AutoState = autoState,
			State = (int)PlayerVipLevelBirthdayFormState.Create,
			Amount = amount,
			ActualAmount = 0,
			SnapshotVipLevel = JsonSerializer.Serialize(snapshotVipLevel),
			CreatedOn = DateTime.UtcNow,
			CreatedUserId = createdUserId,
			CreatedUsername = createdUsername,
			UpdatedUserId = createdUserId,
			UpdatedUsername = createdUsername,
			Remark = string.Empty
		};

		_ = await context.PlayerVipLevelBirthdayForms.AddAsync(
			form,
			cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(string playerCode, DateOnly birthday, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelBirthdayForms
		.Where(predicate: f => f.PlayerCode == playerCode && f.Birthday.Year == birthday.Year)
		.AnyAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<PlayerVipLevelBirthdayFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelBirthdayForms
		.Where(predicate: form => form.Id == id)
		.Select(selector: form => new PlayerVipLevelBirthdayFormInfo(
			form.Id,
			form.BrandId,
			form.PlayerCode,
			form.VipLevel,
			form.Birthday,
			form.AutoState,
			(PlayerVipLevelBirthdayFormState)form.State,
			form.Amount,
			form.ActualAmount,
			form.Remark,
			form.CreatedOn,
			form.CreatedUserId,
			form.CreatedUsername,
			form.UpdatedOn,
			form.UpdatedUserId,
			form.UpdatedUsername))
		.FirstOrDefaultAsync(
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask PendingAsync(Guid id, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelBirthdayForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelBirthdayFormState.Pending;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelBirthdayForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelBirthdayFormState.Reject;
		form.ActualAmount = form.Amount;
		form.Remark = remark;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
