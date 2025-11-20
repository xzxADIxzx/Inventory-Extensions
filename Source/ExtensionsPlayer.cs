namespace Extensions;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

/// <summary> Sort of a patch that extends vanilla functionality. </summary>
public class ExtensionsPlayer : ModPlayer
{
    public override void ProcessTriggers(TriggersSet triggers)
    {
        if (Extensions.QuickStack .JustPressed) QuickStack ();
        if (Extensions.QuickSort  .JustPressed) QuickSort  ();
        if (Extensions.ClearHotbar.JustPressed) ClearHotbar();
    }

    /// <summary> Simply calls vanilla quick stack. </summary>
    public void QuickStack()
    {
        if (Player.chest != -1) ChestUI.QuickStack(ContainerTransferContext.FromUnknown(Player));
    }

    /// <summary> Simply calls vanilla sort items. </summary>
    public void QuickSort()
    {
        if (Player.chest != -1) ItemSorting.SortChest();
    }

    /// <summary> Moves all non-favorited items out of hotbar. </summary>
    public void ClearHotbar()
    {
        void ClearSlot(int i)
        {
            var item = Player.inventory[i];
            if (item.stack == 0 || item.favorited) return;

            for (int j = 49; j >= 10; j--)
            {
                var dest = Player.inventory[j];
                if (dest.type == item.type)
                {
                    ItemLoader.TryStackItems(dest, item, out int itemsMoved);
                    if (itemsMoved != 0) SoundEngine.PlaySound(SoundID.Grab);
                    if (item.stack == 0) return;
                }
            }
            for (int j = 49; j >= 10; j--)
            {
                var dest = Player.inventory[j];
                if (dest.stack == 0)
                {
                    Player.inventory[j] = item;
                    Player.inventory[i] = dest;
                    SoundEngine.PlaySound(SoundID.Grab);
                    return;
                }
            }
        }
        for (int i = 0; i < 10; i++) ClearSlot(i);
    }
}
