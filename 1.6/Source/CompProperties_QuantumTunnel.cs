using Verse;

namespace DanceOfEvolution
{
    public class CompProperties_QuantumTunnel : CompProperties
    {
        public int travelDurationTicks = 600;
        public int bioferriteCostPerHex = 10;

        public CompProperties_QuantumTunnel()
        {
            compClass = typeof(CompQuantumTunnel);
        }
    }
}
