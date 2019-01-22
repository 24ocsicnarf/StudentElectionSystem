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
    public class PartyRepository : IPartyRepository
    {
        public async Task<IEnumerable<PartyModel>> GetPartiesAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new PartyTableAdapter())
            {
                return tableAdapter.GetData(electionId)
                    .AsQueryable()
                    .ProjectTo<PartyModel>()
                    .AsEnumerable();
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
                var party = tableAdapter.GetParties(partyId).SingleOrDefault();

                var model = new PartyModel();
                Mapper.Map(party, model);

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
