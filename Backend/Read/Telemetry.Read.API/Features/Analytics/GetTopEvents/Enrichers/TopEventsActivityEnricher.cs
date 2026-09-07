using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.API.Features.Analytics.GetTopEvents;
using Telemetry.Read.Domain.Abstractions.Enrichers;

namespace Telemetry.Read.API.Features.Analytics.GetTopEvents.Enrichers;

public class TopEventsActivityEnricher : IActivityEnricher<GetTopEventsQuery>
{
    public void Enrich(Activity activity, GetTopEventsQuery query)
    {
        activity?.SetTag(OtelTagConstants.ProjectId, query.ProjectId.ToString());
    }
}
