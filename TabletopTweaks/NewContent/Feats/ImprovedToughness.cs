using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using TabletopTweaks.Config;
using TabletopTweaks.Extensions;
using TabletopTweaks.NewComponents;
using TabletopTweaks.Utilities;

namespace TabletopTweaks.NewContent.Feats {
    static class ImpToughness {
        public static void AddImpToughness() {
            var Toughness = Resources.GetBlueprint<BlueprintFeature>("d09b20029e9abfe4480b356c92095623");
            var ImpToughness = Helpers.CreateBlueprint<BlueprintFeature>("ImpToughness", (System.Action<BlueprintFeature>)(bp => {
                bp.SetName("Improved Toughness");
                bp.SetDescription("You have exceptional physical stamina.");
                bp.ReapplyOnLevelUp = true;
                bp.IsClassFeature = true;
				bp.Groups = new FeatureGroup[] { FeatureGroup.Feat };
//Actually does shit
				bp.AddComponent(new ToughnessLogic())
				bp.AddComponent(new ToughnessLogic())
//Feat Prerequisites
				bp.AddPrerequisiteFeature(Toughness);
            }));
            if (ModSettings.AddedContent.Feats.DisableAll || !ModSettings.AddedContent.Feats.Enabled["ImpToughness"]) { return; }
            FeatTools.AddAsFeat(ImpToughness);
        }
        }
    }
}
