using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.Repositories
{
    public class BallotRepository : Repository, IBallotRepository
    {
        public async Task CastVotesAsync(int ballotId, IEnumerable<VoteModel> votes, DateTime castedAt)
        {
            using (var context = new StudentElectionContext())
            {
                foreach (var model in votes)
                {
                    var vote = new Vote();
                    _mapper.Map(model, vote);

                    context.Votes.Add(vote);
                }

                var ballot = await context.Ballots.SingleOrDefaultAsync(b => b.Id == ballotId);
                ballot.CastedAt = castedAt;

                await context.SaveChangesAsync();
            }
        }

        public async Task<int> CountBallotsAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Ballots.CountAsync(b => b.Id == electionId);
            }
        }

        public async Task<int> CountCastedBallotsQuery(int voterId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Ballots.CountAsync(b => b.VoterId == voterId);
            }
        }

        public async Task<BallotModel> GetBallotAsync(int ballotId)
        {
            using (var context = new StudentElectionContext())
            {
                var ballot = await context.Ballots.SingleOrDefaultAsync(b => b.Id == ballotId);
                if (ballot == null)
                {
                    return null;
                }

                var model = new BallotModel();
                _mapper.Map(ballot, model);

                return model;
            }
        }

        public async Task<BallotModel> GetBallotByVinAsync(int electionId, string vin)
        {
            using (var context = new StudentElectionContext())
            {
                var ballot = await context.Ballots
                    .SingleOrDefaultAsync(b => b.Voter.ElectionId == electionId && b.Voter.Vin == vin);
                if (ballot == null)
                {
                    return null;
                }

                var model = new BallotModel();
                _mapper.Map(ballot, model);

                return model;
            }
        }

        public async Task<IEnumerable<VoteResultModel>> GetVoteResultsAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                var query = @"SELECT * FROM public.""VoteResult"" WHERE ""ElectionId"" = @electionId";

                return await context.Database.SqlQuery<VoteResultModel>(query,
                    new Npgsql.NpgsqlParameter("@electionId", electionId))
                    .ToListAsync();
            }
        }

        public async Task<BallotModel> InsertBallotAsync(VoterModel voter, DateTime enteredAt)
        {
            using (var context = new StudentElectionContext())
            {
                var ballot = new Ballot
                {
                    Code = string.Empty,
                    EnteredAt = enteredAt,
                    VoterId = voter.Id
                };

                context.Ballots.Add(ballot);

                await context.SaveChangesAsync();
            }

            return await GetBallotByVinAsync(voter.ElectionId, voter.Vin);
        }

        public async Task SetBallotCodeAsync(BallotModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var ballot = await context.Ballots.SingleOrDefaultAsync(b => b.Id == model.Id);
                ballot.Code = model.Code;

                await context.SaveChangesAsync();
            }
        }
    }
}
