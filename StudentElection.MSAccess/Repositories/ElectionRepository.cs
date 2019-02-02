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

        public async Task<ElectionModel> GetElectionByServerTagAsync(string tag)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new ElectionTableAdapter())
            {
                var row = tableAdapter.GetElectionsByServerTag(tag).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new ElectionModel();
                _mapper.Map(row, model);

                return model;
            }
        }

        public async Task InsertElectionAsync(ElectionModel model)
        {
            await Task.CompletedTask;

            using (var dataSet = new StudentElectionDataSet())
            {
                using (var manager = new TableAdapterManager())
                {
                    manager.ElectionTableAdapter = new ElectionTableAdapter();

                    var newRow = dataSet.Election.NewElectionRow();
                    _mapper.Map(model, newRow);
                    newRow.ID = -1;
                    newRow.SetCandidatesFinalizedAtNull();
                    newRow.SetClosedAtNull();

                    dataSet.Election.AddElectionRow(newRow);

                    manager.UpdateAll(dataSet);
                }
            }
        }

        public async Task UpdateElectionAsync(ElectionModel election)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new ElectionTableAdapter())
            {
                tableAdapter.Update(
                    election.Title,
                    election.Description,
                    election.TookPlaceOn,
                    election.CandidatesFinalizedAt,
                    election.ClosedAt,
                    election.ServerTag,
                    election.Id
                );
            }
        }

        public async Task UpdateServerTagAsync(int electionId, string serverTag)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new ElectionTableAdapter())
            {
                tableAdapter.UpdateServerTagQuery(serverTag, electionId);
            }
        }
    }
}
