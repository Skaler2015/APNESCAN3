using Autofac;
using ApneScan.EtoForms.Ui;
using ApneScan.ImportExport;
using ApneScan.ImportExport.Email;

namespace ApneScan.Modules;

public class GtkModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<LinuxApplicationLifecycle>().As<ApplicationLifecycle>();
        builder.RegisterType<GtkScannedImagePrinter>().As<IScannedImagePrinter>();
        builder.RegisterType<LinuxServiceManager>().As<IOsServiceManager>();
        builder.RegisterType<LinuxOpenWith>().As<IOpenWith>();
        builder.RegisterType<ThunderbirdEmailProvider>().As<IEmailProvider>().WithParameter("systemDefault", true);
        builder.RegisterType<StubSystemEmailClients>().As<ISystemEmailClients>();

        builder.RegisterType<GtkDesktopForm>().As<DesktopForm>();
        builder.RegisterType<GtkPreviewForm>().As<PreviewForm>();
    }
}
