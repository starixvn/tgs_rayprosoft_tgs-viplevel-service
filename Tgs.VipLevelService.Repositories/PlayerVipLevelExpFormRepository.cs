using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Entities;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelExpFormRepository(
	TgsContext context) : IPlayerVipLevelExpFormRepository
{
	public async ValueTask ApprovedAsync(Guid id, decimal amount, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelExpForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelExpFormState.Approved;
		form.ActualAmount = amount;
		form.Remark = remark;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask CreateAsync(Guid id, Guid brandId, string playerCode, int vipLevel, PlayerVipLevelExpFormType type, bool autoState, int sign, decimal amount, PlayerVipLevelExpFormSnapshotExpInfo snapshotExp, string remark, Guid createdUserId, string createdUsername, CancellationToken cancellationToken = default)
	{
		var form = new PlayerVipLevelExpForm
		{
			Id = id,
			BrandId = brandId,
			PlayerCode = playerCode,
			VipLevel = vipLevel,
			Type = (int)type,
			AutoState = autoState,
			State = (int)PlayerVipLevelCycleFormState.Create,
			Sign = sign,
			Amount = amount,
			ActualAmount = 0,
			SnapshotExp = JsonSerializer.Serialize(snapshotExp),
			CreatedOn = DateTime.UtcNow,
			CreatedUserId = createdUserId,
			CreatedUsername = createdUsername,
			UpdatedUserId = createdUserId,
			UpdatedUsername = createdUsername,
			Remark = remark
		};

		_ = await context.PlayerVipLevelExpForms.AddAsync(
			entity: form,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<PlayerVipLevelExpFormInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await context.PlayerVipLevelExpForms
		.Where(predicate: f => f.Id == id)
		.Select(selector: f => new PlayerVipLevelExpFormInfo(
			f.Id,
			f.BrandId,
			f.PlayerCode,
			f.VipLevel,
			(PlayerVipLevelExpFormType)f.Type,
			f.AutoState,
			(PlayerVipLevelExpFormState)f.State,
			f.Sign,
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
		var form = await context.PlayerVipLevelExpForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelExpFormState.Pending;
		form.UpdatedOn = DateTime.UtcNow;
		form.UpdatedUserId = updatedUserId;
		form.UpdatedUsername = updatedUsername;

		await context.SaveChangesAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask RejectAsync(Guid id, string remark, Guid updatedUserId, string updatedUsername, CancellationToken cancellationToken = default)
	{
		var form = await context.PlayerVipLevelExpForms
			.Where(predicate: f => f.Id == id)
			.SingleAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		form.State = (int)PlayerVipLevelExpFormState.Reject;
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
