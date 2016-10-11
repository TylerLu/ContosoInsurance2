#r "Newtonsoft.Json"

#load "..\Shared\ApplicationInsights.csx"

using Microsoft.ApplicationInsights;

private static readonly string FunctionName = "PushNotification";

public static async Task<HttpResponseMessage> Run(TemplateNotification notification, TraceWriter log)
{
    var telemetryClient = ApplicationInsights.CreateTelemetryClient();
    try
    {
        var hub = NotificationHubClient.CreateClientFromConnectionString(Settings.NotificationHubConnection, Settings.NotificationHubName);
        await hub.SendTemplateNotificationAsync(notification.Properties, notification.TagExpression);
        telemetryClient.TrackStatus(FunctionName, notification.CorrelationId, "Notification sent to Notification Hubs");
        return new HttpResponseMessage(HttpStatusCode.OK);
    }
    catch (Exception ex)
    {
        telemetryClient.TrackException(FunctionName, newClaim.CorrelationId, ex);
        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(ex.Message)
        };
    }
}