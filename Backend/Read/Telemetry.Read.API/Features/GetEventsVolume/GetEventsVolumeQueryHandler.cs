using Telemetry.Read.API.Features.GetEventsVolume.Data;
using Telemetry.Read.CrossCuttingConcerns.Abstractions;

namespace Telemetry.Read.API.Features.GetEventsVolume;

public class GetEventsVolumeQueryHandler(IEventsVolumeDataSource dataSource) : IQueryHandler<GetEventsVolumeQuery, GetEventsVolumeResponse>
{
    public async Task<GetEventsVolumeResponse> HandleAsync(GetEventsVolumeQuery query, CancellationToken cancellationToken)
    {
        DateTime dbFrom = query.From.ToDateTime(TimeOnly.MinValue);
        DateTime dbTo = query.To.ToDateTime(TimeOnly.MaxValue);

        var data = await dataSource.GetAsync(
            query.ProjectApiKey, 
            dbFrom, 
            dbTo, 
            query.Granularity, 
            query.EventName, 
            cancellationToken);

        var items = new List<EventVolumePoint>();
        int stepMinutes = (int)query.Granularity;

        for (var currentStep = dbFrom; currentStep <= dbTo; currentStep = currentStep.AddMinutes(stepMinutes))
        {
            var totalEvents = data.TryGetValue(currentStep, out var count) ? count : 0;

            items.Add(new EventVolumePoint(currentStep, totalEvents));
        }

        string responseEventName = "All Events";

        if (!string.IsNullOrWhiteSpace(query.EventName))
        {
            responseEventName = query.EventName;
        }

        return new GetEventsVolumeResponse(responseEventName, query.Granularity, items);
    }
}
