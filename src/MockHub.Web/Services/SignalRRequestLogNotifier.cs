using Microsoft.AspNetCore.SignalR;
using MockHub.Application.Interfaces;
using MockHub.Web.Hubs;

namespace MockHub.Web.Services;

public class SignalRRequestLogNotifier : IRequestLogNotifier
{
    private readonly IHubContext<MockHubSignalR> _hubContext;

    public SignalRRequestLogNotifier(IHubContext<MockHubSignalR> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyRequestReceivedAsync(RequestLogNotification notification)
    {
        var dto = new RequestLogDto
        {
            ProjectId = notification.ProjectId,
            EndpointId = notification.EndpointId,
            Method = notification.Method,
            Path = notification.Path,
            QueryString = notification.QueryString,
            StatusCode = notification.StatusCode,
            DurationMs = notification.DurationMs,
            IsMatched = notification.IsMatched,
            ErrorMessage = notification.ErrorMessage,
            ClientIp = notification.ClientIp,
            Timestamp = notification.Timestamp
        };

        // Send to all connected clients
        await _hubContext.Clients.All.SendAsync("RequestReceived", dto);
    }
}

