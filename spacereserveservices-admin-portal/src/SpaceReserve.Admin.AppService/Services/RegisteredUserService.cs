using System.Runtime.Versioning;
using AutoMapper;
using Hangfire;
using Hangfire.States;
using SpaceReserve.Admin.AppService.Contracts;
using SpaceReserve.Admin.AppService.DTOs;
using SpaceReserve.Admin.AppService.Enums;
using SpaceReserve.Admin.Infrastructure.Contracts;
using static SpaceReserve.Admin.Utility.Resources.EmailHelper;
using SpaceReserve.AppService.Services;
using SpaceReserve.Infrastructure.Entities;
using SpaceReserve.Admin.Utility.Resources;

namespace SpaceReserve.Admin.AppService.Services;

public class RegisteredUserService : IRegisteredUserService
{
    private readonly IRegisteredUserRepository _registeredUserRepository;
    private readonly IKeyCloakAppService _keyCloakAppService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public RegisteredUserService(IRegisteredUserRepository registeredUserRepository, IKeyCloakAppService keyCloakAppService, IMapper mapper, IEmailService emailService)
    {
        _keyCloakAppService = keyCloakAppService;
        _registeredUserRepository = registeredUserRepository;
        _mapper = mapper;
        _emailService = emailService;
    }

    public async Task<List<UserBookingHistoryDto>> GetAllBookingHistoryOfUserAsync(UserBookingHistorySortDto userBookingHistorySortDto, int userId)
    {
        int sort = userBookingHistorySortDto.Sort;

        int pageNo = userBookingHistorySortDto.PageNo;
        int pageSize = userBookingHistorySortDto.PageSize;
        var bookingHistory = await _registeredUserRepository.GetAllBookingHistoryOfUserAsync(pageNo, pageSize, userId);

        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var threeMonthsFromNow = currentDate.AddMonths(3);

        if (sort == Convert.ToInt32(BookingFilter.Past))
        {
            bookingHistory = await _registeredUserRepository.GetPastBookingHistoryOfUserAsync(pageNo,pageSize,userId);
        }
        else if (sort == Convert.ToInt32(BookingFilter.Upcoming))
        {
           bookingHistory=await _registeredUserRepository.GetUpcomingBookingHistoryOfUserAsync(pageNo,pageSize,userId);
        }
      
        var bookingHistoryDto = _mapper.Map<List<UserBookingHistoryDto>>(bookingHistory);
        return bookingHistoryDto;
    }

    public async Task<UserDto?> GetUserProfileBySubjectIdAsync(int userId)
    {
        var user = await _registeredUserRepository.GetUserProfileBySubjectIdAsync(userId);

        if (user == null)
        {
            return null;
        }

        var userDto = _mapper.Map<UserDto>(user);

        var seatConfig = await _registeredUserRepository.GetSeatConfigurationByUserIdAsync(user.UserId);
        if (seatConfig != null)
        {
            var seats = await _registeredUserRepository.GetSeatByIdAsync(seatConfig.SeatId);
            if (seats != null)
            {
                userDto.Seat = _mapper.Map<SeatDto>(seats);
                var columns = await _registeredUserRepository.GetColumnByIdAsync(seats.ColumnId);
                if (columns != null)
                {
                    userDto.Column = _mapper.Map<ColumnDto>(columns);
                    var floors = await _registeredUserRepository.GetFloorByIdAsync(columns.FloorId);
                    if (floors != null)
                    {
                        userDto.Floor = _mapper.Map<FloorDto>(floors);
                    }
                }
            }
        }
        userDto.Designation = _mapper.Map<DesignationModelDto>(user.DesignationModel);
        userDto.ModeOfWork = _mapper.Map<ModeOfWorkDto>(user.ModeOfWorkModel);
        userDto.City = _mapper.Map<CityModelDto>(user.CityModel);

        userDto.Days = user.UserWorkingDays
            .Where(uw => uw.DeletedDate == null)
            .Select(uw => new WorkingDayDto
            {
                Id = uw.WorkingDayId,
                Day = uw.WorkingDay!.WorkDay
            })
            .OrderBy(w => w.Id)
            .ToList();
        if (userDto.Seat != null)
        {
            userDto.Seat.Booked = true;
        }
        userDto.Active = user.IsActive;
        return userDto;
    }


    public async Task<IEnumerable<RegisteredUserDto>> GetAllUsersAsync(int pageNo, int pageSize)
    {
        var users = await _registeredUserRepository.GetAllUsersAsync(pageNo, pageSize);
        _mapper.Map<IEnumerable<RegisteredUserDto>>(users);
        return users.Select(user => new RegisteredUserDto
        {
            EmployeeId = user.UserId,
            SubjectId = user.SubjectId,
            fullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email,
            Designation = user.DesignationModel?.Designation,
            Status = user.IsActive,
        });
    }

    public async Task<string> UpdateUserStatusAsync(List<UpdateUserStatusDto> updateUserStatusDto, string adminSubjectId)
    {
        using var transaction = await _registeredUserRepository.BeginTransactionAndRollbackAsync();
        var previousStatuses = new Dictionary<string, bool>();
        try
        {
            var admin = await GetAdminBySubjectIdAsync(adminSubjectId);
            foreach (var dto in updateUserStatusDto)
            {
                var previousStatus = await _keyCloakAppService.GetUserEnabledStatusAsync(dto.SubjectId);
                previousStatuses[dto.SubjectId] = previousStatus;

                var user = await _registeredUserRepository.GetUserByIdAsync(dto.UserId);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User not found for Id {dto.UserId}");
                }
                if (!string.Equals(user.SubjectId, dto.SubjectId, StringComparison.Ordinal))
                {
                    throw new ArgumentException($"Mismatch: SubjectId does not belong to the specified User Id {dto.UserId}");
                }
                await _keyCloakAppService.UpdateUserStatusInKeycloakAsync(dto.SubjectId, dto.IsActive);

                if (user != null)
                {
                    if (!dto.IsActive)
                    {
                        await SoftDeleteRelatedEntitiesAsync(dto.UserId, admin.UserId);
                    }
                    if(dto.IsActive && !user.IsActive)
                    {
                        await RestoreSoftDeleteUser(dto.UserId, admin.UserId);
                    }
                    user.IsActive = dto.IsActive;
                    await _registeredUserRepository.UpdateUserAsync(user);

                    if (!dto.IsActive)
                    {
                        await SendEmail("Your Access to Space Reserve Has Been Deactivated", user.FirstName + " " + user.LastName , user.UserId);
                    }
                }
            }
            await transaction.CommitAsync();
            return RegisteredUserCommonResources.UpdateUserStatus;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            foreach (var dto in updateUserStatusDto)
            {
                if (previousStatuses.TryGetValue(dto.SubjectId, out var previousStatus))
                {
                    await _keyCloakAppService.UpdateUserStatusInKeycloakAsync(dto.SubjectId, previousStatus);
                }
            }
            var rollbackMessage = $"Transaction rolled back due to an error: {ex.Message}";
            throw new Exception(rollbackMessage, ex);
        }
    }

    public async Task<User?> GetAdminBySubjectIdAsync(string adminSubjectId)
    {
        return await _registeredUserRepository.GetAdminBySubjectIdAsync(adminSubjectId);
    }

    private async Task SoftDeleteRelatedEntitiesAsync(int userId, int adminId)
    {
        await SoftDeleteUser(userId, adminId);
        await SoftDeleteBookings(userId, adminId);
        await SoftDeleteSeatConfigurations(userId, adminId);
        await SoftDeleteUserWorkingDays(userId, adminId);
        var seatIdConfiguration = await _registeredUserRepository.GetSeatIdOfUserAsync(userId);
        if (seatIdConfiguration != null)
        {
            var seatId = seatIdConfiguration.SeatId;
            await SoftDeleteAssignedBooking(seatId, adminId);
        }
        await _registeredUserRepository.SaveChangesAsync();
    }
    private async Task SoftDeleteBookings(int userId, int adminId)
    {
        var bookings = await _registeredUserRepository.GetBookingsByUserId(userId);
        foreach (var booking in bookings)
        {
            if (booking.DeletedBy == null && booking.DeletedDate == null)
            {
                booking.DeletedBy = adminId;
                booking.DeletedDate = DateTime.Now;
            }
        }
    }
    private async Task SoftDeleteAssignedBooking(int seatId, int adminId)
    {
        var bookings = await _registeredUserRepository.GetAllBookingsOfSeatIdAsync(seatId);
        foreach (var booking in bookings)
        {
            booking.DeletedBy = adminId;
            booking.DeletedDate = DateTime.Now;
        }
    }

    private async Task SoftDeleteSeatConfigurations(int userId, int adminId)
    {
        var seatConfigurations = await _registeredUserRepository.GetSeatConfigurationsByUserId(userId);
        foreach (var seatConfiguration in seatConfigurations)
        {
            if (seatConfiguration.DeletedBy == null && seatConfiguration.DeletedDate == null)
            {
                seatConfiguration.DeletedBy = adminId;
                seatConfiguration.DeletedDate = DateTime.Now;
            }
        }
    }

    private async Task SoftDeleteUserWorkingDays(int userId, int adminId)
    {
        var userWorkingDays = await _registeredUserRepository.GetUserWorkingDaysByUserId(userId);
        foreach (var userWorkingDay in userWorkingDays)
        {
            if (userWorkingDay.DeletedBy == null && userWorkingDay.DeletedDate == null)
            {
                userWorkingDay.DeletedBy = adminId;
                userWorkingDay.DeletedDate = DateTime.Now;
            }
        }
    }

    private async Task SoftDeleteUser(int userId, int adminId)
    {
        var userUpdate = await _registeredUserRepository.GetUserByUserId(userId);
        foreach (var user in userUpdate)
        {
            user.ModifiedBy = adminId;
            user.ModifiedDate = DateTime.Now;
            user.DeletedBy = adminId;
            user.DeletedDate = DateTime.Now;
        }
    }

    private async Task RestoreSoftDeleteUser(int userId, int adminId)
    {
        var userUpdate = await _registeredUserRepository.GetUserByUserId(userId);
        foreach (var user in userUpdate)
        {
            user.ModifiedBy = adminId;
            user.ModifiedDate = DateTime.Now;
            user.DeletedBy = null;
            user.DeletedDate = null;
            user.DesignationId = null;
            user.CityId = null;
            user.ModeOfWorkId = null;
            user.PhoneNumber = null;
            user.ProfileImage = null;
            user.ProfileImageName = null;
        }
        await _registeredUserRepository.SaveChangesAsync();
    }
    private async Task<bool> SendEmail(string subject, string dear,int userId)
    {

        EmailDto emailDto = new EmailDto
        {
            Subject = $"{subject}",
            Body = EmailForCancelBooking(
                dear
                ),
            ToUserId = [userId],
        };
        try
        {
            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(emailDto));
        }
        catch (Exception ex)
        {
            return false;
        }
        return true;
    }
}