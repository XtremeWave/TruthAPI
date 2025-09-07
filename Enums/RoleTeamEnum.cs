using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruthAPI.Enums
{
    public static class RoleTeamEnum
    {
        public enum TeamTypes
        {
            Invalid = -1,
            Common,
            Crewmate,
            Impostor,
            Neutral,
            Role,
            MainRole,
            SubRole,
            Addon
        }
        public enum MoreTeamTypes
        {
            Invalid = -1,
            Common,
            Crewmate,
            Impostor,
            Neutral,
            MainRole,
            SubRole,
            Addon,
            Jester,
        }
    }
}
