namespace StudentElection.Repository.Models
{
    using Project.Library.Helpers;
    using Project.Library.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class VoterModel : IPersonName
    {
        public VoterModel()
        {
            this.Ballots = new HashSet<BallotModel>();
        }
    
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public Sex Sex { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Vin { get; set; }
        public int YearLevel { get; set; }
        public string Section { get; set; }
        public int ElectionId { get; set; }

        public ICollection<BallotModel> Ballots { get; set; }
        public ElectionModel Election { get; set; }
        
        public string FullName => DataHelper.GetPersonFullName(this);
        public bool IsVoted => this.Ballots.SingleOrDefault(b => b.Id > 0 && b.CastedAt != null) != null;

    }
}
