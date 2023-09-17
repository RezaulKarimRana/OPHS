using System.Linq;
using AMS.Common.Notifications;

namespace AMS.Models.ServiceModels
{
    public class ServiceResponse : IServiceResponse
    {
        public NotificationCollection Notifications { get; set; }

        public bool IsSuccessful { get { return !Notifications.Any(n => n.Type == NotificationTypeEnum.Error); } }

        public ServiceResponse()
        {
            Notifications = new NotificationCollection();
        }
    }
}
