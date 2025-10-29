﻿using ServiceUsers.Application.DTOs;

namespace ServiceUsers.Application.Facade
{
    public interface IUserFacade
    {
        Task<UserReadDto> CreateUserAsync(UserCreateDto dto, CancellationToken ct = default);
        Task<AuthTokenDto?> LoginAsync(AuthRequestDto req, CancellationToken ct = default);
        Task<IReadOnlyList<UserReadDto>> GetAllAsync(CancellationToken ct = default);
    }
}
