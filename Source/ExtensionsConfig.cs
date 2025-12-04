namespace Extensions;

using Terraria.ModLoader.Config;

/// <summary> List of options for fine-tuning. </summary>
public class ExtensionsConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    /// <summary> TML is responsible for this field assignment. </summary>
    public static ExtensionsConfig Instance;

    /// <summary> Whether the hotbar must be cleared before stacking. </summary>
    public bool ClearOnStack;
    /// <summary> Whether the chest must be sorted after stacking. </summary>
    public bool SortOnStack;
}
