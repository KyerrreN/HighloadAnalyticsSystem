using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Abstractions.Enrichers;
using Telemetry.Read.Domain.Utils;

namespace Telemetry.Read.API.Features.GetTopEvents.Enrichers;

public class TopEventsActivityEnricher : IActivityEnricher<GetTopEventsQuery>
{
    public void Enrich(Activity activity, GetTopEventsQuery query)
    {
        var hashed = HashUtils.HashApiKey(query.ProjectApiKey);

        activity?.SetTag(OtelTagConstants.ProjectApiKeyHash, hashed);
    }
}
