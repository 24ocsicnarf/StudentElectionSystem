//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudentElection.PostgreSQL.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Party
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Party()
        {
            this.Candidates = new HashSet<Candidate>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortName { get; set; }
        public int Argb { get; set; }
        public int ElectionId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual Election Election { get; set; }
    }
}
