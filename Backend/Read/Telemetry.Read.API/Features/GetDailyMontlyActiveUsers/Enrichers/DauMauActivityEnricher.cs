using System.Diagnostics;
using Telemetry.Contracts.Constants;
using Telemetry.Read.Domain.Abstractions.Enrichers;
using Telemetry.Read.Domain.Utils;

namespace Telemetry.Read.API.Features.GetDailyMontlyActiveUsers.Enrichers;

public sealed class DauMauActivityEnricher : IActivityEnricher<GetDauMauQuery>
{
    public void Enrich(Activity activity, GetDauMauQuery query)
    {
        var hashed = HashUtils.HashApiKey(query.ProjectApiKey);

        activity?.SetTag(OtelTagConstants.ProjectApiKeyHash, hashed);
    }
}
