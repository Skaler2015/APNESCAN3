namespace ApneScan.EtoForms.Notifications;

public class ReviewNotification : NotificationModel
{
    public override NotificationView CreateView()
    {
        return new ReviewNotificationView(this);
    }
}