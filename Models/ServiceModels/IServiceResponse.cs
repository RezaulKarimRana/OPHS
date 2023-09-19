using Common.Notifications;

namespace Models.ServiceModels
{
    public interface IServiceResponse
    {
        NotificationCollection Notifications { get; set; }
    }
}
