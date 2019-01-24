namespace StudentElection.Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BallotModel
    {
        public BallotModel()
        {
            this.Votes = new HashSet<VoteModel>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public System.DateTime EnteredAt { get; set; }
        public Nullable<System.DateTime> CastedAt { get; set; }
        public int VoterId { get; set; }
    
        public VoterModel Voter { get; set; }
        public ICollection<VoteModel> Votes { get; set; }
    }
}
