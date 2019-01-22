namespace StudentElection.Repository.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Media;

    public partial class PartyModel
    {
        public PartyModel()
        {
            this.Candidates = new HashSet<CandidateModel>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortName { get; set; }
        public int Argb { get; set; }
        public int ElectionId { get; set; }

        public ICollection<CandidateModel> Candidates { get; set; }
        public ElectionModel Election { get; set; }

        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb(this.Argb);
            }
        }

        public SolidColorBrush ColorBrush
        {
            get
            {
                return new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, this.Color.R, this.Color.G, this.Color.B));
            }
        }
    }
}
