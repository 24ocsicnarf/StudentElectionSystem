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
        private readonly IPositionRepository _positionRepository;

        public CandidateService()
        {
            _candidateRepository = RepositoryFactory.Get<ICandidateRepository>();
            _positionRepository = RepositoryFactory.Get<IPositionRepository>();
        }

        public async Task<int> GetCandidatesCount(int electionId)
        {
            return await _candidateRepository.GetCandidatesCountAsync(electionId);
        }

        public async Task<IEnumerable<CandidateModel>> GetCandidatesByPartyAsync(int partyId)
        {
            return await _candidateRepository.GetCandidateDetailsListByPartyAsync(partyId);
        }

        public async Task<IEnumerable<CandidateModel>> GetCandidatesByPositionAsync(int positionId)
        {
            return await _candidateRepository.GetCandidateDetailsListByPositionAsync(positionId);
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

        public async Task ImportCandidatesAsync(IEnumerable<CandidateModel> candidates)
        {
            await _candidateRepository.InsertCandidatesAsync(candidates);
        }

        public async Task ValidateAsync(int electionId, CandidateModel candidate)
        {
            if (candidate.FirstName.IsBlank())
            {
                throw new ArgumentException("No first name provided");
            }

            if (candidate.LastName.IsBlank())
            {
                throw new ArgumentException("No last name provided");
            }

            if (candidate.YearLevel < 1 || candidate.YearLevel > 12)
            {
                throw new ArgumentOutOfRangeException("Year level must be from 1 to 12");
            }

            if (candidate.Section.IsBlank())
            {
                throw new ArgumentException("No section provided");
            }

            if (candidate.Alias.IsBlank())
            {
                throw new ArgumentException("No alias provided");
            }

            if (await IsAliasExistingAsync(electionId, candidate.Alias, null))
            {
                throw new InvalidOperationException($"Alias '{ candidate.Alias }' already exists");
            }
        }
    }
}
