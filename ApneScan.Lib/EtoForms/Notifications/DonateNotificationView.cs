namespace ApneScan.EtoForms.Notifications;

public class DonateNotificationView : LinkNotificationView
{
    private const string DONATE_URL = "https://www.apnescan.com/donate?src=notif";

    public DonateNotificationView(DonateNotification model)
        : base(model, MiscResources.DonatePrompt, MiscResources.Donate, DONATE_URL, null)
    {
        HideTimeout = HIDE_LONG;
    }

    protected override void LinkClick()
    {
        base.LinkClick();
        Manager!.Hide(Model);
    }
}