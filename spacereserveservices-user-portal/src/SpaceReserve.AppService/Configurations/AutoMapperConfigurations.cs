using AutoMapper;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.AppService.DTOs.RequestHistory;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.AppService.Configurations;

public class AutoMapperConfigurations : Profile
{
    public AutoMapperConfigurations()
    {
        CreateMap<LoginDto, User>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<DesignationModelDto, DesignationModel>();
        CreateMap<User, UpdateProfileRequestDto>()
            .ForMember(dest => dest.ProfilePictureFileString, opt => opt.MapFrom(src => src.ProfileImage))
            .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.DesignationId))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CityId))
            .ForMember(dest => dest.ModeOfWork, opt => opt.MapFrom(src => src.ModeOfWorkId))
            .ReverseMap();
        CreateMap<UserDto, User>().ReverseMap()
            .ForMember(dest => dest.ProfilePictureFileString, opt => opt.MapFrom(src => src.ProfileImage))
            .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.DesignationModel))
            .ForMember(dest => dest.ModeOfWork, opt => opt.MapFrom(src => src.ModeOfWorkModel))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.CityModel));
        CreateMap<SeatDto, Seat>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SeatId))
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

        CreateMap<SeatDto, Seat>().ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.SeatId))
            .ForMember(dest => dest.SeatNumber, opt => opt.MapFrom(src => src.SeatNumber))
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
        CreateMap<NotificationDto, NotificationModel>().ReverseMap();

        CreateMap<BookingHistoryDto, Booking>().ReverseMap()
            .ForMember(dto => dto.RequestId, b => b.MapFrom(src => src.BookingId))
            .ForMember(dto => dto.Name, b => b.MapFrom(src => src.User!.FirstName + " " + src.User.LastName))
            .ForMember(dto => dto.BookingDate, b => b.MapFrom(src => src.BookingDate.ToString("MM/dd/yyyy")))
            .ForMember(dto => dto.RequestedDate, b => b.MapFrom(src => src.CreatedDate.ToString("MM/dd/yyyy")))
            .ForMember(dto => dto.Email, b => b.MapFrom(src => src.User!.Email))
            .ForMember(dto => dto.Floor, b => b.MapFrom(src => src.Seat!.ColumnModel!.FloorModel!.Floor))
            .ForMember(dto => dto.DeskNumber, b => b.MapFrom(src => src.Seat!.ColumnModel!.Column + "" + src.Seat!.SeatNumber))
            .ForMember(dto => dto.RequestStatus, b => b.MapFrom(src => src.BookingStatusModel!.BookingStatusId));

        CreateMap<User, SeatDetailsDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.DesignationModel!.Designation))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ReverseMap();

        CreateMap<User, TemporarySeatOwnerDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Designation, opt => opt.MapFrom(src => src.DesignationModel!.Designation))
            .ReverseMap();

        CreateMap<BookingDto, Booking>().ReverseMap();

        CreateMap<Booking, RequestHistoryDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User!.FirstName + " " + src.User.LastName))
            .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src => src.CreatedDate.Date.ToString("MM/dd/yyyy")))
            .ForMember(dest => dest.RequestFor, opt => opt.MapFrom(src => src.BookingDate.ToString("MM/dd/yyyy")))
            .ForMember(dest => dest.EmailId, opt => opt.MapFrom(src => src.User!.Email))
            .ForMember(dest => dest.FloorNo, opt => opt.MapFrom(src => src.Seat!.ColumnModel!.FloorModel!.Floor))
            .ForMember(dest => dest.DeskNo, opt => opt.MapFrom(src => src.Seat!.ColumnModel!.Column + "" + src.Seat!.SeatNumber))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.BookingStatusId))
            .ForMember(dest => dest.SeatRequestId, opt => opt.MapFrom(src => src.BookingId));
            
        CreateMap<BookingDto, Booking>()
            .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.RequestDateTime))
            .ReverseMap();
    }
}




