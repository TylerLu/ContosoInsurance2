#r "Newtonsoft.Json"

#load "..\Shared\ApplicationInsights.csx"
#load "..\Shared\Settings.csx"
#load "TemplateNotification.csx"

using Microsoft.ApplicationInsights;
using Microsoft.Azure.NotificationHubs;
using System.Net;

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
        telemetryClient.TrackException(FunctionName, notification.CorrelationId, ex);
        return new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent(ex.Message)
        };
    }
}