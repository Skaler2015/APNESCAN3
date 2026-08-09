using Eto.Forms;
using ApneScan.EtoForms.Layout;

namespace ApneScan.EtoForms;

public abstract class EtoFormBase : Form, IFormBase
{
    private IFormFactory? _formFactory;

    protected EtoFormBase(ApneScanConfig config)
    {
        Config = config;
        FormStateController = new FormStateController(this, config);
        Resizable = true;
        LayoutController.Bind(this);
        LayoutController.Invalidated += (_, _) => FormStateController.UpdateLayoutSize(LayoutController);
        EtoPlatform.Current.InitForm(this);
    }

    protected abstract void BuildLayout();

    protected override void OnPreLoad(EventArgs e)
    {
        BuildLayout();
        base.OnPreLoad(e);
    }

    public FormStateController FormStateController { get; }

    public LayoutController LayoutController { get; } = new();

    public IFormFactory FormFactory
    {
        get => _formFactory ?? throw new InvalidOperationException();
        set => _formFactory = value;
    }
        
    public ApneScanConfig Config { get; set; }

    public string IconName
    {
        set
        {
            EtoPlatform.Current.AttachDpiDependency(this,
                scale => Icon = EtoPlatform.Current.IconProvider.GetFormIcon(value, scale));
        }
    }
}