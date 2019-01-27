using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Interfaces
{
    public interface ICandidateRepository
    {
        Task<IEnumerable<CandidateModel>> GetCandidateDetailsListByPartyAsync(int partyId);
        Task<IEnumerable<CandidateModel>> GetCandidateDetailsListByPositionAsync(int positionId);
        Task<CandidateModel> GetCandidateDetailsAsync(int candidateId);
        Task<int> GetCandidatesCountAsync(int electionId);
        Task<bool> IsAliasExistingAsync(int electionId, string alias, CandidateModel editingCandidate);
        Task InsertCandidateAsync(CandidateModel candidate);
        Task InsertCandidatesAsync(IEnumerable<CandidateModel> candidates);
        Task UpdateCandidateAsync(CandidateModel candidate);
        Task DeleteCandidateAsync(CandidateModel candidate);
    }
}
