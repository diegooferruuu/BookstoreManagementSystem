using BookstoreManagementSystem.Application.DTOs;

namespace BookstoreManagementSystem.Domain.Interfaces
{
    public interface IJwtAuthService
    {
        Task<Result<AuthTokenDto>> SignInAsync(AuthRequestDto req, CancellationToken ct = default);
    }

    public readonly struct Result<T>
    {
        public bool Success { get; }
        public T? Value { get; }
        public string? Error { get; }
        private Result(bool success, T? value, string? error)
        {
            Success = success; Value = value; Error = error;
        }
        public static Result<T> Ok(T value) => new(true, value, null);
        public static Result<T> Fail(string error) => new(false, default, error);
    }
}
