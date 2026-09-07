using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.API.Features.Analytics.GetDailyMontlyActiveUsers;
using Telemetry.Read.Domain.Abstractions.Enrichers;

namespace Telemetry.Read.API.Features.Analytics.GetDailyMontlyActiveUsers.Enrichers;

public sealed class DauMauActivityEnricher : IActivityEnricher<GetDauMauQuery>
{
    public void Enrich(Activity activity, GetDauMauQuery query)
    {
        activity?.SetTag(OtelTagConstants.ProjectId, query.ProjectId.ToString());
    }
}
