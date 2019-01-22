using StudentElection;
//using StudentElection.StudentElectionDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Classes
{
    public class Ballot
    {
        public int UserID { get; set; }
        public string Code { get; set; }
    }

    public static class Ballots
    {
        private static BallotTableAdapter _tableAdapter = new BallotTableAdapter();
        private static Dictionary<int, Ballot> _dictionary = new Dictionary<int, Ballot>();
        private static List<Candidate> _results = new List<Candidate>();
        private static bool _isLoaded = false;

        public static List<Candidate> Results
        {
            get
            {
                if (_results.Count == 0)
                {
                    var vrAdapter = new VoteResultsTableAdapter();
                    var vrData = vrAdapter.GetData();
                    
                    foreach(var row in vrData)
                    {
                        var candidate = Candidates.Dictionary[Convert.ToInt32(row["UserID"])];
                        candidate.VoteCount = Convert.ToInt32(row["VoteCount"]);
                        candidate.PositionVoteCount = Convert.ToInt32(row["PositionVoteCount"]);

                        _results.Add(candidate);
                    }
                }

                return _results.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName).ThenBy(x => x.Alias).ToList();
            }
        }

        public static Dictionary<int, Ballot> Dictionary
        {
            get
            {
                if (!_isLoaded) LoadData();

                return _dictionary;
            }
        }

        public static void InsertData(Ballot ballot)
        {
            _tableAdapter.Insert(ballot.UserID, ballot.Code);
            _dictionary.Add(ballot.UserID, ballot);
        }

        private static void LoadData()
        {
            var rows = _tableAdapter.GetData();

            foreach (var row in rows)
            {
                var ballot = new Ballot()
                {
                    UserID = Convert.ToInt32(row["UserID"]),
                    Code = Convert.ToString(row["Code"])
                };
                _dictionary.Add(ballot.UserID, ballot);
            }

            _isLoaded = true;
        }
    }
}
