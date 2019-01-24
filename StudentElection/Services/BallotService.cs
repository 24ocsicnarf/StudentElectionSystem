using StudentElection.Repository;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Services
{
    public class BallotService
    {
        private readonly IBallotRepository _ballotRepository;

        public BallotService()
        {
            _ballotRepository = RepositoryFactory.Get<IBallotRepository>();
        }

        public async Task<BallotModel> GetBallotAsync(int ballotId)
        {
            return await _ballotRepository.GetBallotAsync(ballotId);
        }

        public async Task<BallotModel> GetBallotAsync(ElectionModel election, VoterModel voter)
        {
            var existingBallot = await _ballotRepository.GetBallotByVinAsync(election.Id, voter.Vin);
            if (existingBallot == null)
            {
                var enteredAt = DateTime.Now;
                existingBallot = await _ballotRepository.InsertBallotAsync(voter, enteredAt);
                existingBallot.Code = GetBallotCode(election.ServerTag, existingBallot);
                await _ballotRepository.SetBallotCodeAsync(existingBallot);
            }

            return existingBallot;
        }

        public async Task<BallotModel> GetBallotByVinAsync(ElectionModel election, VoterModel voter)
        {
            return await _ballotRepository.GetBallotByVinAsync(election.Id, voter.Vin);
        }

        public async Task CastVotesAsync(int ballotId, IEnumerable<VoteModel> votes)
        {
            var castedAt = DateTime.Now;

            await _ballotRepository.CastVotesAsync(ballotId, votes, castedAt);
        }

        public async Task<bool> IsVoterAlreadyVotedAsync(VoterModel voter)
        {
            var castedAt = DateTime.Now;

            var count = await _ballotRepository.CountCastedBallotsQuery(voter.Id);
            if (count > 1)
            {
                throw new ConstraintException($"Voter { voter.FullName } with VIN# { voter.Vin } has voted twice");
            }

            return count > 0;
        }

        public async Task<IEnumerable<VoteResultModel>> GetVoteResultsAsync(int electionId)
        {
            return await _ballotRepository.GetVoteResultsAsync(electionId);
        }

        public async Task<int> CountBallotsAsync(int electionId)
        {
            var castedAt = DateTime.Now;

            var count = await _ballotRepository.CountBallotsAsync(electionId);

            return count;
        }


        private string GetBallotCode(string serverTag, BallotModel ballot)
        {
            return string.Format("{0}{1:000000}", serverTag, ballot.Id);
        }
    }
}
