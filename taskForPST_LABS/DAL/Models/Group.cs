//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace taskForPST_LABS.DAL.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Group
    {
        public Group()
        {
            this.UserGroups = new HashSet<UserGroup>();
        }
    
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
    
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}
