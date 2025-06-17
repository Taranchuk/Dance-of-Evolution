using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
    public class FloatMenuOptionProvider_NeuralEnhancer : FloatMenuOptionProvider
    {
        public override bool Drafted => true;
        public override bool Undrafted => true;
        public override bool Multiselect => false;
        public override bool Applies(FloatMenuContext context)
        {
			return context.FirstSelectedPawn is Pawn pawn && pawn.IsMutant && pawn.mutant.def == DefsOf.DE_FungalGhoul;
		}

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Thing clickedThing, FloatMenuContext context)
		{
			var pawn = context.FirstSelectedPawn;
			var comp = clickedThing.TryGetComp<CompUsableNeuralEnhancer>();
			if (comp != null)
			{
				foreach (FloatMenuOption floatMenuOption4 in clickedThing.GetFloatMenuOptions(pawn))
				{
					yield return floatMenuOption4;
				}
			}
        }
    }

}
