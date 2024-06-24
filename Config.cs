using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PreventDamageBlock;

public static class Config
{
    public static HashSet<string> PreventDamageBlockByItem { get; private set; }

    public static FastTags<TagGroup.Global> PreventDamageBlockByItemTag { get; private set; }

    public static HashSet<string> PreventDamageBlockByEntity { get; private set; }

    public static FastTags<TagGroup.Global> PreventDamageBlockByEntityTag { get; private set; }

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

    private static FastTags<TagGroup.Global> ToFastTags(this JToken token)
    {
        var tags = token.ToObject<string[]>();
        return tags.Length switch
        {
            0 => FastTags<TagGroup.Global>.none,
            1 => FastTags<TagGroup.Global>.GetTag(tags[0]),
            2 => FastTags<TagGroup.Global>.CombineTags(FastTags<TagGroup.Global>.GetTag(tags[0]), FastTags<TagGroup.Global>.GetTag(tags[1])),
            3 => FastTags<TagGroup.Global>.CombineTags(FastTags<TagGroup.Global>.GetTag(tags[0]), FastTags<TagGroup.Global>.GetTag(tags[1]), 
                FastTags<TagGroup.Global>.GetTag(tags[2])),
            4 => FastTags<TagGroup.Global>.CombineTags(FastTags<TagGroup.Global>.GetTag(tags[0]), FastTags<TagGroup.Global>.GetTag(tags[1]), 
                FastTags<TagGroup.Global>.GetTag(tags[2]), FastTags<TagGroup.Global>.GetTag(tags[3])),
            _ => tags.Aggregate(FastTags<TagGroup.Global>.none, (current, tag) => current | FastTags<TagGroup.Global>.GetTag(tag))
        };
    }
}