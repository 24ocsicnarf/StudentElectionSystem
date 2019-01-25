using AutoMapper;
using AutoMapper.QueryableExtensions;
using StudentElection.MSAccess.StudentElectionDataSetTableAdapters;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static StudentElection.MSAccess.StudentElectionDataSet;

namespace StudentElection.MSAccess.Repositories
{
    public class PartyRepository : Repository, IPartyRepository
    {
        public async Task<IEnumerable<PartyModel>> GetPartiesAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PartyTableAdapter())
            {
                var parties = tableAdapter.GetData(electionId)
                    .AsQueryable();

                return _mapper.ProjectTo<PartyModel>(parties);
            }
        }

        public async Task<int> GetPartiesCount(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PartyTableAdapter())
            {
                return tableAdapter.CountPartiesQuery(electionId) ?? 0;
            }
        }

        public async Task<PartyModel> GetPartyAsync(int partyId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PartyTableAdapter())
            {
                var row = tableAdapter.GetParties(partyId).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new PartyModel();
                _mapper.Map(row, model);

                return model;
            }
        }

        public async Task InsertPartyAsync(PartyModel model)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PartyTableAdapter())
            {
                tableAdapter.Insert(
                    model.Title,
                    model.ShortName,
                    model.Argb,
                    model.ElectionId
                );
            }
        }

        public async Task UpdatePartyAsync(PartyModel model)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PartyTableAdapter())
            {
                tableAdapter.Update(
                    model.Title,
                    model.ShortName,
                    model.Argb,
                    model.ElectionId,
                    model.Id
                );
            }
        }
        public async Task DeletePartyAsync(PartyModel model)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PartyTableAdapter())
            {
                tableAdapter.Delete(
                    model.Id
                );
            }
        }

    }
}
