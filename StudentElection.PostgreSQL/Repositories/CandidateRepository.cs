using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.Repositories
{
    public class CandidateRepository : Repository, ICandidateRepository
    {
        public async Task<CandidateModel> GetCandidateDetailsAsync(int candidateId)
        {
            using (var context = new StudentElectionContext())
            {
                var candidate = await context.Candidates
                    .Include(c => c.Party)
                    .Include(c => c.Position)
                    .SingleOrDefaultAsync(c => c.Id == candidateId);
                if (candidate == null)
                {
                    return null;
                }

                var model = new CandidateModel();
                _mapper.Map(candidate, model);

                return model;
            }
        }

        public async Task<IEnumerable<CandidateModel>> GetCandidateDetailsListByPartyAsync(int partyId)
        {
            using (var context = new StudentElectionContext())
            {
                var candidates = context.Candidates
                    .Include(c => c.Party)
                    .Include(c => c.Position)
                    .Where(c => c.PartyId == partyId)
                    .OrderBy(c => c.Position.Rank);

                return await _mapper.ProjectTo<CandidateModel>(candidates)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<CandidateModel>> GetCandidateDetailsListByPositionAsync(int positionId)
        {
            using (var context = new StudentElectionContext())
            {
                var candidates = context.Candidates
                        .Include(c => c.Party)
                        .Include(c => c.Position)
                        .Where(c => c.PositionId == positionId)
                        .OrderBy(c => c.Position.Rank);

                return await _mapper.ProjectTo<CandidateModel>(candidates)
                    .ToListAsync();
            }
        }

        public async Task<int> GetCandidatesCountAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Candidates.CountAsync(c => c.Party.ElectionId == electionId);
            }
        }

        public async Task<bool> IsAliasExistingAsync(int electionId, string alias, CandidateModel editingCandidate)
        {
            using (var context = new StudentElectionContext())
            {
                var candidate = await context.Candidates
                    .Where(c => c.Party.ElectionId == electionId)
                    .SingleOrDefaultAsync(c => c.Alias.ToLower() == alias.ToLower());

                if (editingCandidate == null)
                {
                    return candidate != null;
                }
                else
                {
                    return candidate != null && candidate.Id != editingCandidate.Id;
                }
            }

        }

        public async Task InsertCandidateAsync(CandidateModel model)
        {
            var candidate = new Candidate();
            _mapper.Map(model, candidate);

            using (var context = new StudentElectionContext())
            {
                context.Candidates.Add(candidate);

                await context.SaveChangesAsync();
            }
        }

        public async Task InsertCandidatesAsync(IEnumerable<CandidateModel> candidates)
        {
            using (var context = new StudentElectionContext())
            {
                foreach (var model in candidates)
                {
                    var candidate = new Candidate();
                    _mapper.Map(model, candidate);

                    context.Candidates.Add(candidate);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateCandidateAsync(CandidateModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var candidate = await context.Candidates.SingleOrDefaultAsync(c => c.Id == model.Id);
                _mapper.Map(model, candidate);
                
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCandidateAsync(CandidateModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var candidate = await context.Candidates.SingleOrDefaultAsync(c => c.Id == model.Id);
                context.Candidates.Remove(candidate);

                await context.SaveChangesAsync();
            }
        }

    }
}
