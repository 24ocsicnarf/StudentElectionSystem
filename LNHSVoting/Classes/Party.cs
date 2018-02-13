using LNHSVoting.LNHSVotingDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LNHSVoting.Classes
{
    public class Party
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Abbreviation { get; set; }
        public string RGB { get; set; }

        public System.Drawing.Color @Color
        {
            get
            {
                var rgb = RGB.Split(',').ToArray();
                return System.Drawing.Color.FromArgb(Convert.ToInt32(rgb[0]), Convert.ToInt32(rgb[1]), Convert.ToInt32(rgb[2]));
            }
        }
        public SolidColorBrush ColorBrush
        {
            get
            {
                return new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.R, Color.G, Color.B));
            }
        }
    }

    public static class Parties
    {
        private static PartyTableAdapter _tableAdapter = new PartyTableAdapter();
        private static Dictionary<int, Party> _dictionary = new Dictionary<int, Party>();
        private static bool _isLoaded = false;

        public static Dictionary<int, Party> Dictionary
        {
            get
            {
                if (!_isLoaded) LoadData();

                return _dictionary;
            }
        }
        
        public static void InsertData(Party party)
        {
            _tableAdapter.Insert(party.Title, party.Abbreviation, party.RGB);
            party.ID = (int)_tableAdapter.GetLastID();
            
            _dictionary.Add(party.ID, party);
        }

        public static void UpdateData(Party party)
        {
            _tableAdapter.UpdateQuery(party.Title, party.Abbreviation, party.RGB, party.ID);

            Candidates.UpdateParty(party);

            _dictionary[party.ID] = party;
        }

        public static void DeleteData(int id)
        {
            _tableAdapter.Delete(id);

            var deletedIds = Candidates.Dictionary.Where(x => x.Value.Party.ID == id).Select(x => x.Key).ToList();
            foreach (var key in deletedIds)
                Candidates.DeleteData(key);

            _dictionary.Remove(id);
        }

        private static void LoadData()
        {
            var rows = _tableAdapter.GetData();

            foreach (var row in rows)
            {
                var party = new Party()
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Abbreviation = Convert.ToString(row["Abbreviation"]),
                    Title = Convert.ToString(row["Title"]),
                    RGB = Convert.ToString(row["RGB"])
                };
                _dictionary.Add(party.ID, party);
            }
            _isLoaded = true;
        }
    }
}
