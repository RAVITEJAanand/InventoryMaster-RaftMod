# 🎒 Inventory Master — Dedicated Inventory Management Mod for Raft

**Author**: KONDURI (RAVITEJAanand)  
**Game**: Raft (The Final Chapter Update 1.09 / v13.01)  
**Version**: 1.0.0  
**Framework**: BepInEx 5.4.21, HarmonyLib 2.2, Unity 2020.3 UI  
**Compatibility**: 100% compatible with **Sailor's Companion** (Zero overlaps, zero hotkey clashes)

---

## 🌟 Overview (పరిచయం)

**Inventory Master** is a dedicated Quality-of-Life inventory management mod for **Raft**, purposefully crafted to work in complete harmony alongside **Sailor's Companion**. 

All features in Inventory Master are 100% unique, preventing duplicate patches or feature overlaps:
- **Sailor's Companion** handles: Raft Navigation, Teleport/Recall, Sails, Engines, World Cheats, Research Blueprints, Item Spawner, Craft from Storage, and Increased Stack Size.
- **Inventory Master** handles: Auto Sort (Backpack & Chests), Permanent 15-slot Backpack Expansion, 20-slot Hotbar Row Swap, Auto Pickup Nearby Items (5m sweep), Drop Protection, Smart Item Split, Context Auto Tool Equip & Replace Broken, Fast Bulk Transfer (Double-Click), Storage Dump (One-Click), Favorite Items Lock with `🔒` Badges, Trash Slot with Undo, and Auto Refill Potable Water & Cooked Food.

---

## 🎮 Keybindings & Shortcuts (హాట్‌కీలు)

| Hotkey | Action | Description |
| :--- | :--- | :--- |
| **`F2`** | **Inventory Master Menu** | Opens the rustic in-game settings canvas (Zero clash with Sailor's Companion). |
| **`Z`** | **Auto Sort Inventory** | Neatly organizes backpack or open chest into categories. |
| **`X`** | **Dump to Chest** | Deposits all non-locked backpack items into the open chest. |
| **`V`** | **Hotbar Row Swap** | Instantly swaps hotbar slots with backpack row 1 (20 hotbar items). |
| **`Alt + Left Click`** | **Favorite / Lock Slot** | Toggles slot lock status (shows `🔒` badge; immune to sort/dump/drop). |
| **`Double Left Click`** | **Fast Bulk Transfer** | Moves all matching stacks between backpack and open chest instantly. |
| **`Shift + Right Click`** | **Smart Split (1 item)** | Quickly splits exactly 1 item into next empty slot. |
| **`Ctrl + Right Click`** | **Smart Split (Half)** | Quickly splits half the stack into next empty slot. |
| **`Delete`** | **Trash Slot** | Hover over slot and press Delete to safely trash it (with 1-step undo). |
| **`Shift + Q`** | **Force Drop Item** | Overrides drop protection when intentionally dropping gear. |

---

## 🚀 Features Breakdown (ఫీచర్లు)

1. **✨ Auto Sort Inventory (`Z`)**: Merges partial stacks and reorganizes items into clean categories (Tools, Equipment, Drinks, Cooked Food, Seeds, Resources, Crafting Parts, Quest Items).
2. **🎒 Permanent Inventory Expansion**: Permanently unlocks and activates all 15 backpack slots.
3. **🔄 Hotbar Expansion (`V`)**: Press `V` to swap your active hotbar with backpack row 1 on the fly.
4. **🧲 Auto Pickup Nearby Items**: Sweeps loose debris and dropped items within 5m directly into your pockets.
5. **🛡️ Drop Protection**: Blocks accidental dropping (`Q`) of locked items and high-tier weapons/armor. Hold `Shift+Q` to override.
6. **✂️ Smart Item Split**: Instant 1-item split with `Shift+RightClick`, or half-stack split with `Ctrl+RightClick`.
7. **🛠️ Auto Equip Tools & Auto-Replace**: Context-aware equip on aim (Axe on trees, Hook on debris, Spear on animals); auto-replaces broken tools from backpack.
8. **⚡ Fast Item Transfer**: Double-clicking an item transfers all matching stacks between backpack and storage instantly.
9. **📥 One-Click Storage Dump (`X`)**: Deposits all eligible non-locked backpack items into open chest.
10. **🔒 Favorite Items Lock (`Alt+Click`)**: Renders `🔒` padlock badges; immune to auto-sort, dump, and drop.
11. **🗑️ Trash Slot with Safe Undo (`Delete`)**: Incinerates junk with an instant 1-step undo recovery buffer.
12. **💧 Auto Refill Water & Food**: Auto-consumes fresh water and cooked food when hunger/thirst dips below 25%.

---

## 🛠️ Installation
Place `InventoryMaster.dll` in `Raft/BepInEx/plugins/InventoryMaster/`. Launch Raft and press **`F2`**!
