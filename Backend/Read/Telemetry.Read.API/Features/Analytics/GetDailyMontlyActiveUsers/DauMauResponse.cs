namespace Telemetry.Read.API.Features.Analytics.GetDailyMontlyActiveUsers;

public sealed record DauMauResponse(DateTime Date, long UniqueUsers);
