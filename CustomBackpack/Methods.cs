﻿using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace CustomBackpack
{
    public partial class ModEntry
    {

        public static int scrolled = 0;
        public static List<Item> tempInventory = new List<Item>();

        public enum Where
        {
            above,
            below,
            within,
            absent
        }

        private static bool IsWithinBounds(InventoryMenu instance, int x, int y)
        {
            int first = scrolled * 12;
            Rectangle rect = new Rectangle(instance.inventory[first].bounds.X, instance.inventory[first].bounds.Y, instance.inventory[first + instance.capacity - 1].bounds.X + instance.inventory[first + instance.capacity - 1].bounds.Width - instance.inventory[first].bounds.X, instance.inventory[first+instance.capacity - 1].bounds.Y + instance.inventory[first+instance.capacity - 1].bounds.Height - instance.inventory[first].bounds.Y);
            return rect.Contains(x, y);
        }

        private static Where GetGridPosition(InventoryMenu __instance, int x, int y)
        {
            int width = __instance.capacity / __instance.rows;

            for (int i = 0; i < __instance.inventory.Count; i++)
            {
                if (__instance.inventory[i] != null && __instance.inventory[i].containsPoint(x, y))
                {
                    if (i < scrolled * width)
                    {
                        return Where.above;
                    }
                    else if (i >= scrolled * width + __instance.capacity && __instance.actualInventory.Count >= __instance.capacity / __instance.rows * (scrolled + 1) + __instance.capacity)
                    {
                        return Where.below;
                    }
                    else
                    {
                        return Where.within;
                    }
                }
            }
            return Where.absent;
        }

        private static void SetPlayerSlots(int slots)
        {
            if (!Config.ModEnabled || Game1.player is null || slots < 1)
                return;
            if (slots % 12 != 0)
                slots = (slots / 12 + 1) * 12;
            SMonitor.Log($"Setting player backpack slots to {slots}");

            if(slots < Game1.player.MaxItems)
            {
                for (int i = Game1.player.Items.Count - 1; i >= slots; i--)
                {
                    if (Game1.player.Items.Count <= i)
                        break;
                    if (Game1.player.Items[i] is not null)
                    {
                        SMonitor.Log($"Slot {i} isn't empty, aborting");
                        return;
                    }
                }
                while (Game1.player.Items.Count > slots)
                {
                    Game1.player.Items.RemoveAt(Game1.player.Items.Count - 1);
                }
            }
            else
            {
                while (Game1.player.Items.Count < slots)
                {
                    Game1.player.Items.Add(null);
                }
            }
            Game1.player.maxItems.Value = slots;
        }

        private static void OnHover(ref InventoryMenu __instance, int x, int y)
        {
            if (__instance is FullInventoryMenu)
                return;
            if ((Game1.input.GetGamePadState().IsButtonDown(Buttons.DPadUp) && !Game1.oldPadState.IsButtonDown(Buttons.DPadUp)) || (Game1.input.GetKeyboardState().IsKeyDown(Keys.Up) && !Game1.oldKBState.IsKeyDown(Keys.Up)))
            {
                if (scrolled > 0)
                {
                    scrolled--;
                }
            }
            else if ((Game1.input.GetGamePadState().IsButtonDown(Buttons.DPadDown) && !Game1.oldPadState.IsButtonDown(Buttons.DPadDown)) || (Game1.input.GetKeyboardState().IsKeyDown(Keys.Down) && !Game1.oldKBState.IsKeyDown(Keys.Down)))
            {
                if (__instance.actualInventory.Count >= __instance.capacity / __instance.rows * (scrolled + 1) + __instance.capacity)
                {
                    scrolled++;
                }
            }
            else if (Game1.input.GetMouseState().ScrollWheelValue != Game1.oldMouseState.ScrollWheelValue)
            {
                var oldScrolled = scrolled;
                if (Game1.oldMouseState.ScrollWheelValue - Game1.input.GetMouseState().ScrollWheelValue > 0)
                {
                    if (__instance.actualInventory.Count >= __instance.capacity / __instance.rows * (scrolled + 1) + __instance.capacity)
                    {
                        scrolled++;
                    }
                }
                else
                {
                    if (scrolled > 0)
                    {
                        scrolled--;
                    }
                }
                if (scrolled != oldScrolled)
                {
                    Game1.playSound("shiny4");
                    int width = __instance.capacity / __instance.rows;
                    for (int i = 0; i < __instance.inventory.Count; i++)
                    {
                        if (i < scrolled * width || i >= scrolled * width + __instance.capacity)
                        {
                            __instance.inventory[i].bounds = new Microsoft.Xna.Framework.Rectangle();
                        }
                        else
                        {
                            int j = i - scrolled * width;
                            __instance.inventory[i].bounds = new Microsoft.Xna.Framework.Rectangle(__instance.xPositionOnScreen + i % (__instance.capacity / __instance.rows) * 64 + __instance.horizontalGap * (i % width), __instance.yPositionOnScreen + j / width * (64 + __instance.verticalGap) + (j / width - 1) * 4 - ((j > width || !__instance.playerInventory || __instance.verticalGap != 0) ? 0 : 12), 64, 64);
                        }
                        __instance.inventory[i].downNeighborID = (i >= __instance.capacity + width * (scrolled - 1)) ? 102 : 90000 + (i + width);
                        __instance.inventory[i].upNeighborID = (i < width * (scrolled + 1)) ? (12340 + i - width * scrolled) : 90000 + (i - width);
                    }
                }
            }
        }
    }
}