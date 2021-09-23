using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using TabletopTweaks.Config;
using TabletopTweaks.Extensions;
using TabletopTweaks.Utilities;

namespace TabletopTweaks.NewContent.Feats
{
    static class DemonicToughness
    {
        public static void AddDemonicToughness()
        {
            var Toughness = Resources.GetBlueprint<BlueprintFeature>("d09b20029e9abfe4480b356c92095623");
            var ImpToughness = Resources.GetBlueprint<BlueprintFeature>("79570409-6492-4e4c-ac42-58876a51400a");
            var DemonicToughness = Helpers.CreateBlueprint<BlueprintFeature>("DemonicToughness", (System.Action<BlueprintFeature>)(bp => {
                bp.SetName("Demonic Toughness");
                bp.SetDescription("Your body has taken on the hardiness of a Demon. " + "You gain +9 {g|Encyclopedia:HP}hit points{/g} " + "For every {g|Encyclopedia:Hit_Dice}Hit Die{/g} you possess beyond 3, you gain an additional +3 hit points. " + "If you have more than 3 Hit Dice, you gain +3 hit points whenever you gain a Hit Die (such as when you gain a level).");
                bp.ReapplyOnLevelUp = false;
                bp.IsClassFeature = true;
                bp.Groups = new FeatureGroup[] { FeatureGroup.Feat };
                //Add the Demonic Heritage Shit prereqs
                bp.AddComponent(Helpers.Create<PrerequisiteStatValue>(c => {
                    c.Stat = StatType.Constitution;
                    c.Value = 16;
                }));
                //Actually does shit
                bp.AddComponent(new ToughnessLogic());
                bp.AddComponent(new ToughnessLogic());
                bp.AddComponent(new ToughnessLogic());
                //Feat Prerequisites
                bp.AddPrerequisiteFeature(Toughness);
                bp.AddPrerequisiteFeature(ImpToughness);
            }));
            if (ModSettings.AddedContent.Feats.DisableAll || !ModSettings.AddedContent.Feats.Enabled["DemonicToughness"]) { return; }
            FeatTools.AddAsFeat(DemonicToughness);
        }
    }
}