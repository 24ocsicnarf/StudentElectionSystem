using AutoMapper.QueryableExtensions;
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
    public class PartyRepository : Repository, IPartyRepository
    {
        public async Task<IEnumerable<PartyModel>> GetPartiesAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                var parties = context.Parties.Where(p => p.ElectionId == electionId);

                return await _mapper.ProjectTo<PartyModel>(parties)
                    .ToListAsync();
            }
        }

        public async Task<int> GetPartiesCountAsync(int electionId)
        {
            using (var context = new StudentElectionContext())
            {
                return await context.Parties.CountAsync(p => p.ElectionId == electionId);
            }
        }

        public async Task<PartyModel> GetPartyAsync(int partyId)
        {
            using (var context = new StudentElectionContext())
            {
                var party = await context.Parties.FirstOrDefaultAsync(p => p.Id == partyId);
                if (party == null)
                {
                    return null;
                }

                var model = new PartyModel();
                _mapper.Map(party, model);

                return model;
            }
        }

        public async Task InsertPartyAsync(PartyModel model)
        {
            var party = new Party();
            _mapper.Map(model, party);

            using (var context = new StudentElectionContext())
            {
                context.Parties.Add(party);

                await context.SaveChangesAsync();
            }
        }

        public async Task UpdatePartyAsync(PartyModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var party = context.Parties.SingleOrDefault(p => p.Id == model.Id);
                _mapper.Map(model, party);
                
                await context.SaveChangesAsync();
            }
        }

        public async Task DeletePartyAsync(PartyModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var party = context.Parties.SingleOrDefault(p => p.Id == model.Id);
                context.Parties.Remove(party);

                await context.SaveChangesAsync();
            }
        }
    }
}
