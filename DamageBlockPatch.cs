using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace PreventDamageBlock;

[HarmonyPatch]
internal static class DamageBlockPatch
{
    [HarmonyPatch(typeof(Block), nameof(Block.OnBlockDamaged))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    internal static bool Block_OnBlockDamaged_Prefix(WorldBase _world, int _damagePoints, int _entityIdThatDamaged,
        ref int __result)
    {
        if (_damagePoints <= 0) return true;
        var entity = _world.GetEntity(_entityIdThatDamaged);
        if (entity == null) return true;
        if (Main.Debug)
        {
            var entityClass = entity.EntityClass;
            if (entity is EntityAlive alive)
            {
                var itemClass = alive.inventory.holdingItem;
                Log.Out(
                    $"[PreventDamageBlock] Damage, Entity={entityClass.classname}(Tags={string.Join(",", entity.EntityTags.GetTagNames())}), Item={itemClass.Name}(Tags={string.Join(",", itemClass.ItemTags.GetTagNames())})");
            }
            else
            {
                Log.Out(
                    $"[PreventDamageBlock] Damage, Entity={entityClass.classname}(Tags={string.Join(",", entityClass.Tags.GetTagNames())})");
            }
        }

        if (!Config.IsPreventDamageBlock(entity)) return true;
        __result = 0;
        return false;
    }

    [HarmonyPatch(typeof(NetPackageSetBlock), nameof(NetPackageSetBlock.ProcessPackage))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    internal static void NetPackageSetBlock_ProcessPackage_Prefix(
        World _world,
        GameManager _callbacks,
        NetPackageSetBlock __instance,
        List<BlockChangeInfo> ___blockChanges,
        int ___localPlayerThatChanged)
    {
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer) return;
        if (!_world.Players.dict.TryGetValue(___localPlayerThatChanged, out var player)) return;
        if (!Config.IsPreventDamageBlock(player)) return;

        foreach (var blockChangeInfo in ___blockChanges)
        {
            var oldBlockValue = _world.GetBlock(blockChangeInfo.pos);
            if (blockChangeInfo.blockValue.type != oldBlockValue.type ||
                blockChangeInfo.blockValue.damage > oldBlockValue.damage)
            {
                blockChangeInfo.blockValue = oldBlockValue;
            }
        }

        SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(player.entityId)
            .SendPackage(__instance);
    }
    
    [HarmonyPatch(typeof(Explosion), nameof(Explosion.AttackBlocks))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    internal static bool Explosion_AttackBlocks_Prefix(World ___world, int _entityThatCausedExplosion)
    {
        var entity = ___world.GetEntity(_entityThatCausedExplosion);
        if (entity == null) return true;
        if (Main.Debug)
        {
            var entityClass = entity.EntityClass;
            if (entity is EntityAlive alive)
            {
                var itemClass = alive.inventory.holdingItem;
                Log.Out(
                    $"[PreventDamageBlock] Explosion, Entity={entityClass.classname}(Tags={string.Join(",", entity.EntityTags.GetTagNames())}), Item={itemClass.Name}(Tags={string.Join(",", itemClass.ItemTags.GetTagNames())})");
            }
            else
            {
                Log.Out(
                    $"[PreventDamageBlock] Explosion, Entity={entityClass.classname}(Tags={string.Join(",", entityClass.Tags.GetTagNames())})");
            }
        }

        return !Config.IsPreventDamageBlock(entity);
    }
}