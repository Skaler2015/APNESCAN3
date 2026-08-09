using Autofac;
using ApneScan.EtoForms;

namespace ApneScan;

public class AutofacFormFactory : IFormFactory
{
    private readonly IComponentContext _container;

    public AutofacFormFactory(IComponentContext container)
    {
        _container = container;
    }

    public T Create<T>() where T : IFormBase
    {
        var form = _container.Resolve<T>();
        form.FormFactory = _container.Resolve<IFormFactory>();
        form.Config = _container.Resolve<ApneScanConfig>();
        return form;
    }
}