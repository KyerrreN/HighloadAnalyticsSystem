namespace Telemetry.Ingress.API.Infrastructure.Exceptions;

public sealed class UserManagementUnavailableException(
    string message = "UserManagement service is currently unavailable.",
    Exception? innerException = null)
    : Exception(message, innerException);
