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
    
    public partial class Vote
    {
        public int Id { get; set; }
        public int BallotId { get; set; }
        public int CandidateId { get; set; }
    
        public virtual Ballot Ballot { get; set; }
        public virtual Candidate Candidate { get; set; }
    }
}
