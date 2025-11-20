namespace Extensions;

using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

/// <summary> Essentially only initializes all keybindings. </summary>
public class Extensions : Mod
{
    /// <summary> Keybindings themselves used by the mod player. </summary>
    public static ModKeybind QuickStack, QuickSort, ClearHotbar;

    public override void Load()
    {
        QuickStack  = KeybindLoader.RegisterKeybind(this, "quick-stack",  Keys.Q);
        QuickSort   = KeybindLoader.RegisterKeybind(this, "quick-sort",   Keys.T);
        ClearHotbar = KeybindLoader.RegisterKeybind(this, "clear-hotbar", Keys.F);
    }
}
