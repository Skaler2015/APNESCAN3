using Eto.Drawing;
using Eto.Forms;

namespace ApneScan.EtoForms.Layout;

public class LayoutLeftPanel : LayoutContainer
{
    private readonly LayoutElement _left;
    private readonly LayoutElement _right;
    private readonly LayoutOverlay _overlay;

    private Func<int> _widthGetter = () => 0;
    private Action<int> _widthSetter = _ => { };
    private int? _minWidth;
    private bool _isInitialized;
    private bool _inLayout;
    private float _lastScale;
    private LayoutVisibility? _collapseVisibility;
    private bool _collapseSubscribed;
    private bool _wasCollapsed;

    public LayoutLeftPanel(LayoutElement left, LayoutElement right) : base([left, right])
    {
        _left = left;
        _right = right;
        // The right panel always fills the remaining width (the left panel is the fixed/resizable one).
        // Without this it only filled when its content happened to report a large preferred width, which
        // left wasted empty space when e.g. only a few scanned pages were present.
        _right.Scale = true;
        Splitter = new Splitter
        {
            Orientation = Orientation.Horizontal,
            Panel1 = new Panel(),
            Panel2 = new Panel(),
            FixedPanel = SplitterFixedPanel.Panel1
        };
        _overlay = L.Overlay(Splitter, L.Row(left, right).Spacing(EtoPlatform.Current.IsWinForms ? 3 : 2));
    }

    public Splitter Splitter { get; }

    public override void Materialize(LayoutContext context) => _overlay.Materialize(context);

    public override void DoLayout(LayoutContext context, RectangleF bounds)
    {
        // When the left panel is collapsible and currently hidden, give all the space to the right
        // panel (position 0) instead of reserving the splitter width, so nothing gets squeezed.
        if (_collapseVisibility != null && !_collapseSubscribed)
        {
            _collapseVisibility.IsVisibleChanged += (_, _) => context.Invalidate();
            _collapseSubscribed = true;
        }
        if (_collapseVisibility is { IsVisible: false })
        {
            _inLayout = true;
            Splitter.Panel1MinimumSize = 0;
            EtoPlatform.Current.SetSplitterPosition(Splitter, 0);
            _inLayout = false;
            _left.Width = 0;
            _wasCollapsed = true;
            _overlay.DoLayout(context, bounds);
            return;
        }

        var w = _minWidth.HasValue ? (int) (_minWidth * context.Scale) : MeasureWidth(context, bounds, _left);
        if (Splitter.Position < w)
        {
            EtoPlatform.Current.SetSplitterPosition(Splitter, w);
            _left.Width = w;
        }
        Splitter.Panel1MinimumSize = w;
        Splitter.Panel2MinimumSize = (int) (100 * context.Scale);

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (!_isInitialized || context.Scale != _lastScale || _wasCollapsed)
        {
            _wasCollapsed = false;
            _lastScale = context.Scale;
            int initialWidth = Math.Max((int) (_widthGetter() * context.Scale), w);
            _inLayout = true;
            EtoPlatform.Current.SetSplitterPosition(Splitter, initialWidth);
            _inLayout = false;
            _left.Width = initialWidth;
            if (!_isInitialized)
            {
                Splitter.PositionChanged += (_, _) =>
                {
                    if (!_inLayout && _left.Width != Splitter.Position)
                    {
                        _left.Width = Splitter.Position;
                        _widthSetter((int) (Splitter.Position / context.Scale));
                        context.Invalidate();
                    }
                };
                // TODO: We could hide the splitter based on the side panel's visibility, but currently on WinForms it's
                // responsible for drawing content borders so we don't want to do that. The splitter is hidden behind the
                // listview in any case.
                _isInitialized = true;
            }
        }

        _overlay.DoLayout(context, bounds);
    }

    private int MeasureWidth(LayoutContext context, RectangleF bounds, LayoutElement element)
    {
        var w = element.Width;
        element.Width = null;
        var measureContext = context with
        {
            IsLayout = false,
            UseCache = false
        };
        int result = (int) element.GetPreferredSize(measureContext, bounds).Width;
        element.Width = w;
        return result;
    }

    protected override SizeF GetPreferredSizeCore(LayoutContext context, RectangleF parentBounds)
    {
        return _overlay.GetPreferredSize(context, parentBounds);
    }

    public LayoutLeftPanel SizeConfig(Func<int> getter, Action<int> setter, int? minWidth = null)
    {
        _widthGetter = getter;
        _widthSetter = setter;
        _minWidth = minWidth;
        return this;
    }

    /// <summary>
    /// Makes the left panel collapse to zero width (handing all space to the right panel) whenever the
    /// given visibility is hidden, and restore its configured width when shown again. Used so a hidden
    /// side panel doesn't reserve splitter space.
    /// </summary>
    public LayoutLeftPanel Collapsible(LayoutVisibility visibility)
    {
        _collapseVisibility = visibility;
        return this;
    }
}