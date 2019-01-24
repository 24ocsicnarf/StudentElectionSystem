namespace StudentElection.Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ElectionModel
    {
        public ElectionModel()
        {
            this.Parties = new HashSet<PartyModel>();
            this.Positions = new HashSet<PositionModel>();
            this.Voters = new HashSet<VoterModel>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TookPlaceOn { get; set; }
        public DateTime? CandidatesFinalizedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string ServerTag { get; set; }
    
        public ICollection<PartyModel> Parties { get; set; }
        public ICollection<PositionModel> Positions { get; set; }
        public ICollection<VoterModel> Voters { get; set; }
    }
}
