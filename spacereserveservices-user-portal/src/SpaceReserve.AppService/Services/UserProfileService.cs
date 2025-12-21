using AutoMapper;
using Microsoft.AspNetCore.Http;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.AppService.Services;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMapper _mapper;

    public UserProfileService(IUserProfileRepository userProfileRepository, IMapper mapper)
    {
        _mapper = mapper;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserDto?> GetUserProfileBySubjectIdAsync(string subjectId)
    {
        var user = await _userProfileRepository.GetUserProfileBySubjectIdAsync(subjectId);

        if (user == null )
        {
            return null;
        }
        var userDto = _mapper.Map<UserDto>(user);

        var seatConfig = await _userProfileRepository.GetSeatConfigurationByUserIdAsync(user.UserId);
        if(seatConfig != null)
        {
            var seats =await _userProfileRepository.GetSeatByIdAsync(seatConfig.SeatId);
            if(seats != null){
                userDto.Seat = _mapper.Map<SeatDto>(seats);
                var columns = await _userProfileRepository.GetColumnByIdAsync(seats.ColumnId);
                if(columns != null)
                {
                    userDto.Column = _mapper.Map<ColumnDto>(columns);
                    var floors =await _userProfileRepository.GetFloorByIdAsync(columns.FloorId);
                    if(floors != null)
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

        if(userDto.Seat != null)
        {
            userDto.Seat.Booked = true;
        }
        userDto.Active = user.IsActive;
        return userDto;
    }

    public async Task<bool?> UpdateUserProfileBySubjectIdAsync(string subjectId, UpdateProfileRequestDto updateProfileRequestDto)
    {
        if (updateProfileRequestDto.ModeOfWork != 2 && !await SeatExist((short) updateProfileRequestDto.Seat!, updateProfileRequestDto.City))
        {
            throw new ArgumentException("Seat does exists for the City you entered");
        }
        if (updateProfileRequestDto.ModeOfWork != 2 && !await IsSeatAvailable(subjectId, updateProfileRequestDto.Seat))
        {
            throw new ArgumentException("Seat is already assigned to another user.");
        }

        if (!await IsPhoneNumberAvailable(subjectId, updateProfileRequestDto.PhoneNumber))
        {
            throw new ArgumentException("This phone no. is already in use, please try a different one");
        }

        var user = _mapper.Map<User>(updateProfileRequestDto);
        if (updateProfileRequestDto.ModeOfWork == 2)
        {
            user.CityId = null;
            updateProfileRequestDto.Seat = null;
        }

        user.UserWorkingDays = updateProfileRequestDto.WorkingDays
            .Select(day => new UserWorkingDay
            {
                WorkingDayId = day,
                CreatedBy = user.UserId,
                CreatedDate = DateTime.Now,
            }).ToList();

        return await _userProfileRepository.UpdateUserProfileBySubjectIdAsync(subjectId, user, updateProfileRequestDto.Seat);
    }

    public async Task<bool> IsSeatAvailable(string subjectId, short? seatId)
    {
        var assisgnedSeats = await _userProfileRepository.GetAllAssignedSeats(subjectId);
        return !assisgnedSeats.Any(x => x == seatId);
    }

    public async Task<bool> IsPhoneNumberAvailable(string subjectId, string phoneNumber)
    {
        var phoneNumbers = await _userProfileRepository.GetAllPhoneNumbers(subjectId);
        return !phoneNumbers.Any(x => x == phoneNumber);
    }

    public async Task<bool> SeatExist(short seatId, byte cityId)
    {
        var seat = await _userProfileRepository.FindSeatBySeatIdAndCityIdAsync(seatId, cityId);
        if (seat == null)
        {
            return false;
        }
        return true;
    }
    
}