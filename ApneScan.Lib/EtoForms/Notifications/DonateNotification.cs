namespace ApneScan.EtoForms.Notifications;

public class DonateNotification : NotificationModel
{
    public override NotificationView CreateView()
    {
        return new DonateNotificationView(this);
    }
}