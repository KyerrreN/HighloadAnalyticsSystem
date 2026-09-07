using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.API.Features.Analytics.GetEventsVolume;
using Telemetry.Read.Domain.Abstractions.Enrichers;

namespace Telemetry.Read.API.Features.Analytics.GetEventsVolume.Enrichers;

public sealed class EventsActivityEnricher : IActivityEnricher<GetEventsVolumeQuery>
{
    public void Enrich(Activity activity, GetEventsVolumeQuery query)
    {
        activity?.SetTag(OtelTagConstants.ProjectId, query.ProjectId.ToString());
    }
}
