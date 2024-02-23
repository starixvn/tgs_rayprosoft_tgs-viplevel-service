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

	public IAsyncEnumerable<PlayerVipLevelCycleFormQueryInfo> QueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelCycleFormState> states, int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
	{
		var filters = context.PlayerVipLevelCycleForms
			.Join(
			inner: context.Players,
			outerKeySelector: form => form.PlayerCode,
			innerKeySelector: player => player.PlayerCode,
			resultSelector: (form, player) => new { Form = form, Player = player })
			.Where(predicate: combined => combined.Form.UpdatedOn >= updatedOnStart.UtcDateTime && combined.Form.UpdatedOn < updatedOnEnd.UtcDateTime)
			.Where(predicate: combined => combined.Form.BrandId == brandId);

		if (!string.IsNullOrWhiteSpace(username))
		{
			filters = filters.Where(predicate: combined => combined.Player.Username.StartsWith(username) || combined.Player.Username == username);
		}

		if (!string.IsNullOrWhiteSpace(playerCode))
		{
			filters = filters.Where(predicate: combined => combined.Form.PlayerCode == playerCode);
		}

		if (states.Any())
		{
			filters = filters.Where(predicate: combined => states.Contains((PlayerVipLevelCycleFormState)combined.Form.State));
		}

		return filters
		 .OrderByDescending(keySelector: combined => combined.Form.UpdatedOn)
		 .Skip(limit * offset)
		 .Take(limit)
		 .Select(selector: combined => new PlayerVipLevelCycleFormQueryInfo(
			 combined.Form.Id,
			 combined.Form.BrandId,
			 combined.Form.PlayerCode,
			 combined.Player.Username,
			 combined.Form.VipLevel,
			 (PlayerVipLevelCycleFormCycleType)combined.Form.CycleType,
			 combined.Form.CycleOn,
			 combined.Form.FromOn,
			 combined.Form.ToOn,
			 combined.Form.AutoState,
			 (PlayerVipLevelCycleFormState)combined.Form.State,
			 combined.Form.Amount,
			 combined.Form.ActualAmount,
			 combined.Form.Remark,
			 combined.Form.CreatedOn,
			 combined.Form.CreatedUserId,
			 combined.Form.CreatedUsername,
			 combined.Form.UpdatedOn,
			 combined.Form.UpdatedUserId,
			 combined.Form.UpdatedUsername))
		 .ToAsyncEnumerable();
	}

	public async ValueTask<int> QueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelCycleFormState> states, CancellationToken cancellationToken = default)
	{
		var filters = context.PlayerVipLevelCycleForms
			.Join(
			inner: context.Players,
			outerKeySelector: form => form.PlayerCode,
			innerKeySelector: player => player.PlayerCode,
			resultSelector: (form, player) => new { Form = form, Player = player })
			.Where(predicate: combined => combined.Form.UpdatedOn >= updatedOnStart.UtcDateTime && combined.Form.UpdatedOn < updatedOnEnd.UtcDateTime)
			.Where(predicate: combined => combined.Form.BrandId == brandId);

		if (!string.IsNullOrWhiteSpace(username))
		{
			filters = filters.Where(predicate: combined => combined.Player.Username.StartsWith(username) || combined.Player.Username == username);
		}

		if (!string.IsNullOrWhiteSpace(playerCode))
		{
			filters = filters.Where(predicate: combined => combined.Form.PlayerCode == playerCode);
		}

		if (states.Any())
		{
			filters = filters.Where(predicate: combined => states.Contains((PlayerVipLevelCycleFormState)combined.Form.State));
		}

		return await filters.CountAsync(
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
