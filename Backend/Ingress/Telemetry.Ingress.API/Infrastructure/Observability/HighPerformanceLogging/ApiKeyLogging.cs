using Grpc.Core;

namespace Telemetry.Ingress.API.Infrastructure.Observability.HighPerformanceLogging;

public static partial class ApiKeyLogging
{
    [LoggerMessage(
        EventId = LoggingEventIdConstants.UserManagementGrpcError,
        Level = LogLevel.Error,
        Message = "gRPC call to UserManagement failed with status: {status}")]
    public static partial void LogUserManagementGrpcError(this ILogger logger, Status status, Exception ex);
}
