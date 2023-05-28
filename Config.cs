using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PreventDamageBlock;

public static class Config
{
    public static HashSet<string> PreventDamageBlockByItem { get; private set; }

    public static FastTags PreventDamageBlockByItemTag { get; private set; }

    public static HashSet<string> PreventDamageBlockByEntity { get; private set; }

    public static FastTags PreventDamageBlockByEntityTag { get; private set; }

    public static bool IsPreventDamageBlock(Entity entity)
    {
        var entityClass = entity.EntityClass;
        if (PreventDamageBlockByEntity.Contains(entityClass.entityClassName) ||
            PreventDamageBlockByEntityTag.Test_AnySet(entityClass.Tags)) return true;

        if (entity is not EntityAlive alive) return false;

        var itemClass = alive.inventory.holdingItem;
        if (PreventDamageBlockByItem.Contains(itemClass.Name) ||
            PreventDamageBlockByItemTag.Test_AnySet(itemClass.ItemTags)) return true;

        return false;
    }

    public static void Load()
    {
        Log.Out("[PreventDamageBlock] Loading config");

        var configFile = Main.Instance.Path + "/Config.json";
        var root = JObject.Parse((File.Exists(configFile) ? File.ReadAllText(configFile) : null) ?? "{}");

        PreventDamageBlockByItem = root.GetValue("PreventDamageBlockByItem").ToObject<HashSet<string>>();
        PreventDamageBlockByItemTag = root.GetValue("PreventDamageBlockByItemTag").ToFastTags();
        PreventDamageBlockByEntity = root.GetValue("PreventDamageBlockByEntity").ToObject<HashSet<string>>();
        PreventDamageBlockByEntityTag = root.GetValue("PreventDamageBlockByEntityTag").ToFastTags();

        Log.Out("[PreventDamageBlock] Loaded config");
    }

    private static FastTags ToFastTags(this JToken token)
    {
        var tags = token.ToObject<string[]>();
        return tags.Length switch
        {
            0 => FastTags.none,
            1 => FastTags.GetTag(tags[0]),
            2 => FastTags.CombineTags(FastTags.GetTag(tags[0]), FastTags.GetTag(tags[1])),
            3 => FastTags.CombineTags(FastTags.GetTag(tags[0]), FastTags.GetTag(tags[1]), FastTags.GetTag(tags[2])),
            4 => FastTags.CombineTags(FastTags.GetTag(tags[0]), FastTags.GetTag(tags[1]), FastTags.GetTag(tags[2]),
                FastTags.GetTag(tags[3])),
            _ => tags.Aggregate(FastTags.none, (current, tag) => current | FastTags.GetTag(tag))
        };
    }
}