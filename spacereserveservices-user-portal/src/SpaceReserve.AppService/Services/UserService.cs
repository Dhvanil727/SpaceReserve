using AutoMapper;
using SpaceReserve.AppService.Contracts;
using SpaceReserve.AppService.DTOs;
using SpaceReserve.Infrastructure.Contracts;
using SpaceReserve.Infrastructure.Entities;

namespace SpaceReserve.AppService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }
    public async Task FindOrCreateUserAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetBySubjectIdAsync(loginDto.SubjectId);
        var userEmails = await _userRepository.GetAllEmails();

        if (user == null && !userEmails.Contains(loginDto.Email))
        {
            var newUser = _mapper.Map<User>(loginDto);
            await _userRepository.AddUserAsync(newUser);
        }
        
    }
}
