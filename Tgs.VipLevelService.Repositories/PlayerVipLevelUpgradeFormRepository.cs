using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelUpgradeFormRepository(
	TgsContext context) : IPlayerVipLevelUpgradeFormRepository
{
	public async ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelUpgradeForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelUpgradeFormState.Approved;
		form.ActualAmount = amount;
		form.Remark = remark;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, decimal amount, PlayerVipLevelUpgradeFormSnapshotVipLevelInfo snapshotVipLevel, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		var form = new PlayerVipLevelUpgradeForm
		{
			Id = id,
			BrandId = brandId,
			PlayerCode = playerCode,
			VipLevel = vipLevel,
			State = (int)PlayerVipLevelUpgradeFormState.Create,
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

		_ = await context.PlayerVipLevelUpgradeForms.AddAsync(
			form,
			cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelUpgradeForms
		.Where(predicate: f => f.PlayerCode == playerCode && f.VipLevel == vipLevel)
		.AnyAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<PlayerVipLevelUpgradeFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelUpgradeForms
		.Where(predicate: f => f.Id == id)
		.Select(selector: f => new PlayerVipLevelUpgradeFormInfo(
			f.Id,
			f.BrandId,
			f.PlayerCode,
			f.VipLevel,
			(PlayerVipLevelUpgradeFormState)f.State,
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
		var form = await context.PlayerVipLevelUpgradeForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelUpgradeFormState.Pending;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelUpgradeForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelUpgradeFormState.Reject;
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
