using AutoMapper;
using StudentElection.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Models;

namespace StudentElection.PostgreSQL.Repositories
{
    public class ElectionRepository : Repository, IElectionRepository
    {
        public async Task<int> AddElectionAsync(ElectionModel model)
        {
            using (var context = new StudentElectionContext())
            {
                var election = new Election();
                _mapper.Map(model, election);

                context.Elections.Add(election);

                return await context.SaveChangesAsync();
            }
        }

        public async Task CloseElectionAsync(int electionId, DateTime dateTime)
        {
            using (var context = new StudentElectionContext())
            {
                var election = await context.Elections.SingleOrDefaultAsync(e => e.Id == electionId);
                election.ClosedAt = dateTime;

                await context.SaveChangesAsync();
            }
        }

        public async Task FinalizeCandidatesAsync(int electionId, DateTime dateTime)
        {
            using (var context = new StudentElectionContext())
            {
                var election = await context.Elections.SingleOrDefaultAsync(e => e.Id == electionId);
                election.CandidatesFinalizedAt = dateTime;

                await context.SaveChangesAsync();
            }
        }

        public async Task<ElectionModel> GetCurrentElectionAsync()
        {
            using (var context = new StudentElectionContext())
            {
                var election = await context.Elections
                    .OrderByDescending(e => e.TookPlaceOn)
                    .ThenByDescending(e => e.Id).FirstOrDefaultAsync();
                if (election == null)
                {
                    return null;
                }

                var model = new ElectionModel();
                _mapper.Map(election, model);

                return model;
            }
        }

        public async Task UpdateTagAsync(int electionId, string tag)
        {
            using (var context = new StudentElectionContext())
            {
                var election = await context.Elections.SingleOrDefaultAsync(e => e.Id == electionId);
                election.ServerTag = tag;

                await context.SaveChangesAsync();
            }
        }
    }
}
