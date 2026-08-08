using Autofac;
using ApneScan.EtoForms;
using ApneScan.EtoForms.Mac;
using ApneScan.EtoForms.Ui;
using ApneScan.ImportExport;
using ApneScan.ImportExport.Email;

namespace ApneScan.Modules;

public class MacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MacApplicationLifecycle>().As<ApplicationLifecycle>();
        builder.RegisterType<MacScannedImagePrinter>().As<IScannedImagePrinter>();
        builder.RegisterType<AppleMailEmailProvider>().As<IAppleMailEmailProvider>();
        builder.RegisterType<MacIconProvider>().As<IIconProvider>();
        builder.RegisterType<MacServiceManager>().As<IOsServiceManager>();
        builder.RegisterType<MacOpenWith>().As<IOpenWith>();
        builder.RegisterType<AppleMailEmailProvider>().As<IEmailProvider>().WithParameter("systemDefault", true);
        builder.RegisterType<StubSystemEmailClients>().As<ISystemEmailClients>();

        builder.RegisterType<MacDesktopForm>().As<DesktopForm>();
        builder.RegisterType<MacPreviewForm>().As<PreviewForm>();
    }
}
