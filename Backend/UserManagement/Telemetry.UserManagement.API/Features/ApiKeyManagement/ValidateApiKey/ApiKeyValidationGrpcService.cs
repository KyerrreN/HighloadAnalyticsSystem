using Grpc.Core;
using Telemetry.Contracts.Grpc;

namespace Telemetry.UserManagement.API.Features.ApiKeyManagement.ValidateApiKey;

public class ApiKeyValidationGrpcService : ApiKeyValidation.ApiKeyValidationBase
{
    private readonly IApiKeyManagementService _service;
    private readonly ILogger<ApiKeyValidationGrpcService> _logger;

    public ApiKeyValidationGrpcService(IApiKeyManagementService service, ILogger<ApiKeyValidationGrpcService> logger)
    {
        _service = service;
        _logger = logger;
    }

    public override async Task<ValidateApiKeyResponse> ValidateApiKey(ValidateApiKeyRequest request, ServerCallContext context)
    {
        var result = await _service.ValidateApiKeyHashAsync(request.KeyHash, context.CancellationToken);

        if (result.IsFailure)
        {
            _logger.LogWarning("gRPC API Key validation failed: {ErrorCode} - {ErrorMessage}",
                result.Error.Code,
                result.Error.Message); // todo: high-performance logging

            return new ValidateApiKeyResponse
            {
                IsValid = false,
                ProjectId = string.Empty
            };
        }

        return new ValidateApiKeyResponse
        {
            IsValid = true,
            ProjectId = result.Value.ToString()
        };
    }
}
