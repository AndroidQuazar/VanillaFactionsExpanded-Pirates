using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VFEPirates
{

    public class VerbProps_MultipleProjectiles : VerbProperties
    {
        public int projectileCount;
    }
	public class Verb_LaunchProjectileStaticMultiple : Verb_LaunchProjectileStatic
    {
        public VerbProps_MultipleProjectiles VerbProps => this.verbProps as VerbProps_MultipleProjectiles;
        protected override bool TryCastShot()
        {
            bool result = false;
            for (var i = 0; i < VerbProps.projectileCount; i++)
            {
                result = base.TryCastShot();
            }
            return result;
        }
    }
}
