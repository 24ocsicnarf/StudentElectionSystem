using AutoMapper;
using StudentElection.PostgreSQL.Model;
using StudentElection.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentElection.PostgreSQL.AutoMapper
{
    public class PostgreSQLProfile : Profile
    {
        public PostgreSQLProfile()
        {
            CreateMap<User, UserModel>()
                .ReverseMap()
                .ForMember(m => m.Type, o => o.MapFrom(s => Enum.GetName(typeof(UserType), s.Type)));

            CreateMap<ElectionModel, Election>()
                .ReverseMap();

            CreateMap<PositionModel, Position>()
                .ReverseMap();
        }
    }
}
