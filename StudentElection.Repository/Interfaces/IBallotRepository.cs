using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Interfaces
{
    public interface IBallotRepository
    {
        Task<BallotModel> GetBallotByVinAsync(int electionId, string vin);
        Task InsertBallotAsync(BallotModel ballot);
        Task CastVotesAsync(int ballotId, IEnumerable<VoteModel> votes, DateTime castedAt);

        Task<IEnumerable<VoteResultModel>> GetVoteResultsAsync(int electionId);
    }
}
