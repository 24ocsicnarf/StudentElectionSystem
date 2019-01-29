using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace StudentElection.PostgreSQL.Repositories
{
    public class VoterRepository : Repository, IVoterRepository
    {
        public async Task<int> CountVotedVotersAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Voters.Where(v => v.ElectionId == electionId).CountAsync(v => v.Ballots.Any());
            }
        }

        public async Task<int> CountVotersAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Voters.CountAsync(v => v.ElectionId == electionId);
            }
        }

        public async Task DeleteVoterAsync(VoterModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var voter = await context.Voters.SingleOrDefaultAsync(v => v.Id == model.Id);
                context.Voters.Remove(voter);

                await context.SaveChangesAsync();
            }
        }

        public async Task<VoterModel> GetVoterAsync(int voterId)
        {
            using (var context = new StudentElectionContext())
            {
                var voter = await context.Voters.SingleOrDefaultAsync(v => v.Id == voterId);
                if (voter == null)
                {
                    return null;
                }

                var model = new VoterModel();
                _mapper.Map(voter, model);

                return model;
            }
        }

        public async Task<VoterModel> GetVoterByVinAsync(int electionId, string vin)
        {
            using (var context = new StudentElectionContext())
            {
                var voter = await context.Voters.SingleOrDefaultAsync(v => v.ElectionId == electionId && v.Vin == vin);
                if (voter == null)
                {
                    return null;
                }

                var model = new VoterModel();
                _mapper.Map(voter, model);

                return model;
            }
        }

        public async Task<IEnumerable<VoterModel>> GetVoterDetailsListAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                var voters = context.Voters.Where(v => v.ElectionId == electionId)
                    .Include(v => v.Ballots);

                return await _mapper.ProjectTo<VoterModel>(voters)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<VoterModel>> GetVotersAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                var voters = context.Voters.Where(v => v.ElectionId == electionId);

                return await _mapper.ProjectTo<VoterModel>(voters)
                    .ToListAsync();
            }
        }

        public async Task InsertVoterAsync(VoterModel model)
        {
            var voter = new Voter();
            _mapper.Map(model, voter);

            using (var context = new StudentElectionContext())
            {
                context.Voters.Add(voter);

                await context.SaveChangesAsync();
            }
        }

        public async Task InsertVotersAsync(IEnumerable<VoterModel> voters)
        {
            using (var context = new StudentElectionContext())
            {
                foreach (var model in voters)
                {
                    var voter = new Voter();
                    _mapper.Map(model, voter);

                    context.Voters.Add(voter);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsVinExistingAsync(int electionId, string vin, VoterModel model)
        {
            var voter = await GetVoterByVinAsync(electionId, vin);

            if (model == null)
            {
                return voter != null;
            }
            else
            {
                return voter != null && voter.Id != model.Id;
            }
        }

        public async Task UpdateVoterAsync(VoterModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var voter = context.Voters.SingleOrDefaultAsync(v => v.Id == model.Id);

                _mapper.Map(model, voter);

                await context.SaveChangesAsync();
            }
        }
    }
}
