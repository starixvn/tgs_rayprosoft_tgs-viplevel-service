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
		form.UpdatedUsername = updatedUsername.Trim();

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, DateOnly birthday, decimal amount, PlayerVipLevelBirthdaySnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		var form = new PlayerVipLevelBirthdayForm
		{
			Id = id,
			BrandId = brandId,
			PlayerCode = playerCode,
			VipLevel = vipLevel,
			Birthday = birthday,
			State = (int)PlayerVipLevelBirthdayFormState.Create,
			Amount = amount,
			ActualAmount = 0,
			SnapshotVipLevel = JsonSerializer.Serialize(snapshotVipLevel),
			CreatedOn = DateTime.UtcNow,
			CreatedUserId = createdUserId,
			CreatedUsername = createdUsername.Trim(),
			UpdatedUserId = createdUserId,
			UpdatedUsername = createdUsername.Trim(),
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
		.Where(predicate: f => f.Id == id)
		.Select(selector: f => new PlayerVipLevelBirthdayFormInfo(
			f.Id,
			f.BrandId,
			f.PlayerCode,
			f.VipLevel,
			f.Birthday,
			(PlayerVipLevelBirthdayFormState)f.State,
			f.Amount,
			f.ActualAmount,
			f.Remark,
			f.CreatedOn,
			f.CreatedUserId,
			f.CreatedUsername,
			f.UpdatedOn,
			f.UpdatedUserId,
			f.UpdatedUsername))
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
		form.UpdatedUsername = updatedUsername.Trim();

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
		form.UpdatedUsername = updatedUsername.Trim();

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
