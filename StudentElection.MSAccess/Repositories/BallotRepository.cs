using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.MSAccess.Repositories
{
    public class BallotRepository : IBallotRepository
    {
        public Task CastVotesAsync(int ballotId, IEnumerable<VoteModel> votes, DateTime castedAt)
        {
            throw new NotImplementedException();
        }

        public Task<BallotModel> GetBallotByVinAsync(int electionId, string vin)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VoteResultModel>> GetVoteResultsAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public Task InsertBallotAsync(BallotModel ballot)
        {
            throw new NotImplementedException();
        }
    }
}
