using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelCycleFormRepository(
	TgsContext context) : IPlayerVipLevelCycleFormRepository
{
	public async ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelCycleForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelCycleFormState.Approved;
		form.ActualAmount = amount;
		form.Remark = remark;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, DateOnly fromOn, DateOnly toOn, bool autoState, decimal amount, PlayerVipLevelCycleFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		var form = new PlayerVipLevelCycleForm
		{
			Id = id,
			BrandId = brandId,
			PlayerCode = playerCode,
			VipLevel = vipLevel,
			CycleType = (int)cycleType,
			CycleOn = cycleOn,
			FromOn = fromOn,
			ToOn = toOn,
			AutoState = autoState,
			State = (int)PlayerVipLevelCycleFormState.Create,
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

		_ = await context.PlayerVipLevelCycleForms.AddAsync(
			form,
			cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelCycleForms
		.Where(predicate: f => f.PlayerCode == playerCode && f.CycleType == (int)cycleType && f.CycleOn == cycleOn)
		.AnyAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<PlayerVipLevelCycleFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelCycleForms
		.Where(predicate: form => form.Id == id)
		.Select(selector: form => new PlayerVipLevelCycleFormInfo(
			form.Id,
			form.BrandId,
			form.PlayerCode,
			form.VipLevel,
			(PlayerVipLevelCycleFormCycleType)form.CycleType,
			form.CycleOn,
			form.FromOn,
			form.ToOn,
			form.AutoState,
			(PlayerVipLevelCycleFormState)form.State,
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
		var form = await context.PlayerVipLevelCycleForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelCycleFormState.Pending;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)

	{
		var form = await context.PlayerVipLevelCycleForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelCycleFormState.Reject;
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
