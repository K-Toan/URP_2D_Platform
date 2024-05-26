using System.Collections.Generic;

public enum TagName
{
    Prop,
    Player,
    Enemy,
    Trap,
}

public static class Tags
{
    private static readonly Dictionary<TagName, string> TagNameMap = new Dictionary<TagName, string>
    {
        { TagName.Prop, "Prop" },
        { TagName.Player, "Player" },
        { TagName.Enemy, "Enemy" },
        { TagName.Trap, "Trap" },
    };

    public static string GetTag(TagName Tag)
    {
        return TagNameMap[Tag];
    }
}
