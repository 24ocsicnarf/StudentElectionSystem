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

namespace StudentElection.MSAccess.Repositories
{
    public class VoterRepository : IVoterRepository
    {
        public async Task<IEnumerable<VoterModel>> GetVotersAsync(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new VoterTableAdapter())
            {
                return tableAdapter.GetData()
                    .AsQueryable()
                    .ProjectTo<VoterModel>()
                    .AsEnumerable();
            }
        }

        public async Task<VoterModel> GetVoterAsync(int voterId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new VoterTableAdapter())
            {
                var row = tableAdapter.GetVotersById(voterId).SingleOrDefault();

                var model = new VoterModel();
                Mapper.Map(row, model);

                return model;
            }
        }

        public async Task InsertVoterAsync(VoterModel voter)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new VoterTableAdapter())
            {
                tableAdapter.Insert(
                    voter.FirstName,
                    voter.MiddleName,
                    voter.LastName,
                    voter.Suffix,
                    (int)voter.Sex,
                    voter.Birthdate,
                    voter.Vin,
                    voter.YearLevel,
                    voter.Section,
                    voter.ElectionId
                );
            }
        }

        public async Task UpdateVoterAsync(VoterModel voter)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new VoterTableAdapter())
            {
                tableAdapter.Update(
                    voter.FirstName,
                    voter.MiddleName,
                    voter.LastName,
                    voter.Suffix,
                    (int)voter.Sex,
                    voter.Birthdate,
                    voter.Vin,
                    voter.YearLevel,
                    voter.Section,
                    voter.ElectionId,
                    voter.Id
                );
            }
        }

        public async Task DeleteVoterAsync(VoterModel voter)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new VoterTableAdapter())
            {
                tableAdapter.Delete(
                    voter.Id
                );
            }
        }

        public async Task<bool> IsVinExistingAsync(int electionId, string vin, VoterModel voter)
        {
            var row = await GetVoterByVinAsync(electionId, vin);

            if (voter == null)
            {
                return row != null;
            }
            else
            {
                return row != null && row.Id != voter.Id;
            }
        }

        public async Task<VoterModel> GetVoterByVinAsync(int electionId, string vin)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new VoterTableAdapter())
            {
                var row = tableAdapter.GetVotersByVin(vin, electionId).SingleOrDefault();
                if (row == null)
                {
                    return null;
                }

                var model = new VoterModel();
                Mapper.Map(row, model);

                return model;
            }
        }
    }
}
