using AutoMapper;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Utility.Resources;
using static SpaceReserve.Utility.Resources.SeatConfigurationResources;
using static SpaceReserve.Utility.Resources.CommonResource;
using SpaceReserve.Utility.Enum;
namespace SpaceReserve.AppService.Services;

public class SeatBookingService : ISeatBookingService
{
    private readonly ISeatBookingRepository _seatBookingRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public SeatBookingService(ISeatBookingRepository seatBookingRepository, IUserRepository userRepository, IMapper mapper, IEmailService emailService)
    {
        _seatBookingRepository = seatBookingRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task<SeatDetailsDto?> GetSeatDetailsAsync(short seatId, DateOnly date)
    {
        var (owner, booking, numberOfBookings) = await _seatBookingRepository.GetSeatBookingDetailsAsync(seatId, date);
        var seatDetailsDto = _mapper.Map<SeatDetailsDto>(owner);
        if (seatDetailsDto == null)
            seatDetailsDto = new SeatDetailsDto();

        if (booking?.User != null)
        {
            var tempOwnerDto = _mapper.Map<TemporarySeatOwnerDto>(booking.User);
            seatDetailsDto.TemporarySeatOwnerDto = tempOwnerDto;

            
            seatDetailsDto.CountOfRequest = numberOfBookings;
        }
        else
        {
            seatDetailsDto.TemporarySeatOwnerDto = null;
            seatDetailsDto.CountOfRequest = numberOfBookings;
            if (string.IsNullOrEmpty(seatDetailsDto.Name))
            {
                var messageUnassigned = CommonResource.messageUnassigned;
                seatDetailsDto.Message = messageUnassigned;
                SetMessage(seatDetailsDto);
            }
            else if (!Enum.IsDefined(typeof(ReservedDesignations), seatDetailsDto.Designation))
            {
                if (seatDetailsDto.CountOfRequest == 0)
                {
                    var messageAvailable = CommonResource.messageAvailable;
                    seatDetailsDto.Message = messageAvailable;
                }
                SetMessage(seatDetailsDto);
            }
        }
        return seatDetailsDto;
    }
    private void SetMessage(SeatDetailsDto seatDetailsDto)
    {
        if (seatDetailsDto.CountOfRequest == (byte)CountOfRequest.One)
        {
            seatDetailsDto.Message = OneRequestPending;
        }
        else if (seatDetailsDto.CountOfRequest == (byte)CountOfRequest.Two)
        {
            seatDetailsDto.Message = TwoRequestPending;
        }
        else if (seatDetailsDto.CountOfRequest == (byte)CountOfRequest.Three)
        {
            seatDetailsDto.Message = ThreeRequestPending;
        }
    }

    public async Task<Result<string>> BookSeat(BookingDto bookingDto, string userSubId)
    {
        var seatExists = await _seatBookingRepository.SeatExistsAsync(bookingDto.SeatId);
        if (!seatExists)
        {
            return Result<string>.NotFound(ResultStatus.NotFound, CommonResource.Seat);
        }
        var status = await _seatBookingRepository.SeatAvailable(bookingDto.SeatId, bookingDto.RequestDateTime);
        if (status.seat!.IsUnderMaintenance || (status.designationId != null && Enum.IsDefined(typeof(ReservedDesignations), status.designationId)) || status.bookingStatus == (int)BookingStatus.Accepted)
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.SeatNotAvailable);
        }
        if (status.seatConfiguration != null && status.seatConfiguration.User != null && status.seatConfiguration!.User!.UserWorkingDays != null && status.seatConfiguration.User.UserWorkingDays.Any(uwd => uwd.WorkingDayId == (byte)bookingDto.RequestDateTime.DayOfWeek && uwd.DeletedDate == null))
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.SeatNotAvailable);
        }
        if (status.mode == (byte)ModeOfWork.Regular)
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.SeatNotAvailable);
        }
        var modeOfUserUser = await _seatBookingRepository.ModeOfUser(userSubId);
        if (modeOfUserUser == SeatConfigurationResources.ModeRegular)
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.RegularUser);
        }
        else if (modeOfUserUser == SeatConfigurationResources.ModeHybrid)
        {
            var daysOfWorking = await _seatBookingRepository.WorkingDaysOfHybridUser(userSubId);
            byte day = (byte)bookingDto.RequestDateTime.DayOfWeek;
            if (daysOfWorking.Contains(day))
            {
                return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.SeatAssign);
            }
        }

        var userBookingStatus = await _seatBookingRepository.CheckUserBookingStatus(userSubId, bookingDto.RequestDateTime);
        if (userBookingStatus)
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.SeatAlloted);
        }

        var (limit, seatRequestCountOfUser) = await _seatBookingRepository.CheckLimitOfUserAsync(userSubId, bookingDto.RequestDateTime, bookingDto.SeatId);
        if (limit >= (int)Number.Three)
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.MaximumLimitUser);
        }
        if (seatRequestCountOfUser >= (int)Number.One)
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.SeatRequested);
        }

        var seatCount = await _seatBookingRepository.GetSeatRequestCountAsync(bookingDto.SeatId, bookingDto.RequestDateTime);
        if (seatCount >= (int)Number.Three)
        {
            return Result<string>.ErrorMessage(ResultStatus.ErrorMessage, CommonResource.MaximumLimit);
        }

        var booking = _mapper.Map<Booking>(bookingDto);
        var userSubjectId = userSubId;
        var user = await _userRepository.GetBySubjectIdAsync(userSubjectId);
        booking.UserId = user!.UserId;
        booking.BookingStatusId = CommonResource.BookingPending;
        booking.CreatedBy = user.UserId;
        string? type = string.Empty;
        EmailDto emailDto = new EmailDto();

        var seatOwner = await _seatBookingRepository.GetSeatDetailBySeatIdAsync(booking.SeatId);
        if (seatOwner.user == null)
        {
            var checkSeatExists = await _seatBookingRepository.SeatExistsAsync(booking.SeatId);
            if (checkSeatExists)
            {
                var seatOwnerAdmin = await _seatBookingRepository.GetUnassignSeatDetailBySeatIdAsync(booking.SeatId);
                var adminDeatil = await _seatBookingRepository.GetAdminDetails();
                var adminId = Convert.ToInt16(adminDeatil?.UserId);
                type = CommonResource.isBooking;
                emailDto = new EmailDto
                {
                    Subject = EmailConstants.AdminSubject,
                    Body = EmailHelper.GenerateSeatRequestEmailBody(
                         $"{user.FirstName} {user.LastName}",
                         bookingDto.RequestDateTime,
                         seatOwnerAdmin.city,
                         seatOwnerAdmin.floor,
                         seatOwnerAdmin.seatNumber,
                         EmailConstants.DearAdminGreeting,
                         EmailConstants.AdminRequestMessage,
                         EmailConstants.AdminActionMessage
                     ),
                    ToUserId = [adminId],
                    SenderSubjectId = user.SubjectId
                };
            }
        }
        else
        {
            int seatOwnerId = Convert.ToInt16(seatOwner.seatConfiguration?.UserId);
            type = CommonResource.isRequest;
            emailDto = new EmailDto
            {
                Subject = EmailConstants.UserSubject,
                Body = EmailHelper.GenerateSeatRequestEmailBody(
                             $"{user.FirstName} {user.LastName}",
                             bookingDto.RequestDateTime,
                             seatOwner.city,
                             seatOwner.floor,
                             seatOwner.seatNumber,
                             string.Format(EmailConstants.DearUserGreeting, seatOwner.user.FirstName, seatOwner.user.LastName),
                             EmailConstants.UserRequestMessage,
                             EmailConstants.UserActionMessage
                         ),
                ToUserId = [seatOwnerId],
                SenderSubjectId = user.SubjectId
            };

        }
        var insertBooking = await _seatBookingRepository.BookSeat(booking, type);
        if (insertBooking != null)
        {
            await _emailService.SendEmailAsync(emailDto);
        }
        return Result<string>.Success(CommonResource.SeatBooked);
    }

    public async Task<List<SeatViewDto>> GetAllSeatsAsync(DateOnly date, byte cityId, byte floorId)
    {
        var seats = await _seatBookingRepository.GetAllSeatsAsync(date, cityId, floorId);
        var result = new List<SeatViewDto>();
        foreach (var seat in seats)
        {
            string status = Unassigned;
            if (seat.IsUnderMaintenance)
            {
                status = Unavailable;
            }
            else if (seat.SeatConfigurations != null && seat.SeatConfigurations.Any(sc => sc.User != null && sc.User.DesignationModel != null && Enum.IsDefined(typeof(ReservedDesignations), sc.User.DesignationModel!.DesignationId)))
            {
                status = Reserved;
            }
            else if (seat.SeatConfigurations != null && seat.SeatConfigurations.Any(sc => sc.User != null && sc.User.DesignationModel != null && !Enum.IsDefined(typeof(ReservedDesignations), sc.User.DesignationModel!.DesignationId)))
            {
                var config = seat.SeatConfigurations.First(sc => sc.User != null);
                var mode = config.User!.ModeOfWorkModel!.ModeOfWork;
                if (mode == ModeRegular)
                    status = Booked;
                else if (mode == ModeHybrid)
                {
                    if (config.User.UserWorkingDays != null && config.User.UserWorkingDays.Any(uwd => uwd.WorkingDayId == (byte)date.DayOfWeek && uwd.DeletedDate == null))
                    {
                        status = Booked;
                    }
                    else
                    {
                        status = Available;
                    }
                }
            }
            if (seat.Bookings != null && seat.Bookings.Any(b => b.BookingDate == date && b.BookingStatusId == (byte)BookingStatus.Accepted))
            {
                status = Booked;
            }
            result.Add(new SeatViewDto
            {
                Seat = new SeatDto
                {
                    Id = seat.SeatId,
                    SeatNumber = seat.SeatNumber,
                    Booked = (status == Reserved || status == Booked) ? true : false,
                },
                Column = new ColumnDto
                {
                    Id = seat.ColumnModel!.ColumnId,
                    Name = seat.ColumnModel.Column
                },
                Status = status
            });
        }
        return result;
    }

}