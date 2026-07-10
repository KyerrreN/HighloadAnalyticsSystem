using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Abstractions.Enrichers;
using Telemetry.Read.Domain.Utils;

namespace Telemetry.Read.API.Features.GetEventsVolume.Enrichers;

public sealed class EventsActivityEnricher : IActivityEnricher<GetEventsVolumeQuery>
{
    public void Enrich(Activity activity, GetEventsVolumeQuery query)
    {
        var hashed = HashUtils.HashApiKey(query.ProjectApiKey);

        activity?.SetTag(OtelTagConstants.ProjectApiKeyHash, hashed);
    }
}
