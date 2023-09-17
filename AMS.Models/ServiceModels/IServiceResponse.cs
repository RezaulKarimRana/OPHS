using AMS.Common.Notifications;

namespace AMS.Models.ServiceModels
{
    public interface IServiceResponse
    {
        NotificationCollection Notifications { get; set; }
    }
}
