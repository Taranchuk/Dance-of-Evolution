using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class Window_SelectPawnsForDemand : Window
    {
        private readonly Pawn envoy;
        private readonly List<TransferableOneWay> transferables;
        private readonly TransferableOneWayWidget pawnsTransfer;

        public override Vector2 InitialSize => new Vector2(800f, 700f);
        private int RequiredPawnCount => GameComponent_CurseManager.Instance.requiredPawnCount;
        private int SelectedPawnCount => transferables.Sum(x => x.CountToTransfer);

        public Window_SelectPawnsForDemand(Pawn envoy)
        {
            this.envoy = envoy;
            this.forcePause = true;
            this.absorbInputAroundWindow = true;
            transferables = new List<TransferableOneWay>();
            AddPawnsToTransferables();

            pawnsTransfer = new TransferableOneWayWidget(transferables, null, null,
                "DE_SelectPawnsDesc".Translate(),
                drawMass: false, IgnorePawnsInventoryMode.Ignore, includePawnsMassInMassUsage: true, availableMassGetter: () => 0, extraHeaderSpace: 0f, ignoreSpawnedCorpseGearAndInventoryMass: false, tile: null, drawMarketValue: true, drawEquippedWeapon: false, drawNutritionEatenPerDay: false, drawMechEnergy: false, drawItemNutrition: false, drawForagedFoodPerDay: false, drawDaysUntilRot: false, playerPawnsReadOnly: false, drawIdeo: false, drawXenotype: false);
        }

        public override void DoWindowContents(Rect inRect)
        {
            int selected = SelectedPawnCount;
            int required = RequiredPawnCount;
            var selectedText = selected > required ? $"<color=red>{selected}</color>" : selected.ToString();
            var title = "DE_SelectPawnsTitle".Translate() + $" ({selectedText}/{required})";

            Rect titleRect = new Rect(0f, 0f, inRect.width, 35f);
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(titleRect, title);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Rect counterRect = new Rect(inRect.width - 200f, 45f, 200f, 25f);
            DrawPawnCounter(counterRect);

            inRect.yMin += 45f;
            Widgets.DrawMenuSection(inRect);
            inRect = inRect.ContractedBy(17f);
            Widgets.BeginGroup(inRect);
            Rect contentRect = inRect.AtZero();
            DoBottomButtons(contentRect);
            Rect listRect = contentRect;
            listRect.yMax -= 59f;
            pawnsTransfer.OnGUI(listRect, out _);
            Widgets.EndGroup();
        }

        private void DrawPawnCounter(Rect rect)
        {
            int selected = SelectedPawnCount;
            int required = RequiredPawnCount;
            string label = $"{selected} / {required}";
            GUI.color = selected == required ? Color.white : Color.red;
            Text.Anchor = TextAnchor.MiddleRight;
            Widgets.Label(rect, label);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void DoBottomButtons(Rect rect)
        {
            Rect acceptButtonRect = new Rect(rect.width / 2f - 160f / 2f, rect.height - 55f, 160f, 40f);

            bool canAccept = SelectedPawnCount == RequiredPawnCount;
            if (!canAccept)
            {
                Widgets.ButtonText(acceptButtonRect, "Accept".Translate(), active: true);
            }
            else if (Widgets.ButtonText(acceptButtonRect, "Accept".Translate()))
            {
                List<Pawn> pawnsToDeliver = transferables.Where(t => t.CountToTransfer > 0)
                                                         .Select(t => (Pawn)t.AnyThing)
                                                         .ToList();
                var lord = envoy.GetLord();
                PawnDemandUtility.DeliverPawns(pawnsToDeliver, lord);

                var factionRelation = new FactionRelation();
                factionRelation.other = Faction.OfPlayer;
                factionRelation.kind = FactionRelationKind.Ally;
                factionRelation.baseGoodwill = 100;

                envoy.Faction.SetRelation(factionRelation);

                var instance = GameComponent_CurseManager.Instance;
                instance.mycelyssDemandActive = true;
                instance.mycelyssDemandTick = Find.TickManager.TicksGame + (GenDate.TicksPerDay * 10);
                instance.requiredPawnCount = 2;
                Messages.Message("DE_MycelyssDemandActivated".Translate(instance.requiredPawnCount, 10), MessageTypeDefOf.NeutralEvent);
                Find.LetterStack.ReceiveLetter("DE_MycelyssAlly".Translate(), "DE_MycelyssAllyDesc".Translate(), LetterDefOf.NeutralEvent, envoy);
                lord?.ReceiveMemo("Leave");
                Find.WindowStack.TryRemove(typeof(Dialog_MycelyssEnvoy));
                Close();
            }

            if (Widgets.ButtonText(new Rect(acceptButtonRect.x - 170f, acceptButtonRect.y, 160f, 40f), "Reset".Translate()))
            {
                foreach (var t in transferables) t.CountToTransfer = 0;
            }

            if (Widgets.ButtonText(new Rect(acceptButtonRect.xMax + 10f, acceptButtonRect.y, 160f, 40f), "Cancel".Translate()))
            {
                Close();
            }
        }

        private void AddPawnsToTransferables()
        {
            foreach (Pawn p in PawnDemandUtility.GetValidDemandPawns())
            {
                var transferable = new TransferableOneWay();
                transferable.things.Add(p);
                transferables.Add(transferable);
            }
        }
    }
}
