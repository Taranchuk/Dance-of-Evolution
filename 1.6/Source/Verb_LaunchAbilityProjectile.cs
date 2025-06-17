using Verse;
using RimWorld;

namespace DanceOfEvolution
{
	[HotSwappable]
	public class Verb_LaunchAbilityProjectile : Verb_LaunchProjectileStatic, IAbilityVerb
	{
		private Ability ability;

		public Ability Ability
		{
			get
			{
				if (ability is null && verbTracker.directOwner is Ability ability2)
				{
					ability = ability2;
				}
				return ability;
			}
			set
			{
				ability = value;
			}
		}

		public override bool TryCastShot()
		{
			bool num = base.TryCastShot();
			if (num)
			{
				Ability.StartCooldown(Ability.def.cooldownTicksRange.RandomInRange);
			}
			return num;
		}

		public override void ExposeData()
		{
			Scribe_References.Look(ref ability, "ability");
			base.ExposeData();
		}
	}
}
