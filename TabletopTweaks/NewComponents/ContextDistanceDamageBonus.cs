using Kingmaker.Armies.TacticalCombat;
using Kingmaker.Designers;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.Utility;

namespace TabletopTweaks.NewComponents
{
    public class ContextDistanceDamageBonus : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleDealDamage>, IRulebookHandler<RuleDealDamage>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public Feet Range;

        public ContextValue Bonus;

        public void OnEventAboutToTrigger(RuleDealDamage evt) {
            bool isRangedWeapon = evt.DamageBundle.Weapon != null && evt.DamageBundle.Weapon.Blueprint.IsRanged;
            bool abilityWithRangedAttackRole = (evt.Reason.Ability?.Blueprint.GetDeliverProjectile(TacticalCombatHelper.IsActive))?.NeedAttackRoll ?? false;
            bool withinRange = evt.Target.DistanceTo(evt.Initiator) <= Range.Meters + evt.Target.Corpulence + evt.Initiator.Corpulence;
            if ((isRangedWeapon | abilityWithRangedAttackRole) & withinRange) {
                evt.DamageBundle.First?.AddModifierTargetRelated(Bonus.Calculate(Context), Fact);
            }
        }

        public void OnEventDidTrigger(RuleDealDamage evt) { }
    }
}