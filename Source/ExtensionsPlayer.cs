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
    /// <summary> Hotbar slot to be selected during post-update. </summary>
    public static int SelectOnPost = -1;

    public override void ProcessTriggers(TriggersSet triggers)
    {
        if (Extensions.QuickStack .JustPressed)
        {
            if (ExtensionsConfig.Instance.ClearOnStack) ClearHotbar();
            QuickStack();
            if (ExtensionsConfig.Instance.SortOnStack) QuickSort();
        }
        if (Extensions.QuickSort  .JustPressed) QuickSort  ();
        if (Extensions.QuickHeal  .JustPressed) QuickHeal  ();
        if (Extensions.ClearHotbar.JustPressed) ClearHotbar();

        for (int i = 0; i < Extensions.QuickUse.Length; i++)
        {
            if (Extensions.QuickUse[i].JustPressed) QuickUse(i);
        }
    }

    public override void PostUpdate()
    {
        if (Player == Main.LocalPlayer && SelectOnPost != -1 && Player.itemTime == 0)
        {
            Player.selectedItem = SelectOnPost;
            SelectOnPost = -1;
        }
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

    /// <summary> Heals the player if there's a nurse nearby. </summary>
    public void QuickHeal()
    {
        int Modify(int count, NPC npc)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int type = Player.buffType[i];
                if (Main.debuff[type] && Player.buffTime[i] > 60 && (type < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[type]))
                    count += 100;
            }

                 if (NPC.downedGolemBoss  ) count *= 200;
            else if (NPC.downedPlantBoss  ) count *= 150;
            else if (NPC.downedMechBossAny) count *= 100;
            else if (Main.hardMode        ) count *=  60;
            else if (NPC.downedQueenBee   ) count *=  25;
            else if (NPC.downedBoss3      ) count *=  25;
            else if (NPC.downedBoss2      ) count *=  10;
            else if (NPC.downedBoss1      ) count *=   3;
                 if (Main.expertMode      ) count *=   2;

            return (int)(count * Main.ShopHelper.GetShoppingSettings(Player, npc).PriceAdjustment);
        }
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.type != NPCID.Nurse || !npc.Center.WithinRange(Player.Center, 8f * 16f)) continue;

            int count = Player.statLifeMax2 - Player.statLife,
                price = Modify(count, npc);

            if (price > 0 && Player.BuyItem(price))
            {
                Player.Heal(count);

                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int type = Player.buffType[i];
                    if (Main.debuff[type] && Player.buffTime[i] > 0 && (type < 0 || !BuffID.Sets.NurseCannotRemoveDebuff[type]))
                    {
                        Player.DelBuff(i);
                        i = -1; // the exact way the game shuffles buffs is unknown; therefore, the iterations is reset
                    }
                }

                SoundEngine.PlaySound(SoundID.Item4);
            }
        }
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

    /// <summary> Activates an item in the given hotbar slot. </summary>
    public void QuickUse(int i)
    {
        // save the selected hotbar slot
        SelectOnPost = Player.selectedItem;

        // make it ignore currently pressed buttons
        Player.itemTime = Player.itemAnimation = 0;

        Player.selectedItem = i;
        Player.controlUseItem = true;
        Player.releaseUseItem = true;
        Player.ItemCheck();
    }
}
