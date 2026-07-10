using System.Diagnostics;

namespace Telemetry.Read.Domain.Abstractions.Enrichers;

public interface IActivityEnricher<in TQuery>
{
    void Enrich(Activity activity, TQuery query);
}
