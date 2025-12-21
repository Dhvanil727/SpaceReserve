using AutoMapper;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.Infrastructure.Contracts;
using SpaceReserve.Admin.AppService.Enums;
using static SpaceReserve.Admin.Utility.Resources.SeatConfigurationResources.SeatStatus;
using static SpaceReserve.Admin.Utility.Resources.SeatConfigurationResources.ModeOfWorkStatus;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Admin.Utility.Resources;
using SpaceReserve.AppService.Services;

namespace SpaceReserve.Admin.AppService.Services;

public class SeatConfigurationService : ISeatConfigurationService
{
    private readonly ISeatConfigurationRepository _seatConfigurationRepository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    public SeatConfigurationService(ISeatConfigurationRepository seatConfigurationRepository, IEmailService emailService, IMapper mapper)
    {
        _seatConfigurationRepository = seatConfigurationRepository;
        _emailService = emailService;
        _mapper = mapper;
    }
    public async Task<List<(byte columnId, int seatCount, List<byte> seatNumbers)>> GetSeatCountAsync(List<byte> columnId)
    {
        var seatCount = await _seatConfigurationRepository.GetSeatCountAsync(columnId);
        return seatCount;
    }
    public async Task AddSeatAsync(List<AddSeatRequestDto> addSeatRequestDto, int userId)
    {
        var listOfColumns = addSeatRequestDto
                                .GroupBy(s => s.ColumnId)
                                .Select(g => g.Key)
                                .ToList();

        if (listOfColumns.Count > 0)
        {
            var seatCountByColumn = await _seatConfigurationRepository.GetSeatCountAsync(listOfColumns);
            var seatsToAdd = new List<Seat>();

            foreach (var column in seatCountByColumn)
            {
                if (column.seatCount >= 15)
                {
                    throw new ArgumentException(CommonResources.SeatCount);
                }
                else
                {
                    var seatsToAddInColumn = addSeatRequestDto.Where(s => s.ColumnId == column.columnId).ToList();
                    var alreadySeatNumbersInColumn = column.seatNumbers.OrderBy(n => n).ToList();
                    var maxSeatNumber = alreadySeatNumbersInColumn.Count > 0 ? alreadySeatNumbersInColumn.Max() : 0;
                    for (int i = 0; i < seatsToAddInColumn.Count; i++)
                    {
                        var nextSeat = maxSeatNumber + 1 + i;
                        if (seatsToAddInColumn[i].SeatNumber != nextSeat)
                        {
                            throw new ArgumentException(CommonResources.SeatSequence);
                        }
                        seatsToAdd.Add(new Seat
                        {
                            ColumnId = column.columnId,
                            SeatNumber = seatsToAddInColumn[i].SeatNumber,
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now
                        });
                    }
                }
            }
            await _seatConfigurationRepository.AddSeatAsync(seatsToAdd);

        }
    }
    public async Task<User?> GetUserByIdAsync(string subjectId)
    {
        return await _seatConfigurationRepository.GetUserByIdAsync(subjectId);
    }
    public async Task<GetSeatConfigurationResponseDto> GetSeatsByFloorIdAsync(byte cityId, byte floorId)
    {
        var seats = await _seatConfigurationRepository.GetSeatsByFloorIdAsync(cityId, floorId);
        var result = new GetSeatConfigurationResponseDto();
        foreach (var seat in seats)
        {

            if (seat.IsUnderMaintenance)
            {
                result.UnavailableSeat.Add(_mapper.Map<SeatResponseDto>(seat));
            }
            else if (seat.SeatConfigurations!.Any(sc => sc.User!.DesignationModel != null &&
                                                        Enum.IsDefined(typeof(ReservedDesignations), sc.User.DesignationModel.DesignationId)))
            {
                result.ReservedSeat.Add(_mapper.Map<SeatResponseDto>(seat));
            }
            else if (seat.Bookings!.Any(b => b.BookingDate == DateOnly.FromDateTime(DateTime.Today) && b.BookingStatusId == 2))
            {
                result.BookedSeat.Add(_mapper.Map<SeatResponseDto>(seat));
            }
            else if (seat.SeatConfigurations!.Any(sc => sc.User!.DesignationModel != null &&
                                                        !Enum.IsDefined(typeof(ReservedDesignations), sc.User.DesignationModel.DesignationId)))
            {
                var config = seat.SeatConfigurations!.FirstOrDefault(sc => sc.DeletedDate == null);
                if (config?.User?.ModeOfWorkModel?.ModeOfWork == Regular)
                {
                    result.BookedSeat.Add(_mapper.Map<SeatResponseDto>(seat));
                }
                else if (config?.User?.ModeOfWorkModel?.ModeOfWork == Hybrid)
                {
                    var isWorkingToday = config.User.UserWorkingDays?.Any(uwd =>
                        uwd.WorkingDayId == (byte)DateTime.Today.DayOfWeek && uwd.DeletedDate == null) == true;

                    if (isWorkingToday)
                    {
                        result.BookedSeat.Add(_mapper.Map<SeatResponseDto>(seat));
                    }
                    else
                    {
                        result.AvailableforBookingSeat.Add(_mapper.Map<SeatResponseDto>(seat));
                    }
                }
            }

            else
            {
                result.UnallocatedSeat.Add(_mapper.Map<SeatResponseDto>(seat));
            }

        }
        return result;
    }
    public async Task<string> UpdateSeatStatusAsync(string subjectId, SeatStatusUpdateDto seatStatusUpdateDto)
    {
        var adminId = await _seatConfigurationRepository.GetAdminIdAsync(subjectId);
        if (adminId == 0)
        {
            throw new ArgumentException(CommonResources.AdminNotFound);
        }
        var seat = await _seatConfigurationRepository.GetSeatByIdAsync(seatStatusUpdateDto.seatId);

        if (seatStatusUpdateDto.unAssigned && seatStatusUpdateDto.isAvailable)
        {
            await EmailConfiguration(subjectId,seatStatusUpdateDto,CommonResources.UnAssigned,CommonResources.unassigned,adminId,EmailHelper.UnAssignedUser);
            var UnassignSeat = await _seatConfigurationRepository.GetSeatByIdAsync(seatStatusUpdateDto.seatId);
            if (UnassignSeat != null)
            {
                if (UnassignSeat.IsUnderMaintenance)
                {
                    UnassignSeat.IsUnderMaintenance = false;
                    UnassignSeat.ModifiedBy = adminId;
                    UnassignSeat.ModifiedDate = DateTime.Now;
                }
            }
            await _seatConfigurationRepository.UpdateSeatAsync(seat!);
            return CommonResources.SeatStatusChanged + EmailHelper.UnAssigned;
        }
        else if (!seatStatusUpdateDto.isAvailable && !seatStatusUpdateDto.unAssigned)
        {
            await EmailConfiguration(subjectId,seatStatusUpdateDto,CommonResources.UnAvailable,CommonResources.unavailable,adminId,EmailHelper.UnAvailableUser);
            seat!.IsUnderMaintenance = true;
            seat.ModifiedBy = adminId;
            seat.ModifiedDate = DateTime.Now;
            await _seatConfigurationRepository.UpdateSeatAsync(seat!);
            return CommonResources.SeatStatusChanged + EmailHelper.UnAvailable;
        }
        return CommonResources.SeatStatusNotChanged;
    }
    private async Task EmailConfiguration(string subjectId,SeatStatusUpdateDto seatStatusUpdateDto,string EmailMessage,string MessageBy,int adminId,string EmailSubject)
    {
        var config = await _seatConfigurationRepository.GetSeatConfigurationBySeatIdAsync(seatStatusUpdateDto.seatId);
        if (config != null)
        {
            await _seatConfigurationRepository.RemoveSeatConfigurationAsync(subjectId, config);
            await _emailService.SendEmailAsync(new EmailDto
            {
                Subject = EmailSubject,
                Body = EmailHelper.UnAssignedUserEmail(config!.User!.FirstName,
                config!.User!.LastName,
                config!.DeletedDate,
                config!.Seat!.ColumnModel!.FloorModel!.CityModel!.City,
                config.Seat.ColumnModel.FloorModel.Floor,
                config!.Seat.ColumnModel.Column + "" + config!.Seat.SeatNumber,
                EmailMessage,MessageBy),
                ToUserId = [config!.User!.UserId]
            });
        }
        var bookings = await _seatConfigurationRepository.GetBookingsForSeatOnDateAsync(seatStatusUpdateDto.seatId);
        if (bookings.Any())
        {
            foreach (var booking in bookings)
            {
                booking.BookingStatusId = (byte)CommonResources.BookingStatus.Cancelled;
                booking.ModifiedBy = adminId;
                booking.ModifiedDate = DateTime.Now;
                await _seatConfigurationRepository.UpdateBookingAsync(booking);
                await _emailService.SendEmailAsync(new EmailDto
                {
                    Subject = EmailHelper.CancelledUser,
                    Body = EmailHelper.CancelledUserEmail(booking.User!.FirstName,
                    booking.User!.LastName,
                    booking.ModifiedDate,
                    booking.Seat!.ColumnModel!.FloorModel!.CityModel!.City,
                    booking.Seat.ColumnModel.FloorModel.Floor,
                    booking!.Seat.ColumnModel.Column + "" + booking.Seat.SeatNumber,
                    MessageBy),
                    ToUserId = [booking.UserId]
                });
            }
        }
    }
}




