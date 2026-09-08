using System;
using System.Collections.Generic;

namespace InventoryMaster.Helpers
{
    #region [START] ITEM CATEGORY HELPER
    // ============================================================================
    // [START] ITEM CATEGORY HELPER
    // Description: Classifies Raft items into logical sort categories with priority weights.
    // ============================================================================
    public enum ItemCategory
    {
        ToolsAndWeapons = 0,
        EquipmentAndArmor = 1,
        PotableDrink = 2,
        CookedFood = 3,
        Medicine = 4,
        RawFood = 5,
        SeedsAndFarming = 6,
        PrimaryResources = 7,
        RefinedMetals = 8,
        CraftingComponents = 9,
        DecorAndFurniture = 10,
        QuestAndValuables = 11,
        Miscellaneous = 12
    }

    public static class ItemCategoryHelper
    {
        public static ItemCategory GetCategory(Item_Base item)
        {
            if (item == null) return ItemCategory.Miscellaneous;

            string name = item.UniqueName?.ToLowerInvariant() ?? "";

            // 1. Tools and Weapons
            if (name.Contains("hook") || name.Contains("axe") || name.Contains("spear") ||
                name.Contains("bow") || name.Contains("arrow") || name.Contains("hammer") ||
                name.Contains("shovel") || name.Contains("shears") || name.Contains("machete") ||
                name.Contains("netgun") || name.Contains("net_canister") || name.Contains("fishingrod") ||
                name.Contains("paddle") || name.Contains("binoculars") || name.Contains("paintbrush"))
            {
                return ItemCategory.ToolsAndWeapons;
            }

            // 2. Equipment and Armor
            if (name.Contains("flippers") || name.Contains("oxygen") || name.Contains("headlight") ||
                name.Contains("backpack") || name.Contains("armor") || name.Contains("helmet") ||
                name.Contains("hazmat") || name.Contains("suit"))
            {
                return ItemCategory.EquipmentAndArmor;
            }

            // 3. Drinks & Hydration
            if (name.Contains("water") || name.Contains("drinking") || name.Contains("cup") ||
                name.Contains("bottle") || name.Contains("smoothie"))
            {
                return ItemCategory.PotableDrink;
            }

            // 4. Healing
            if (name.Contains("heal") || name.Contains("salve") || name.Contains("bandage"))
            {
                return ItemCategory.Medicine;
            }

            // 5. Cooked Food vs Raw Food
            if (name.Contains("cooked") || name.Contains("stew") || name.Contains("soup") ||
                name.Contains("pie") || name.Contains("roast") || name.Contains("barbecue"))
            {
                return ItemCategory.CookedFood;
            }
            if (name.Contains("raw") || name.Contains("beet") || name.Contains("potato") ||
                name.Contains("fish") || name.Contains("mango") || name.Contains("coconut") ||
                name.Contains("mushroom") || name.Contains("berry") || name.Contains("egg"))
            {
                return ItemCategory.RawFood;
            }

            // 6. Seeds & Farming
            if (name.Contains("seed") || name.Contains("flower") || name.Contains("sapling") ||
                name.Contains("spore"))
            {
                return ItemCategory.SeedsAndFarming;
            }

            // 7. Refined Metals & Glass
            if (name.Contains("ingot") || name.Contains("glass") || name.Contains("metal") ||
                name.Contains("copper") || name.Contains("titanium"))
            {
                return ItemCategory.RefinedMetals;
            }

            // 8. Crafting Components
            if (name.Contains("bolt") || name.Contains("hinge") || name.Contains("circuit") ||
                name.Contains("battery") || name.Contains("rope") || name.Contains("nail") ||
                name.Contains("glue") || name.Contains("wool") || name.Contains("leather") ||
                name.Contains("brick_wet") || name.Contains("brick_dry") || name.Contains("pipe"))
            {
                return ItemCategory.CraftingComponents;
            }

            // 9. Primary Raw Resources
            if (name.Contains("plank") || name.Contains("plastic") || name.Contains("thatch") ||
                name.Contains("scrap") || name.Contains("palmleaf") || name.Contains("stone") ||
                name.Contains("clay") || name.Contains("sand") || name.Contains("seaweed") ||
                name.Contains("dirt") || name.Contains("ore"))
            {
                return ItemCategory.PrimaryResources;
            }

            // 10. Quest & Valuables
            if (name.Contains("blueprint") || name.Contains("note") || name.Contains("key") ||
                name.Contains("token") || name.Contains("cassette") || name.Contains("ticket") ||
                name.Contains("recorder") || name.Contains("letter"))
            {
                return ItemCategory.QuestAndValuables;
            }

            // Check Settings_Inventory or default
            if (item.settings_consumeable != null)
            {
                return ItemCategory.CookedFood;
            }

            return ItemCategory.Miscellaneous;
        }
    }
    // ============================================================================
    // [END] ITEM CATEGORY HELPER
    // ============================================================================
    #endregion
}
