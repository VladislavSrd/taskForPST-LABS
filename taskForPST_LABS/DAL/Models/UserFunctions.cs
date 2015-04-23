using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace taskForPST_LABS.DAL.Models
{
    public partial class User
    {
        public bool HaveWriteAccess()
        {
            return UserGroups.Any(x => x.Group.Write);
        }

        public bool HaveReadAccess()
        {
            return UserGroups.Any(x => x.Group.Read);
        }
    }
}