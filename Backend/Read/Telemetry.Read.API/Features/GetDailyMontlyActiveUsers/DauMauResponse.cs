namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers;

public sealed record DauMauResponse(DateTime Date, long UniqueUsers);
