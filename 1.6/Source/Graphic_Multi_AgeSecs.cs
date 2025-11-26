using UnityEngine;
using Verse;

namespace DanceOfEvolution
{
    [HotSwappable]
    public class Graphic_Multi_AgeSecs : Graphic_Multi
    {
        public MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

        public float AgeSecs(Thing thing)
        {
            var sec = (float)(Find.TickManager.TicksGame - thing.TickSpawned) / 60f;
            return sec  * 0.25f;
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            MatAt(rot, thing);
            if (thing is Building building)
            {
                propertyBlock.SetFloat(ShaderPropertyIDs.Working, building.IsWorking() ? 1f : 0f);
            }
            propertyBlock.SetFloat(ShaderPropertyIDs.Rotation, rot.AsInt);
            if (thing != null)
            {
                propertyBlock.SetFloat(ShaderPropertyIDs.RandomPerObject, thing.thingIDNumber.HashOffset());
            }
            Material material = MatAt(rot, thing);
            if (thing != null)
            {
                material.SetFloat(ShaderPropertyIDs.AgeSecs, AgeSecs(thing));
            }
            base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
        }

        public override void DrawMeshInt(Mesh mesh, Vector3 loc, Quaternion quat, Material mat)
        {
            Graphics.DrawMesh(mesh, loc, quat, mat, 0, null, 0, propertyBlock);
        }
    }

}
