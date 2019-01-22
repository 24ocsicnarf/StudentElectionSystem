using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Models
{
    public class VoteResultModel
    {
        public int CandidateId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public int YearLevel { get; set; }
        public string Section { get; set; }
        public DateTime? Birthdate { get; set; }
        public Sex Sex { get; set; }
        public int PartyId { get; set; }
        public int PositionId { get; set; }
        public string Alias { get; set; }
        public int PositionRank { get; set; }
        public int VoteCount { get; set; }
        public int PositionVoteCount { get; set; }
        public string PositionTitle { get; set; }
        public string PartyTitle { get; set; }
        public string PartyShortName { get; set; }
        public int PartyArgb { get; set; }
        public string ServerTag { get; set; }
    }
}
