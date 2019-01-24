namespace StudentElection.Repository.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VoterModel
    {
        public VoterModel()
        {
            this.Ballot = new BallotModel();
        }
    
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public Sex Sex { get; set; }
        public Nullable<System.DateTime> Birthdate { get; set; }
        public string Vin { get; set; }
        public int YearLevel { get; set; }
        public string Section { get; set; }
        public int ElectionId { get; set; }

        public BallotModel Ballot { get; set; }
        public ElectionModel Election { get; set; }
        
        public string FullName => $"{ this.LastName }, { this.FirstName } { this.Suffix } { this.MiddleName }".Trim();
    }
}
