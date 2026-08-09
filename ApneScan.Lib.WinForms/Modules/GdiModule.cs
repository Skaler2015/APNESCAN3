using Autofac;
using ApneScan.Images.Gdi;
using ApneScan.ImportExport.Email.Mapi;

namespace ApneScan.Modules;

public class GdiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<GdiImageContext>().As<ImageContext>();
        builder.RegisterType<GdiImageContext>().AsSelf();
        builder.RegisterType<MapiWrapper>().As<IMapiWrapper>();
    }
}