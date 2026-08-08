using Autofac;
using ApneScan.EtoForms;
using ApneScan.EtoForms.Desktop;
using ApneScan.EtoForms.Notifications;
using ApneScan.ImportExport;
using ApneScan.Ocr;
using ApneScan.Pdf;
using ApneScan.Scan;
using ApneScan.Scan.Batch;
using ApneScan.Update;

namespace ApneScan.Modules;

/// <summary>
/// GUI-specific module used by GuiEntryPoint.
/// </summary>
public class GuiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<BatchScanPerformer>().As<IBatchScanPerformer>();
        builder.RegisterType<MessageBoxErrorOutput>().As<ErrorOutput>();
        builder.RegisterType<EtoOverwritePrompt>().As<IOverwritePrompt>();
        builder.RegisterType<EtoOperationProgress>().As<OperationProgress>().SingleInstance();
        builder.RegisterType<EtoDialogHelper>().As<DialogHelper>();
        builder.RegisterType<EtoDevicePrompt>().As<IDevicePrompt>();
        builder.RegisterType<EtoPdfPasswordProvider>().As<IPdfPasswordProvider>();
        builder.RegisterType<NotificationManager>().AsSelf().SingleInstance();
        builder.RegisterType<Notify>().As<INotify>();
        builder.RegisterType<Notify>().As<ISaveNotify>();
        builder.RegisterType<DesktopController>().AsSelf().SingleInstance();
        builder.RegisterType<UpdateChecker>().As<IUpdateChecker>();
        builder.RegisterType<ExportController>().As<IExportController>();
        builder.RegisterType<DesktopScanController>().As<IDesktopScanController>().SingleInstance();
        builder.RegisterType<DesktopSubFormController>().As<IDesktopSubFormController>().SingleInstance();
        builder.RegisterType<DesktopFormProvider>().AsSelf().SingleInstance();
        builder.RegisterType<ImageListActions>().AsSelf().SingleInstance();
        builder.RegisterInstance(EtoPlatform.Current.DarkModeProvider);
        builder.RegisterInstance(EtoPlatform.Current.ColorScheme);

        builder.RegisterBuildCallback(ctx =>
        {
            var scanningContext = ctx.Resolve<ScanningContext>();
            scanningContext.FileStorageManager = ctx.Resolve<FileStorageManager>();
            scanningContext.OcrEngine = ctx.Resolve<IOcrEngine>();
        });
    }
}