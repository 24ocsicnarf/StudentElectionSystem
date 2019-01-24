namespace StudentElection.Repository.Models
{
    using Project.Library;
    using Project.Library.Enums;
    using Project.Library.Extensions;
    using Project.Library.Helpers;
    using Project.Library.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public partial class CandidateModel : IPersonName
    {
        public CandidateModel()
        {
            this.Votes = new HashSet<VoteModel>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public Sex Sex { get; set; }
        public DateTime? Birthdate { get; set; }
        public int YearLevel { get; set; }
        public string Section { get; set; }
        public string Alias { get; set; }
        public string PictureFileName { get; set; }
        public int PositionId { get; set; }
        public int PartyId { get; set; }

        public PartyModel Party { get; set; }
        public PositionModel Position { get; set; }
        public ICollection<VoteModel> Votes { get; set; }

        public string FullName => DataHelper.GetPersonFullName(this, PersonNameFormat.LastNameFirst,
            System.Windows.Controls.CharacterCasing.Upper);
    }
}
