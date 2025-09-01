using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
namespace DanceOfEvolution
{
	[HotSwappable]
	[StaticConstructorOnStartup]
	public class Building_Sporemaker : Building, IThingGlower
	{
		public CompRefuelable refuelableComp;
		public HediffDef sporeHediff;
		public int ticksSwitching;
		public bool Active => refuelableComp.HasFuel && ticksSwitching <= 0;
		public Graphic cachedSporeGraphic;
		public Graphic SporeGraphic
		{
			get
			{
				if (cachedSporeGraphic is null)
				{
					var suffix = "";
					if (sporeHediff == DefsOf.DE_HangingSporesMood)
					{
						suffix = "_green";
					}
					else if (sporeHediff == DefsOf.DE_HangingSporesConsciousness)
					{
						suffix = "_blue";
					}
					else
					{
						suffix = "_yellow";
					}
					cachedSporeGraphic = GetGraphicDataWithOtherPath(def.graphicData.texPath + suffix).GraphicColoredFor(this);
				}
				return cachedSporeGraphic;

			}
		}

		protected GraphicData GetGraphicDataWithOtherPath(string texPath)
		{
			var copy = new GraphicData();
			copy.CopyFrom(def.graphicData);
			copy.texPath = texPath;
			copy.shaderType = DefsOf.CutoutPlant;
			return copy;
		}

		private float sporeSway;
		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			base.DrawAt(drawLoc, flip);
			if (Active)
			{
				var offset = new Vector3(0, 1, 0.75f);
				SporeGraphic.MatSingle.SetFloat(ShaderPropertyIDs.SwayHead, sporeSway);
				SporeGraphic.Draw(DrawPos + offset + FloatingOffset(Find.TickManager.TicksGame), Rot4.South, this);

			}
		}

		public static Vector3 FloatingOffset(float tickOffset)
		{
			float num = tickOffset % 500f / 500f;
			float num2 = Mathf.Sin((float)Math.PI * num);
			float z = num2 * num2 * 0.04f;
			return new Vector3(0f, 0f, z);
		}

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			refuelableComp = GetComp<CompRefuelable>();
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				sporeHediff = DefsOf.DE_HangingSporesMood;
			}
		}

		public bool ShouldBeLitNow()
		{
			return refuelableComp.HasFuel;
		}

		public override void Tick()
		{
			base.Tick();
			if (ticksSwitching > 0)
			{
				ticksSwitching -= 100;
				if (ticksSwitching <= 0)
				{
					BroadcastCompSignal("CrateContentsChanged");
				}
			}
			else
			{
				refuelableComp.Notify_UsedThisTick();
			}
			sporeSway += 0.1f;
		}

		public static Texture2D Icon = ContentFinder<Texture2D>.Get("UI/Icons/Sporemaker");
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (var g in base.GetGizmos())
			{
				yield return g;
			}
			yield return new Command_ActionWithCooldown
			{
				cooldownPercentGetter = () => Mathf.InverseLerp(GenDate.TicksPerDay * 5, 0f, ticksSwitching),
				defaultLabel = "DE_SporeMode".Translate(sporeHediff.label),
				defaultDesc = "DE_SporeModeDesc".Translate(sporeHediff.label, sporeHediff.description),
				icon = Icon,
				action = delegate
				{
					var floatList = new List<FloatMenuOption>();
					AddSporeMode(floatList, DefsOf.DE_HangingSporesMood);
					AddSporeMode(floatList, DefsOf.DE_HangingSporesConsciousness);
					AddSporeMode(floatList, DefsOf.DE_HangingSporesMoving);
					Find.WindowStack.Add(new FloatMenu(floatList));
				},
				Disabled = ticksSwitching > 0
			};
		}

		private void AddSporeMode(List<FloatMenuOption> floatList, HediffDef sporeHediff)
		{
			floatList.Add(new FloatMenuOption(sporeHediff.label, delegate
			{
				SetSpore(sporeHediff);
			}));
		}

		private void SetSpore(HediffDef sporeHediff)
		{
			this.sporeHediff = sporeHediff;
			this.cachedSporeGraphic = null;
			this.ticksSwitching = GenDate.TicksPerDay * 5;
			BroadcastCompSignal("CrateContentsChanged");
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look(ref sporeHediff, "sporeHediff");
			Scribe_Values.Look(ref ticksSwitching, "ticksSwitching");
		}
	}
}