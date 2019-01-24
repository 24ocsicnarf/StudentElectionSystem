using AutoMapper;
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

namespace StudentElection.MSAccess.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        public async Task<CandidateModel> GetCandidateDetailsAsync(int candidateId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                return tableAdapter.GetCandidateDetails(candidateId)
                    .AsQueryable<DataRow>()
                    .ProjectTo<CandidateModel>()
                    .SingleOrDefault();
            }
        }

        public async Task<IEnumerable<CandidateModel>> GetCandidateDetailsListByPartyAsync(int partyId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                return tableAdapter.GetCandidateDetailsListByParty(partyId)
                    .AsQueryable<DataRow>()
                    .ProjectTo<CandidateModel>();
            }
        }

        public async Task<IEnumerable<CandidateModel>> GetCandidateDetailsListByPositionAsync(int positionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                return tableAdapter.GetCandidateDetailsListByPosition(positionId)
                    .AsQueryable<DataRow>()
                    .ProjectTo<CandidateModel>();
            }
        }

        public async Task InsertCandidateAsync(CandidateModel candidate)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                tableAdapter.Insert(
                    candidate.FirstName,
                    candidate.MiddleName,
                    candidate.LastName,
                    candidate.Suffix,
                    (int)candidate.Sex,
                    candidate.Birthdate,
                    candidate.YearLevel,
                    candidate.Section,
                    candidate.Alias,
                    candidate.PictureFileName,
                    candidate.PositionId,
                    candidate.PartyId
                );
            }
        }

        public async Task<bool> IsAliasExistingAsync(int electionId, string alias, CandidateModel editingCandidate)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                var row = tableAdapter.GetCandidatesByAlias(electionId, alias)
                    .SingleOrDefault();
                
                if (editingCandidate == null)
                {
                    return row != null;
                }
                else
                {
                    return row != null && row.ID != editingCandidate.Id;
                }
            }
        }

        public async Task UpdateCandidateAsync(CandidateModel candidate)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                tableAdapter.Update(
                    candidate.FirstName,
                    candidate.MiddleName,
                    candidate.LastName,
                    candidate.Suffix,
                    (int)candidate.Sex,
                    candidate.Birthdate,
                    candidate.YearLevel,
                    candidate.Section,
                    candidate.Alias,
                    candidate.PictureFileName,
                    candidate.PositionId,
                    candidate.PartyId,
                    candidate.Id
                );
            }
        }

        public async Task DeleteCandidateAsync(CandidateModel candidate)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                tableAdapter.Delete(
                    candidate.Id
                );
            }
        }

        public async Task<int> GetCandidatesCount(int electionId)
        {
            await Task.CompletedTask;

            using (var tableAdapter = new CandidateTableAdapter())
            {
                var objCount = tableAdapter.CountCandidateQuery(electionId) ?? "0";

                return Convert.ToInt32(objCount);
            }
        }

        public async Task InsertCandidatesAsync(IEnumerable<CandidateModel> candidates)
        {
            await Task.CompletedTask;

            using (var dataSet = new StudentElectionDataSet())
            {
                using (var manager = new TableAdapterManager())
                {
                    manager.CandidateTableAdapter = new CandidateTableAdapter();

                    var id = -1;
                    foreach (var candidate in candidates)
                    {
                        var newRow = dataSet.Candidate.NewCandidateRow();
                        Mapper.Map(candidate, newRow);

                        newRow.ID = id;
                        if (candidate.Birthdate == null)
                        {
                            newRow.SetBirthdateNull();
                        }

                        dataSet.Candidate.AddCandidateRow(newRow);

                        id--;
                    }

                    manager.UpdateAll(dataSet);
                }
            }
        }
    }
}
