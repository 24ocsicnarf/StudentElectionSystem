using StudentElection;
//using StudentElection.StudentElectionDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudentElection.G;

namespace StudentElection.Classes
{
    public class Voter : User
    {
        public int GradeLevel { get; set; }
        public string StrandSection { get; set; }
        public bool IsMale { get; set; }
        public string VoterID { get; set; }
        public bool IsForeign { get; set; }

        public string GradeStrand
        {
            get
            {
                return "Grade " + GradeLevel + " " + StrandSection;
            }
        }
        public SexType @SexType
        {
            get
            {
                if (IsMale) return SexType.Male;
                else return SexType.Female;
            }
        }
        public string Sex
        {
            get
            {
                if (IsMale) return SexType.Male.ToString();
                else return SexType.Female.ToString();
            }
            set
            {
                value = Sex;
            }
        }
        public DateTime Birthdate { get; set; }
        public string BirthdateString
        {
            get
            {
                return string.Format("{0:MMMM d, yyyy}", Birthdate);
            }
            set
            {
                value = BirthdateString;
            }
        }

        public bool IsVoted { get; set; }
        public bool IsACandidate { get; set; }
    }

    public static class Voters
    {
        private static VoterTableAdapter _tableAdapter = new VoterTableAdapter();
        private static Dictionary<int, Voter> _dictionary = new Dictionary<int, Voter>();
        private static bool _isLoaded = false;

        public static Dictionary<int, Voter> Dictionary
        {
            get
            {
                if (!_isLoaded) LoadData();

                return _dictionary;
            }
        }

        public static void InsertData(Voter voter)
        {
            //_tableAdapter.InsertQuery(voter.LastName, voter.FirstName, voter.MiddleName, voter.GradeLevel, voter.StrandSection, voter.IsMale, voter.Birthdate, voter.VoterID, voter.IsForeign, Machine.ID);
            voter.ID = (int)_tableAdapter.GetLastID();
            _dictionary.Add(voter.ID, voter);
        }

        public static void UpdateData(Voter voter)
        {
            _tableAdapter.UpdateQuery(voter.LastName, voter.FirstName, voter.MiddleName, voter.GradeLevel, voter.StrandSection, voter.IsMale, voter.Birthdate, voter.VoterID, voter.IsForeign, voter.ID);
            voter.IsForeign = _dictionary[voter.ID].IsForeign;
            voter.IsACandidate = _dictionary[voter.ID].IsACandidate;

            if (voter.IsACandidate) Candidates.UpdateInfo(voter);

            _dictionary[voter.ID] = voter;
        }

        public static void UpdateIsForeign(int id, bool isForeign)
        {
            _dictionary[id].IsForeign = isForeign;
            _tableAdapter.UpdateForeign(isForeign, id);
        }

        public static void UpdateIsACandidate(int id, bool isACandidate)
        {
            _dictionary[id].IsACandidate = isACandidate;
        }

        public static void DeleteData(int id)
        {
            Candidates.DeleteData(id);
            Users.DeleteData(id);

            _tableAdapter.UpdateForeign(false, id);
            _dictionary.Remove(id);
        }

        private static void LoadData()
        {
            var rows = _tableAdapter.GetData();
            foreach (var row in rows)
            {
                var id = Convert.ToInt32(row["ID"]);
                _dictionary.Add(id, new Voter()
                {
                    ID = id,
                    LastName = Convert.ToString(row["LastName"]),
                    FirstName = Convert.ToString(row["FirstName"]),
                    MiddleName = Convert.ToString(row["MiddleName"]),
                    Type = (UserType)Convert.ToByte(row["Type"]),

                    GradeLevel = Convert.ToInt32(row["GradeLevel"]),
                    StrandSection = Convert.ToString(row["StrandSection"]),
                    VoterID = Convert.ToString(row["VoterID"]),
                    IsForeign = Convert.ToBoolean(row["IsForeign"]),
                    Birthdate = Convert.ToDateTime(row["Birthday"]),
                    IsMale = Convert.ToBoolean(row["IsMale"]),

                    IsVoted = Ballots.Dictionary.Keys.Contains(id)
                });
            }
            _isLoaded = true;
        }
    }
}
