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
    public class VoterService
    {
        private readonly IVoterRepository _voterRepository;

        public VoterService()
        {
            _voterRepository = RepositoryFactory.Get<IVoterRepository>();
        }

        public async Task<IEnumerable<VoterModel>> GetVotersAsync(int electionId)
        {
            return await _voterRepository.GetVotersAsync(electionId);
        }

        public async Task<IEnumerable<VoterModel>> GetVoterDetailsListAsync(int electionId)
        {
            return await _voterRepository.GetVoterDetailsListAsync(electionId);
        }

        public async Task<VoterModel> GetVoterAsync(int voterId)
        {
            return await _voterRepository.GetVoterAsync(voterId);
        }

        public async Task<VoterModel> GetVoterByVinAsync(int electionId, string vin)
        {
            return await _voterRepository.GetVoterByVinAsync(electionId, vin);
        }

        public async Task<int> CountVotersAsync(int electionId)
        {
            return await _voterRepository.CountVotersAsync(electionId);
        }

        public async Task<bool> IsVinExistingAsync(int electionId, string vin, VoterModel voter)
        {
            return await _voterRepository.IsVinExistingAsync(electionId, vin, voter);
        }

        public async Task SaveVoterAsync(VoterModel voter)
        {
            if (voter.Id == 0)
            {
                await _voterRepository.InsertVoterAsync(voter);
            }
            else
            {
                await _voterRepository.UpdateVoterAsync(voter);
            }
        }

        public async Task DeleteVoterAsync(VoterModel voter)
        {
            await _voterRepository.DeleteVoterAsync(voter);
        }

        public async Task ImportVotersAsync(IEnumerable<VoterModel> voters)
        {
            await _voterRepository.InsertVotersAsync(voters);
        }

        public async Task<int> CountVotedVotersAsync(int electionId)
        {
            return await _voterRepository.CountVotedVotersAsync(electionId);
        }

        public async Task ValidateAsync(int electionId, VoterModel voter)
        {
            if (voter.FirstName.IsBlank())
            {
                throw new ArgumentException("No first name provided", nameof(voter.FirstName));
            }

            if (voter.LastName.IsBlank())
            {
                throw new ArgumentException("No last name provided", nameof(voter.LastName));
            }

            if (voter.YearLevel < 1 || voter.YearLevel > 12)
            {
                throw new ArgumentOutOfRangeException("Year level must be from 1 to 12", nameof(voter.YearLevel));
            }

            if (voter.Section.IsBlank())
            {
                throw new ArgumentException("No section provided", nameof(voter.Section));
            }

            if (await IsVinExistingAsync(electionId, voter.Vin, null))
            {
                throw new InvalidOperationException($"Voter ID '{ voter.Vin }' already exists");
            }
        }
    }
}
