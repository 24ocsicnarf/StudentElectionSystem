using AutoMapper;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudentElection.MSAccess.StudentElectionDataSet;

namespace StudentElection.MSAccess.AutoMapper
{
    public class MSAccessProfile : Profile
    {
        public MSAccessProfile()
        {
            MapUsers();
            MapElections();
            MapParties();
            MapPositions();
            MapCandidates();
            MapVoters();
            MapBallots();
            MapVotes();
        }

        public void MapUsers()
        {
            CreateMap<UserRow, UserModel>()
                .ForMember(m => m.Type, o => o.MapFrom(s => (UserType)s.Type));
            CreateMap<UserModel, UserRow>()
                .ForMember(m => m.Type, o => o.MapFrom(s => (int)s.Type));
        }

        public void MapElections()
        {
            CreateMap<ElectionRow, ElectionModel>()
                .ForMember(m => m.CandidatesFinalizedAt, o => o.MapFrom(s => s.IsCandidatesFinalizedAtNull() ? default(DateTime?) : s.CandidatesFinalizedAt))
                .ForMember(m => m.ClosedAt, o => o.MapFrom(s => s.IsClosedAtNull() ? default(DateTime?) : s.ClosedAt));
            CreateMap<ElectionModel, ElectionRow>();
        }

        public void MapParties()
        {
            CreateMap<PartyRow, PartyModel>();
            CreateMap<PartyModel, PartyRow>();
        }

        public void MapPositions()
        {
            CreateMap<PositionRow, PositionModel>();
            CreateMap<PositionModel, PositionRow>();
        }
        
        public void MapCandidates()
        {
            CreateMap<CandidateRow, CandidateModel>()
                .ForMember(m => m.Sex, o => o.MapFrom(s => (Sex)s.Sex))
                .ForMember(m => m.Birthdate, o => o.MapFrom(s => s.IsBirthdateNull() ? default(DateTime?) : s.Birthdate));

            CreateMap<CandidateModel, CandidateRow>()
                .ForMember(m => m.Sex, o => o.MapFrom(s => (int)s.Sex));

            CreateMap<DataRow, CandidateModel>()
                .ForMember(m => m.Id,
                    o => o.MapFrom(s => Convert.IsDBNull(s["Id"]) ? default(int?) : Convert.ToInt32(s["Id"])))
                .ForMember(m => m.FirstName,
                    o => o.MapFrom(s => Convert.IsDBNull(s["FirstName"]) ? null : Convert.ToString(s["FirstName"])))
                .ForMember(m => m.MiddleName,
                    o => o.MapFrom(s => Convert.IsDBNull(s["MiddleName"]) ? null : Convert.ToString(s["MiddleName"])))
                .ForMember(m => m.LastName,
                    o => o.MapFrom(s => Convert.IsDBNull(s["LastName"]) ? null : Convert.ToString(s["LastName"])))
                .ForMember(m => m.Suffix,
                    o => o.MapFrom(s => Convert.IsDBNull(s["Suffix"]) ? null : Convert.ToString(s["Suffix"])))
                .ForMember(m => m.Birthdate,
                    o => o.MapFrom(s => Convert.IsDBNull(s["Birthdate"]) ? default(DateTime?) : Convert.ToDateTime(s["Birthdate"])))
                .ForMember(m => m.Sex,
                    o => o.MapFrom(s => Convert.IsDBNull(s["Sex"]) ? default(Sex?) : (Sex)Convert.ToInt32(s["Sex"])))
                .ForMember(m => m.YearLevel,
                    o => o.MapFrom(s => Convert.IsDBNull(s["YearLevel"]) ? default(int?) : Convert.ToInt32(s["YearLevel"])))
                .ForMember(m => m.Section,
                    o => o.MapFrom(s => Convert.IsDBNull(s["Section"]) ? null : Convert.ToString(s["Section"])))
                .ForMember(m => m.Alias,
                    o => o.MapFrom(s => Convert.IsDBNull(s["Alias"]) ? null : Convert.ToString(s["Alias"])))
                .ForMember(m => m.PictureFileName,
                    o => o.MapFrom(s => Convert.IsDBNull(s["PictureFileName"]) ? null : Convert.ToString(s["PictureFileName"])))
                .ForMember(m => m.PartyId,
                    o => o.MapFrom(s => Convert.IsDBNull(s["PartyId"]) ? default(int?) : Convert.ToInt32(s["PartyId"])))
                .ForMember(m => m.PositionId,
                    o => o.MapFrom(s => Convert.IsDBNull(s["PositionId"]) ? default(int?) : Convert.ToInt32(s["PositionId"])))
                .ForMember(m => m.Party, o => o.MapFrom(s => new PartyModel
                {
                    Id = Convert.ToInt32(s["PartyId"]),
                    Title = Convert.ToString(s["PartyTitle"]),
                    ShortName = Convert.ToString(s["PartyShortName"]),
                    Argb = Convert.ToInt32(s["PartyArgb"]),
                    ElectionId = Convert.ToInt32(s["PartyElectionId"]),
                }))
                .ForMember(m => m.Position, o => o.MapFrom(s => new PositionModel
                {
                    Id = Convert.ToInt32(s["PositionId"]),
                    Title = Convert.ToString(s["PositionTitle"]),
                    WinnersCount = Convert.ToInt32(s["PositionWinnersCount"]),
                    Rank = Convert.ToInt32(s["PositionRank"]),
                    ElectionId = Convert.ToInt32(s["PositionElectionId"]),
                }));
        }

        public void MapVoters()
        {
            CreateMap<VoterRow, VoterModel>()
                .ForMember(m => m.Sex, o => o.MapFrom(s => (Sex)s.Sex))
                .ForMember(m => m.Birthdate, o => o.MapFrom(s => s.IsBirthdateNull() ? default(DateTime?) : s.Birthdate));

            CreateMap<VoterModel, VoterRow>()
                .ForMember(m => m.Sex, o => o.MapFrom(s => (int)s.Sex));
        }

        private void MapBallots()
        {
            CreateMap<BallotRow, BallotModel>()
                .ForMember(m => m.CastedAt, o => o.MapFrom(s => s.IsCastedAtNull() ? default(DateTime?) : s.CastedAt));

            CreateMap<BallotModel, BallotRow>();
        }

        private void MapVotes()
        {
            CreateMap<VoteRow, VoteModel>();

            CreateMap<VoteModel, VoteRow>();
        }
    }
}
