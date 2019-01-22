using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.Repository.Interfaces
{
    public interface IVoterRepository
    {
        Task<VoterModel> GetVoterAsync(int voterId);
        Task<IEnumerable<VoterModel>> GetVotersAsync(int electionId);
        Task<VoterModel> GetVoterByVinAsync(int electionId, string vin);
        Task<bool> IsVinExistingAsync(int electionId, string vin, VoterModel voter);
        Task InsertVoterAsync(VoterModel voter);
        Task UpdateVoterAsync(VoterModel voter);
        Task DeleteVoterAsync(VoterModel voter);
    }
}
