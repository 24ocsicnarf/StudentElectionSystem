using StudentElection.Repository;
using StudentElection.Repository.Interfaces;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Services
{
    public class CandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService()
        {
            _candidateRepository = RepositoryFactory.Get<ICandidateRepository>();
        }

        public async Task<int> GetCandidatesCount(int electionId)
        {
            return await _candidateRepository.GetCandidatesCount(electionId);
        }

        public async Task<IEnumerable<CandidateModel>> GetCandidatesByPartyAsync(int partyId)
        {
            return await _candidateRepository.GetCandidateDetailsListByPartyAsync(partyId);
        }

        public async Task<CandidateModel> GetCandidateAsync(int candidateId)
        {
            return await _candidateRepository.GetCandidateDetailsAsync(candidateId);
        }

        public async Task<bool> IsAliasExistingAsync(int electionId, string alias, CandidateModel editingCandidate)
        {
            return await _candidateRepository.IsAliasExistingAsync(electionId, alias, editingCandidate);
        }

        public async Task SaveCandidateAsync(CandidateModel candidate)
        {
            if (candidate.Id == 0)
            {
                await _candidateRepository.InsertCandidateAsync(candidate);
            }
            else
            {
                await _candidateRepository.UpdateCandidateAsync(candidate);
            }
        }

        public async Task DeleteCandidateAsync(CandidateModel candidate)
        {
            await _candidateRepository.DeleteCandidateAsync(candidate);
        }
    }
}
