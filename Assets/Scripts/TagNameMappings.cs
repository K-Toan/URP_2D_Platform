using System.Collections.Generic;

public enum TagName
{
    Prop,
    Enemy,
}

public static class Tags
{
    private static readonly Dictionary<TagName, string> TagNameMap = new Dictionary<TagName, string>
    {
        { TagName.Prop, "Prop" },
        { TagName.Enemy, "Enemy" },
    };

    public static string GetTag(TagName Tag)
    {
        return TagNameMap[Tag];
    }
}
