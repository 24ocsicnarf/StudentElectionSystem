using AutoMapper;
using StudentElection.MSAccess.StudentElectionDataSetTableAdapters;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.MSAccess.Repositories
{
    public class ElectionRepository : Repository, IElectionRepository
    {
        public async Task CloseElectionAsync(int electionId, DateTime dateTime)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new ElectionTableAdapter())
            {
                tableAdapter.CloseElectionQuery(dateTime, electionId);
            }
        }

        public async Task FinalizeCandidatesAsync(int electionId, DateTime dateTime)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new ElectionTableAdapter())
            {
                tableAdapter.FinalizeCandidatesQuery(dateTime, electionId);
            }
        }

        public async Task<ElectionModel> GetCurrentElectionAsync()
        {
            await Task.CompletedTask;

            using (var tableAdapter = new ElectionTableAdapter())
            {
                var row = tableAdapter.GetCurrentElections().SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new ElectionModel();
                _mapper.Map(row, model);

                return model;
            }
        }

        public async Task UpdateTagAsync(int electionId, string serverTag)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new ElectionTableAdapter())
            {
                tableAdapter.UpdateServerTagQuery(serverTag, electionId);
            }
        }
    }
}
