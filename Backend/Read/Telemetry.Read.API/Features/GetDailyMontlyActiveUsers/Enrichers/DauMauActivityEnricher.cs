using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Abstractions.Enrichers;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers.Enrichers;

public sealed class DauMauActivityEnricher : IActivityEnricher<GetDauMauQuery>
{
    public void Enrich(Activity activity, GetDauMauQuery query)
    {
        activity?.SetTag(OtelTagConstants.ProjectId, query.ProjectId.ToString());
    }
}
