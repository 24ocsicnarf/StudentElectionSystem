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
        Task<int> CountBallotsAsync(int electionId);
        Task<BallotModel> GetBallotAsync(int ballotId);
        Task<BallotModel> GetBallotByVinAsync(int electionId, string vin);
        Task<BallotModel> InsertBallotAsync(VoterModel voter, DateTime enteredAt);
        Task SetBallotCodeAsync(BallotModel ballot);
        Task CastVotesAsync(int ballotId, IEnumerable<VoteModel> votes, DateTime castedAt);
        Task<int> CountCastedBallotsAsync(int voterId);

        Task<IEnumerable<VoteResultModel>> GetVoteResultsAsync(int electionId);
    }
}
