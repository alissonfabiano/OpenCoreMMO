﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NeoServer.Data.Contexts;
using NeoServer.Data.Interfaces;
using NeoServer.Data.Model;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using Serilog;

namespace NeoServer.Data.Repositories;

public class AccountRepository : BaseRepository<AccountModel>, IAccountRepository
{
    #region constructors

    public AccountRepository(DbContextOptions<NeoContext> contextOptions, ILogger logger) : base(contextOptions,
        logger)
    {
    }

    #endregion

    #region public methods implementation

    public async Task<AccountModel> GetAccount(string name, string password)
    {
        await using var context = NewDbContext;

        return await context.Accounts
            .Where(x => x.Name.Equals(name) && x.Password.Equals(password))
            .Include(x => x.Players)
            .ThenInclude(x => x.World)
            .SingleOrDefaultAsync();
    }

    public async Task<PlayerModel> GetPlayer(string accountName, string password, string charName)
    {
        await using var context = NewDbContext;

        return await context.Players.Where(x => x.Account.Name.Equals(accountName) &&
                                                x.Account.Password.Equals(password) &&
                                                x.Name.Equals(charName))
            .Include(x => x.PlayerItems)
            .Include(x => x.PlayerInventoryItems)
            .Include(x => x.Account)
            .ThenInclude(x => x.VipList)
            .ThenInclude(x => x.Player)
            .Include(x => x.GuildMember)
            .ThenInclude(x => x.Guild).SingleOrDefaultAsync();
    }

    public async Task UpdatePlayerOnlineStatus(uint playerId, bool status)
    {
        await using var context = NewDbContext;

        var player = await context.Players.SingleOrDefaultAsync(x => x.PlayerId == playerId);
        if (player is null) return;

        player.Online = status;

        await context.SaveChangesAsync();
    }

    public async Task UpdatePlayer(IPlayer player)
    {
        var sql = @"UPDATE players
                           SET 
                               cap = @cap,
                               level = @level,
                               mana = @mana,
                               manamax = @manamax,
                               health = @health,
                               healthmax = @healthmax,
                               Soul = @Soul,
                               MaxSoul = @MaxSoul,
                               Speed = @Speed,
                               StaminaMinutes = @StaminaMinutes,
                               -- Online = @Online,
                               lookaddons = @lookaddons,
                               lookbody = @lookbody,
                               lookfeet = @lookfeet,
                               lookhead = @lookhead,
                               looklegs = @looklegs,
                               looktype = @looktype,
                               posx = @posx,
                               posy = @posy,
                               posz = @posz,
                               -- offlinetraining_time = @offlinetraining_time,
                               -- offlinetraining_skill = @offlinetraining_skill,
                               skill_fist = @skill_fist,
                               skill_fist_tries = @skill_fist_tries,
                               skill_club = @skill_club,
                               skill_club_tries = @skill_club_tries,
                               skill_sword = @skill_sword,
                               skill_sword_tries = @skill_sword_tries,
                               skill_axe = @skill_axe,
                               skill_axe_tries = @skill_axe_tries,
                               skill_dist = @skill_dist,
                               skill_dist_tries = @skill_dist_tries,
                               skill_shielding = @skill_shielding,
                               skill_shielding_tries = @skill_shielding_tries,
                               skill_fishing = @skill_fishing,
                               skill_fishing_tries = @skill_fishing_tries,
                               MagicLevel = @MagicLevel,
                               MagicLevelTries = @MagicLevelTries,
                               Experience = @Experience,
                               ChaseMode = @ChaseMode,
                               FightMode = @FightMode,
                               vocation = @vocation,
                               remaining_recovery_seconds = @remaining_recovery_seconds
                         WHERE id = @playerId";

        await using var context = NewDbContext;

        if (!context.Database.IsRelational()) return;

        await using var connection = context.Database.GetDbConnection();

        await connection.ExecuteAsync(sql, new
        {
            cap = player.TotalCapacity,
            level = player.Level,
            mana = player.Mana,
            manamax = player.MaxMana,
            health = player.HealthPoints,
            healthmax = player.MaxHealthPoints,
            Soul = player.SoulPoints,
            MaxSoul = player.MaxSoulPoints,
            player.Speed,
            player.StaminaMinutes,

            lookaddons = player.Outfit.Addon,
            lookbody = player.Outfit.Body,
            lookfeet = player.Outfit.Feet,
            lookhead = player.Outfit.Head,
            looklegs = player.Outfit.Legs,
            looktype = player.Outfit.LookType,
            posx = player.Location.X,
            posy = player.Location.Y,
            posz = player.Location.Z,

            skill_fist = player.GetSkillLevel(SkillType.Fist),
            skill_fist_tries = player.GetSkillTries(SkillType.Fist),
            skill_club = player.GetSkillLevel(SkillType.Club),
            skill_club_tries = player.GetSkillTries(SkillType.Club),
            skill_sword = player.GetSkillLevel(SkillType.Sword),
            skill_sword_tries = player.GetSkillTries(SkillType.Sword),
            skill_axe = player.GetSkillLevel(SkillType.Axe),
            skill_axe_tries = player.GetSkillTries(SkillType.Axe),
            skill_dist = player.GetSkillLevel(SkillType.Distance),
            skill_dist_tries = player.GetSkillTries(SkillType.Distance),
            skill_shielding = player.GetSkillLevel(SkillType.Shielding),
            skill_shielding_tries = player.GetSkillTries(SkillType.Shielding),
            skill_fishing = player.GetSkillLevel(SkillType.Fishing),
            skill_fishing_tries = player.GetSkillTries(SkillType.Fishing),
            MagicLevel = player.GetSkillLevel(SkillType.Magic),
            MagicLevelTries = player.GetSkillTries(SkillType.Magic),
            player.Experience,
            player.ChaseMode,
            player.FightMode,
            remaining_recovery_seconds =
                player.Conditions.TryGetValue(ConditionType.Regeneration, out var condition)
                    ? condition.RemainingTime / TimeSpan.TicksPerMillisecond
                    : 0,
            vocation = player.VocationType,
            playerId = player.Id
        }, commandTimeout: 5);
    }


    public async Task UpdatePlayers(IEnumerable<IPlayer> players)
    {
        var tasks = new List<Task>();

        foreach (var player in players)
        {
            tasks.Add(UpdatePlayer(player));
            if (UpdatePlayerInventory(player) is { } updates) tasks.AddRange(updates);
        }

        await Task.WhenAll(tasks);
    }

    public async Task<PlayerModel> GetPlayer(string playerName)
    {
        await using var context = NewDbContext;
        return await context.Players.FirstOrDefaultAsync(x => x.Name.Equals(playerName));
    }

    public async Task SavePlayerInventory(IPlayer player)
    {
        await AddMissingInventoryRecords(player);

        if (UpdatePlayerInventory(player) is { } updates) await Task.WhenAll(updates);
    }

    public async Task<int> Ban(uint playerAccountId, string reason, uint bannedByAccountId)
    {
        await using var context = NewDbContext;

        return await context.Accounts
            .Where(x => x.AccountId == playerAccountId)
            .ExecuteUpdateAsync(x
                => x.SetProperty(y => y.BannedBy, bannedByAccountId)
                    .SetProperty(y => y.BanishmentReason, reason)
                    .SetProperty(y => y.BanishedAt, DateTime.Now));
    }

    private async Task AddMissingInventoryRecords(IPlayer player)
    {
        await using var context = NewDbContext;

        var inventoryRecords = context.PlayerInventoryItems
            .Where(x => x.PlayerId == player.Id)
            .Select(x => x.SlotId)
            .ToHashSet();

        for (var i = 1; i <= 10; i++)
        {
            if (inventoryRecords.Contains(i)) continue;

            await context.PlayerInventoryItems.AddAsync(new PlayerInventoryItemModel
            {
                Amount = 0, PlayerId = (int)player.Id, SlotId = i, ServerId = 0
            });
        }

        await context.SaveChangesAsync();
    }

    private List<Task> UpdatePlayerInventory(IPlayer player)
    {
        if (player is null) return null;

        var sql = @"UPDATE player_inventory_items
                           SET sid = @sid,
                               count = @count
                         WHERE player_id = @playerId and slot_id = @pid";

        var tasks = new List<Task>();

        foreach (var slot in new[]
                 {
                     Slot.Necklace, Slot.Head, Slot.Backpack, Slot.Left, Slot.Body, Slot.Right, Slot.Ring, Slot.Legs,
                     Slot.Ammo, Slot.Feet
                 })
            tasks.Add(Task.Run(async () =>
            {
                await using var context = NewDbContext;
                if (!context.Database.IsRelational()) return;

                await using var connection = context.Database.GetDbConnection();

                var item = player.Inventory[slot];

                await connection.ExecuteAsync(sql, new
                {
                    sid = item?.Metadata?.TypeId ?? 0,
                    count = item?.Amount ?? 0,
                    playerId = player.Id,
                    pid = (int)slot
                }, commandTimeout: 5);
            }));

        return tasks;
    }

    public async Task AddPlayerToVipList(int accountId, int playerId)
    {
        await using var context = NewDbContext;

        await context.AccountsVipList.AddAsync(new AccountVipListModel
        {
            AccountId = accountId,
            PlayerId = playerId
        });

        await CommitChanges(context);
    }

    public async Task<PlayerModel> GetOnlinePlayer(string accountName)
    {
        await using var context = NewDbContext;

        return await context.Players
            .Include(x => x.Account)
            .Where(x => x.Account.Name.Equals(accountName) && x.Online)
            .FirstOrDefaultAsync();
    }

    public async Task RemoveFromVipList(int accountId, int playerId)
    {
        await using var context = NewDbContext;

        var item = await context.AccountsVipList.SingleOrDefaultAsync(x =>
            x.PlayerId == playerId && x.AccountId == accountId);

        if (item is null) return;

        context.AccountsVipList.Remove(item);
        await CommitChanges(context);
    }

    #endregion
}