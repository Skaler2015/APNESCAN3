using Autofac;
using ApneScan.Images.Mac;

namespace ApneScan.Modules;

public class MacImagesModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MacImageContext>().As<ImageContext>();
        builder.RegisterType<MacImageContext>().AsSelf();
    }
}
