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

        //public async Task<int> GetCandidatesCount(int electionId)
        //{
        //    return await _voterRepository.G(electionId);
        //}

        public async Task<VoterModel> GetVoterAsync(int voterId)
        {
            return await _voterRepository.GetVoterAsync(voterId);
        }

        public async Task<VoterModel> GetVoterByVinAsync(int electionId, string vin)
        {
            return await _voterRepository.GetVoterByVinAsync(electionId, vin);
        }

        public async Task<IEnumerable<VoterModel>> GetVotersAsync(int electionId)
        {
            return await _voterRepository.GetVotersAsync(electionId);
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
    }
}
