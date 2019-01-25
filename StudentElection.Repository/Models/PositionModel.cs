namespace StudentElection.Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PositionModel
    {
        public PositionModel()
        {
            this.Candidates = new HashSet<CandidateModel>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public int WinnersCount { get; set; }
        public int Rank { get; set; }
        public int? YearLevel { get; set; }
        public int ElectionId { get; set; }

        public override string ToString()
        {
            return this.Title;
        }

        public ICollection<CandidateModel> Candidates { get; set; }
        public ElectionModel Election { get; set; }
    }
}
