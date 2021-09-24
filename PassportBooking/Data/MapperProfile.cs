using AutoMapper;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PassportBooking.Areas.Identity.Pages.Account;
using PassportBooking.Models.Entities;
using PassportBooking.Models.ViewModels.GymClass;

namespace PassportBooking.Data
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {

            CreateMap<GymClass, GymClassIndexViewModel>()
                .ForMember(
                        dest => dest.AttendingMembersCount,
                        from => from.MapFrom(s => s.AttendingMembers.Count));
        }
    }
}
