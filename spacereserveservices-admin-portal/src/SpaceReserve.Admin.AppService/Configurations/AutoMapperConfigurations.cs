using AutoMapper;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.AppService.Enums;
using SpaceReserve.Infrastructure.Entities;

public class AutoMapperConfigurations : Profile
{
    public AutoMapperConfigurations()
    {
        CreateMap<NotificationDto, NotificationModel>().ReverseMap();


        CreateMap<User, UserDto>()
        .ForMember(dest => dest.ProfilePictureFileString, opt => opt.MapFrom(src => src.ProfileImage))
        .ReverseMap();

        CreateMap<SeatDto, Seat>().ReverseMap()
            .ForMember(dest => dest.SeatId, opt => opt.MapFrom(src => src.SeatId))
            .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
            .ReverseMap();

        CreateMap<DesignationModelDto, DesignationModel>()
            .ForMember(dest => dest.DesignationId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.Name))
            .ReverseMap();

        CreateMap<WorkingDayDto, WorkingDay>()
            .ForMember(w => w.WorkingDayId, opt => opt.MapFrom(cd => cd.Id))
            .ForMember(w => w.WorkDay, opt => opt.MapFrom(cd => cd.Day))
            .ReverseMap();

        CreateMap<ModeOfWorkDto, ModeOfWorkModel>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ModeOfWorkId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ModeOfWork))
            .ReverseMap();

        CreateMap<ColumnDto, ColumnModel>()
            .ForMember(c => c.ColumnId, opt => opt.MapFrom(cd => cd.Id))
            .ForMember(c => c.Column, opt => opt.MapFrom(cd => cd.Name))
            .ReverseMap();

        CreateMap<CityModelDto, CityModel>()
            .ForMember(c => c.CityId, opt => opt.MapFrom(cd => cd.Id))
            .ForMember(c => c.City, opt => opt.MapFrom(cd => cd.Name))
            .ReverseMap();

        CreateMap<FloorDto, FloorModel>()
            .ForMember(f => f.FloorId, opt => opt.MapFrom(fd => fd.Id))
            .ForMember(f => f.Floor, opt => opt.MapFrom(fd => fd.Name))
            .ReverseMap();

        CreateMap<RegisteredUserDto, User>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.fullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.DesignationModel, opt => opt.MapFrom(src => new DesignationModel { Designation = src.Designation! }))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Status))
            .ReverseMap();

        CreateMap<Booking,BookingHistoryDto>()
            .ForMember(dest=>dest.RequestId, opt => opt.MapFrom(src => src.BookingId))
            .ForMember(dest=>dest.Name, opt => opt.MapFrom(src => src.User!.FirstName+" "+src.User!.LastName))
            .ForMember(dest=>dest.Email, opt => opt.MapFrom(src => src.User!.Email))
            .ForMember(dest=>dest.RequestedDate, opt => opt.MapFrom(src => src.CreatedDate.ToString("MM/dd/yyyy")))
            .ForMember(dest=>dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate.ToString("MM/dd/yyyy")))
            .ForMember(dest=>dest.Floor, opt => opt.MapFrom(src => src!.Seat!.ColumnModel!.FloorModel!.Floor))
            .ForMember(dest=>dest.DeskNumber, opt => opt.MapFrom(src => src.Seat!.ColumnModel!.Column+""+src.Seat!.SeatNumber))
            .ForMember(dest=>dest.RequestStatus, opt => opt.MapFrom(src => src.BookingStatusModel!.BookingStatusId))
            .ReverseMap();

       CreateMap<UserBookingHistoryDto, Booking>().ReverseMap()
            .ForMember(dto => dto.RequestId, b => b.MapFrom(src => src.BookingId))
            .ForMember(dto => dto.Name, b => b.MapFrom(src => src.User!.FirstName + " " + src.User.LastName))
            .ForMember(dto => dto.BookingDate, b => b.MapFrom(src => src.BookingDate.ToString("MM/dd/yyyy")))
            .ForMember(dto => dto.RequestedDate, b => b.MapFrom(src => src.CreatedDate.ToString("MM/dd/yyyy")))
            .ForMember(dto => dto.Email, b => b.MapFrom(src => src.User!.Email))
            .ForMember(dto => dto.Floor, b => b.MapFrom(src => src.Seat!.ColumnModel!.FloorModel!.Floor))
            .ForMember(dto => dto.DeskNumber, b => b.MapFrom(src => src.Seat!.ColumnModel!.Column + "" + src.Seat!.SeatNumber))
            .ForMember(dto => dto.RequestStatus, b => b.MapFrom(src => src.BookingStatusModel!.BookingStatusId));

        CreateMap<SeatResponseDto, Seat>().ReverseMap()
            .ForMember(dest => dest.SeatId, opt => opt.MapFrom(src => src.SeatId))
            .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
            .ForMember(dest => dest.ColumnName, opt => opt.MapFrom(src => src.ColumnModel!.Column))
            .ForMember(dest => dest.ColumnId, opt => opt.MapFrom(src => src.ColumnModel!.ColumnId))
            .ReverseMap();

    }
}




