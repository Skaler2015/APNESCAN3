using Autofac;
using ApneScan.Images.Gtk;

namespace ApneScan.Modules;

public class GtkImagesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<GtkImageContext>().As<ImageContext>();
        builder.RegisterType<GtkImageContext>().AsSelf();
    }
}
