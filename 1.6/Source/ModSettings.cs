using Verse;

namespace DanceOfEvolution
{
    public class DanceOfEvolutionSettings : ModSettings
    {
        public bool showMushroomCap = true;
        public bool useTimelessHead = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref showMushroomCap, "showMushroomCap", true);
            Scribe_Values.Look(ref useTimelessHead, "useTimelessHead", true);
            base.ExposeData();
        }
    }
}