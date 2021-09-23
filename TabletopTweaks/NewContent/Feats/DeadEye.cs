using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using TabletopTweaks.Config;
using TabletopTweaks.Extensions;
using TabletopTweaks.NewComponents;
using TabletopTweaks.Utilities;

namespace TabletopTweaks.NewContent.Feats
{
    static class DeadEye
    {
        public static void AddDeadEye()
        {
            var PointBlankShot = Resources.GetBlueprint<BlueprintFeature>("0da0c194d6e1d43419eb8d990b28e0ab");
            var PowerAttack = Resources.GetBlueprint<BlueprintFeature>("9972f33f977fc724c838e59641b2fca5");
            var WeaponFocus = Resources.GetBlueprint<BlueprintFeature>("1e1f627d26ad36f43bbd26cc2bf8ac7e");
            var DeadEye = Helpers.CreateBlueprint<BlueprintFeature>("DeadEye", (System.Action<BlueprintFeature>)(bp =>
            {
                bp.SetName("Dead Eye");
                bp.SetDescription("Your precision with ranged weapons translates into more telling strikes than you would normally make. " +
                    "SPECIAL: Dead Eye does not increase the damage dealt to creatures immune to critical hits.");
                bp.Ranks = 1;
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Feat };
                //Adds Dex Mod to damage with ranged weapons within 30ft
                bp.AddComponent(Helpers.Create<ContextDistanceDamageBonus>(c =>
                {
                    c.Range = new Feet(30.0f);
                    c.Bonus = new ContextValue() { ValueType = ContextValueType.CasterProperty, Property = UnitProperty.StatBonusDexterity };
                }));
                //Not Recommended if they have Power Attack
                bp.AddComponent(Helpers.Create<RecommendationNoFeatFromGroup>(c =>
                {
                    c.m_Features = new BlueprintUnitFactReference[] {
                        PowerAttack.ToReference<BlueprintUnitFactReference>(),
                    };
                }));
                //Prerequisities
                //Dex Prereq
                bp.AddComponent(Helpers.Create<PrerequisiteStatValue>(c =>
                {
                    c.Stat = StatType.Dexterity;
                    c.Value = 17;
                }));
                //BAB Prereq
                bp.AddComponent(Helpers.Create<PrerequisiteStatValue>(c =>
                {
                    c.Stat = StatType.BaseAttackBonus;
                    c.Value = 1;
                }));
                //Feat Prerequisits
                bp.AddPrerequisiteFeature(PointBlankShot);
                bp.AddComponent(Helpers.Create<PrerequisiteParametrizedWeaponSubcategory>(c => {
                    c.SubCategory = WeaponSubCategory.Ranged;
                    c.m_Feature = WeaponFocus.ToReference<BlueprintFeatureReference>();
                }));
            }));
            if (ModSettings.AddedContent.Feats.DisableAll || !ModSettings.AddedContent.Feats.Enabled["DeadEye"]) { return; }
            FeatTools.AddAsFeat(DeadEye);
        }
    }
}