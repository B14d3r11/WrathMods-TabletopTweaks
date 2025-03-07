﻿using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Spells;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.Utility;
using System;
using System.Linq;

namespace TabletopTweaks.NewComponents {
    [TypeId("070fd2a4a2cb4f198a44ae036082818c")]
    class AdditionalSpellSelection : UnitFactComponentDelegate, IUnitCompleteLevelUpHandler {

        private Spellbook spellbook { get => Owner.DemandSpellbook(SpellCastingClass); }
        private int adjustedMaxLevel {
            get {
                if (!UseOffset) { return MaxSpellLevel; }
                return Math.Max((spellbook?.MaxSpellLevel ?? 0) - SpellLevelOffset, 1);
            }
        }
        public override void OnFactAttached() {
            LevelUpController controller = Kingmaker.Game.Instance?.LevelUpController;
            if (controller == null) { Main.LogDebug("null controller"); return; }
            if (spellbook == null) { Main.LogDebug("null book"); return; }
            spellSelection = controller.State.DemandSpellSelection(spellbook.Blueprint, SpellList);
            spellSelection.SetExtraSpells(Count, adjustedMaxLevel);
        }
        public override void OnDeactivate() {
            if (spellSelection == null) { return; }
            LevelUpController controller = Kingmaker.Game.Instance?.LevelUpController;
            if (controller == null) { return; }
            if (spellbook == null) { return; }
            controller.State.SpellSelections.Remove(spellSelection);
        }

        public void HandleUnitCompleteLevelup(UnitEntityData unit) {
            spellSelection = null;
        }

        private SpellSelectionData spellSelection;

        public BlueprintSpellListReference SpellList;
        public BlueprintCharacterClassReference SpellCastingClass;
        public int MaxSpellLevel;
        public bool UseOffset;
        public int SpellLevelOffset;
        public int Count = 1;

        [HarmonyPatch(typeof(SpellSelectionData), "CanSelectAnything", new Type[]{ typeof(UnitDescriptor) })]
        static class SpellSelectionData_CanSelectAnything_AdditionalSpellSelection_Patch {
            static void Postfix(SpellSelectionData __instance, ref bool __result, UnitDescriptor unit) {
                Spellbook spellbook = unit.Spellbooks.FirstOrDefault((Spellbook s) => s.Blueprint == __instance.Spellbook);
                if (spellbook == null) {
                    __result = false;
                }
                if (!__instance.Spellbook.AllSpellsKnown) { return; }
                if (__instance.ExtraSelected != null && __instance.ExtraSelected.Length != 0) {
                    if (__instance.ExtraSelected.HasItem((BlueprintAbility i) => i == null) && !__instance.ExtraByStat) {
                        for (int k = 0; k <= __instance.ExtraMaxLevel; k++) {
                            int ii = k;
                            if (__instance.SpellList.SpellsByLevel[k].SpellsFiltered.HasItem((BlueprintAbility sb) => !sb.IsCantrip 
                            && !__instance.SpellbookContainsSpell(spellbook, ii, sb) && !__instance.ExtraSelected.Contains(sb))) {
                                __result = true;
                            }
                        }
                    }
                }
            }
        }
        [HarmonyPatch(typeof(CharGenSpellsPhaseVM), "DefinePhaseMode", new Type[] { typeof(SpellSelectionData), typeof(SpellSelectionData.SpellSelectionState) })]
        static class CharGenSpellsPhaseVM_DefinePhaseMode_AdditionalSpellSelection_Patch {
            static void Postfix(CharGenSpellsPhaseVM __instance, ref CharGenSpellsPhaseVM.SpellSelectorMode __result, SpellSelectionData selectionData) {
                if (!selectionData.Spellbook.AllSpellsKnown) { return; }
                if (selectionData.ExtraSelected.Any<BlueprintAbility>() && !selectionData.ExtraByStat) {
                    __result = CharGenSpellsPhaseVM.SpellSelectorMode.AnyLevels;
                }
            }
        }
        [HarmonyPatch(typeof(CharGenSpellsPhaseVM), "OrderPriority", MethodType.Getter)]
        static class CharGenSpellsPhaseVM_OrderPriority_AdditionalSpellSelection_Patch {
            static void Postfix(CharGenSpellsPhaseVM __instance, ref int __result) {
                if (__instance?.m_SelectionData == null) { return; }
                if (__instance.m_SelectionData.Spellbook?.SpellList?.AssetGuid != __instance.m_SelectionData.SpellList?.AssetGuid) {
                    __result -= 500;
                }
            }
        }
    }
}
