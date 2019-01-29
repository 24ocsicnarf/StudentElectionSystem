using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.Model
{
    public partial class Voter
    {
        [NotMapped]
        public Ballot Ballot
        {
            get
            {
                return this.Ballots.FirstOrDefault();
            }
        }
    }
}
