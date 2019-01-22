using StudentElection;
//using StudentElection.StudentElectionDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static StudentElection.G;

namespace StudentElection.Classes
{
    public class Candidate : Voter
    {
        public Party Party { get; set; }
        public Position @Position { get; set; }
        public string Alias { get; set; }
        public byte[] PictureByte { get; set; }

        public System.Drawing.Image PictureImage
        {
            get
            {
                if (PictureByte == null)
                {
                    Bitmap bitmap = new Bitmap(Properties.Resources.default_candidate);
                    //for(int x = 0; x < bitmap.Width; x++)
                    //{
                    //    for (int y = 0; y < bitmap.Height; y++)
                    //    {
                    //        if (bitmap.GetPixel(x, y).R == 0 && bitmap.GetPixel(x, y).G == 0 && bitmap.GetPixel(x, y).B == 0)
                    //        {
                    //            bitmap.SetPixel(x, y, @Party.Color);
                    //        }
                    //    }
                    //}
                    return bitmap;
                }

                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Bitmap));
                return (Bitmap)tc.ConvertFrom(PictureByte);
            }
        }
        public ImageSource PictureSource
        {
            get
            {
                return Imaging.CreateBitmapSourceFromHBitmap((PictureImage as Bitmap).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }

        public int VoteCount { get; set; }
        public int PositionVoteCount { get; set; }

        public void SetOtherInfo()
        {
            var voter = Voters.Dictionary[ID];

            LastName = voter.LastName;
            FirstName = voter.FirstName;
            MiddleName = voter.MiddleName;
            Type = voter.Type;

            GradeLevel = voter.GradeLevel;
            StrandSection = voter.StrandSection;
            VoterID = voter.VoterID;
            IsForeign = voter.IsForeign;
            Birthdate = voter.Birthdate;
            IsMale = voter.IsMale;

            Voters.UpdateIsACandidate(ID, true);
        }
    }

    public static class Candidates
    {
        private static CandidateTableAdapter _tableAdapter = new CandidateTableAdapter();
        private static Dictionary<int, Candidate> _dictionary = new Dictionary<int, Candidate>();
        private static bool _isLoaded = false;

        public static Dictionary<int, Candidate> Dictionary
        {
            get
            {
                if (!_isLoaded) LoadData();

                return _dictionary;
            }
        }

        public static void InsertData(Candidate candidate)
        {
            _tableAdapter.InsertQuery(candidate.ID, candidate.Party.ID, candidate.Position.ID, candidate.Alias, candidate.PictureByte);

            candidate.SetOtherInfo();

            candidate.Party = Parties.Dictionary[candidate.Party.ID];
            candidate.Position = Positions.Dictionary[candidate.Position.ID];

            _dictionary.Add(candidate.ID, candidate);
        }

        public static void UpdateData(int id, Candidate candidate)
        {
            _tableAdapter.UpdateQuery(candidate.ID, candidate.Position.ID, candidate.Party.ID, candidate.Alias, candidate.PictureByte, id);

            candidate.SetOtherInfo();

            candidate.ID = id;
            candidate.Party = Parties.Dictionary[candidate.Party.ID];
            candidate.Position = Positions.Dictionary[candidate.Position.ID];

            _dictionary[id] = candidate;
        }

        public static void UpdateParty(Party party)
        {
            _dictionary.Values.Where(x => x.Party.ID == party.ID).ToList().ForEach(x => x.Party = party);
        }

        public static void UpdatePosition(Position position)
        {
            _dictionary.Where(x => x.Value.Position.ID == position.ID).ToList().ForEach(x => x.Value.Position = position);
        }

        public static void UpdateInfo(Voter voter)
        {
            _dictionary[voter.ID].LastName = voter.LastName;
            _dictionary[voter.ID].FirstName = voter.FirstName;
            _dictionary[voter.ID].MiddleName = voter.MiddleName;
            _dictionary[voter.ID].Type = voter.Type;

            _dictionary[voter.ID].GradeLevel = voter.GradeLevel;
            _dictionary[voter.ID].StrandSection = voter.StrandSection;
            _dictionary[voter.ID].VoterID = voter.VoterID;
            _dictionary[voter.ID].IsForeign = voter.IsForeign;
            _dictionary[voter.ID].Birthdate = voter.Birthdate;
            _dictionary[voter.ID].IsMale = voter.IsMale;
        }

        public static void DeleteData(int id)
        {
            _tableAdapter.DeleteQuery(id);

            Voters.UpdateIsACandidate(id, false);
            _dictionary.Remove(id);
        }

        private static void LoadData()
        {
            var rows = _tableAdapter.GetData();

            foreach (var row in rows)
            {
                var id = Convert.ToInt32(row["UserID"]);
                var candidate = new Candidate()
                {
                    ID = id,
                    Party = Parties.Dictionary[Convert.ToInt32(row["PartyID"])],
                    Position = Positions.Dictionary[Convert.ToInt32(row["PositionID"])],
                    Alias = Convert.ToString(row["Alias"]),
                    PictureByte = row["Picture"] as byte[],
                };
                candidate.SetOtherInfo();
                _dictionary.Add(candidate.ID, candidate);
            }
            _isLoaded = true;
        }
    }
}
