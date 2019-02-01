using AutoMapper.QueryableExtensions;
using StudentElection.MSAccess.StudentElectionDataSetTableAdapters;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudentElection.MSAccess.StudentElectionDataSet;

namespace StudentElection.MSAccess.Repositories
{
    public class BallotRepository : Repository, IBallotRepository
    {
        public async Task<BallotModel> GetBallotAsync(int ballotId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new BallotTableAdapter())
            {
                var row = tableAdapter.GetBallotsById(ballotId).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new BallotModel();
                _mapper.Map(row, model);

                return model;
            }
        }

        public async Task<BallotModel> GetBallotByVinAsync(int electionId, string vin)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new BallotTableAdapter())
            {
                var row = tableAdapter.GetBallotsByVin(electionId, vin).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new BallotModel();
                _mapper.Map(row, model);

                return model;
            }
        }

        public async Task<BallotModel> InsertBallotAsync(VoterModel voter, DateTime enteredAt)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new BallotTableAdapter())
            {
                tableAdapter.Insert(
                    string.Empty,
                    enteredAt,
                    null,
                    voter.Id
                );
            }

            return await GetBallotByVinAsync(voter.ElectionId, voter.Vin);
        }

        public async Task SetBallotCodeAsync(BallotModel ballot)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new BallotTableAdapter())
            {
                tableAdapter.SetBallotCodeQuery(ballot.Code, ballot.Id);
            }
        }

        public async Task CastVotesAsync(int ballotId, IEnumerable<VoteModel> votes, DateTime castedAt)
        {
            await Task.CompletedTask;

            using (var dataSet = new StudentElectionDataSet())
            {
                using (var manager = new TableAdapterManager())
                {
                    manager.VoteTableAdapter = new VoteTableAdapter();
                    manager.BallotTableAdapter = new BallotTableAdapter();
                    
                    foreach (var vote in votes)
                    {
                        var newRow = dataSet.Vote.NewVoteRow();
                        newRow.BallotId = ballotId;
                        newRow.CandidateId = vote.CandidateId;

                        dataSet.Vote.AddVoteRow(newRow);
                    }

                    manager.BallotTableAdapter.FillBallotsById(dataSet.Ballot, ballotId);
                    var ballot = dataSet.Ballot.FindByID(ballotId);
                    ballot.CastedAt = castedAt;
                    
                    manager.UpdateAll(dataSet);
                }
            }
        }

        public async Task<int> CountCastedBallotsAsync(int voterId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new BallotTableAdapter())
            {
                var count = tableAdapter.CountCastedBallotsQuery(voterId);
                return count ?? 0;
            }
        }

        public async Task<IEnumerable<VoteResultModel>> GetVoteResultsAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new VoteResultTableAdapter())
            {
                var voteResults = tableAdapter.GetVoteResults(electionId)
                       .AsQueryable();

                return _mapper.ProjectTo<VoteResultModel>(voteResults);
            }
        }

        public async Task<int> CountBallotsAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new BallotTableAdapter())
            {
                var count = tableAdapter.CountBallotsQuery(electionId);
                return count ?? 0;
            }
        }
    }
}
