using Eto.Drawing;
using Eto.Forms;
using NAPS2.EtoForms.Layout;
using NAPS2.EtoForms.Ui;
using NAPS2.EtoForms.Widgets;
using NAPS2.ImportExport;
using NAPS2.Scan;
using NAPS2.Scan.Internal;

namespace NAPS2.EtoForms.Desktop;

public class Sidebar
{
    public const int MIN_SIDEBAR_WIDTH = 200;
    public const int MIN_CONTENT_WIDTH = 100;

    // The sidebar preview is rendered at this size (longest side, in pixels) and scaled down when drawn
    private const int PREVIEW_RENDER_SIZE = 800;
    private const int MAX_FOLDER_FILES = 500;
    private const int MAX_FOLDER_DEPTH = 3;
    private const string HEADER_KEY_PREFIX = "#";

    private static readonly HashSet<string> MyFilesExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".bmp"
    };

    private readonly IScanPerformer _scanPerformer;
    private readonly DeviceCapsCache _deviceCapsCache;
    private readonly IProfileManager _profileManager;
    private readonly Naps2Config _config;
    private readonly IIconProvider _iconProvider;
    private readonly IFormFactory _formFactory;
    private readonly IDesktopScanController _desktopScanController;
    private readonly UiImageList _imageList;
    private readonly ThumbnailRenderer _thumbnailRenderer;
    private readonly IDesktopSubFormController _desktopSubFormController;
    private readonly DesktopController _desktopController;
    private readonly MyFilesStore _myFilesStore;
    private readonly ColorScheme _colorScheme;

    // Tabs
    private readonly LayoutVisibility _scanTabVis = new(true);
    private readonly LayoutVisibility _filesTabVis = new(false);
    private Button? _scanTabButton;
    private Button? _filesTabButton;

    // Preview
    private readonly Drawable _previewBox = new();
    private readonly Label _previewLabel = new()
    {
        Text = "",
        Wrap = WrapMode.None,
        TextAlignment = TextAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center
    };
    private Bitmap? _previewBitmap;
    private UiImage? _previewImage;
    private TransformState? _previewState;
    private bool _previewRendering;
    private bool _previewPending;

    // My Files
    private readonly TextBox _fileSearch = new() { PlaceholderText = "Search..." };
    private readonly ListBox _fileList = new();
    private List<string> _recentFiles = [];
    private List<string> _folderFiles = [];
    private string? _activeFolder;
    private int _filesRefreshVersion;

    private Window? _parentWindow;
    private bool _attached;
    private bool _closed;

    private readonly LayoutVisibility _sidebarVis = new(true);
    private readonly LayoutVisibility _onboardingVis = new(false);
    private readonly LayoutVisibility _predefinedVis = new(true);
    private readonly DropDownWidget<ScanProfile> _profile = new();
    private readonly EnumDropDownWidget<ScanSource> _paperSource = new();
    private readonly EnumDropDownWidget<ScanBitDepth> _bitDepth = new();

    private DeviceSelectorWidget? _deviceSelectorWidget;
    private PageSizeDropDownWidget? _pageSize;
    private ResolutionDropDownWidget? _resolution;

    public Sidebar(IScanPerformer scanPerformer, DeviceCapsCache deviceCapsCache, IProfileManager profileManager,
        Naps2Config config, IIconProvider iconProvider, IFormFactory formFactory,
        IDesktopScanController desktopScanController, UiImageList imageList, ThumbnailRenderer thumbnailRenderer,
        IDesktopSubFormController desktopSubFormController, DesktopController desktopController,
        MyFilesStore myFilesStore, ColorScheme colorScheme)
    {
        _scanPerformer = scanPerformer;
        _deviceCapsCache = deviceCapsCache;
        _profileManager = profileManager;
        _config = config;
        _iconProvider = iconProvider;
        _formFactory = formFactory;
        _desktopScanController = desktopScanController;
        _imageList = imageList;
        _thumbnailRenderer = thumbnailRenderer;
        _desktopSubFormController = desktopSubFormController;
        _desktopController = desktopController;
        _myFilesStore = myFilesStore;
        _colorScheme = colorScheme;
        _profile.Format = x => x.DisplayName;
        _profileManager.ProfilesUpdated += (_, _) => UpdateProfilesDropdown();
        UpdateProfilesDropdown();

        EditProfileCommand = new ActionCommand(EditProfile)
        {
            Text = UiStrings.Edit,
            IconName = "pencil"
        };
        NewProfileCommand = new ActionCommand(NewProfile)
        {
            Text = UiStrings.NewProfile,
            IconName = "add"
        };
        ScanCommand = new ActionCommand(DoScan)
        {
            Text = UiStrings.Scan,
            IconName = "control_play_blue"
        };
    }

    private ActionCommand NewProfileCommand { get; }
    private ActionCommand EditProfileCommand { get; }
    private ActionCommand ScanCommand { get; }

    private void UpdateProfilesDropdown()
    {
        _profile.Items = _profileManager.Profiles;
        _onboardingVis.IsVisible = _profileManager.Profiles.Count == 0;
    }

    private void EditProfile()
    {
        var originalProfile = _profile.SelectedItem;
        if (originalProfile != null)
        {
            var fedit = _formFactory.Create<EditProfileForm>();
            fedit.ScanProfile = originalProfile;
            fedit.ShowModal();
            if (fedit.Result)
            {
                _profile.SelectedItem = fedit.ScanProfile;
                _profileManager.Mutate(new ListMutation<ScanProfile>.ReplaceWith(fedit.ScanProfile),
                    ListSelection.Of(originalProfile));
            }
        }
    }

    private void NewProfile()
    {
        if (!(_config.Get(c => c.NoUserProfiles) && _profileManager.Profiles.Any(x => x.IsLocked)))
        {
            var fedit = _formFactory.Create<EditProfileForm>();
            fedit.NewProfile = true;
            fedit.ScanProfile = _config.DefaultProfileSettings();
            fedit.ShowModal();
            if (fedit.Result)
            {
                _profile.SelectedItem = fedit.ScanProfile;
                _profileManager.Mutate(new ListMutation<ScanProfile>.Append(fedit.ScanProfile),
                    ListSelection.Empty<ScanProfile>());
            }
        }
    }

    private void DoScan()
    {
        var profile = _profile.SelectedItem!;
        var pageSize = _pageSize!.SelectedItem!;

        profile.PaperSource = _paperSource.SelectedItem;
        profile.PageSize = pageSize.Type;
        profile.CustomPageSizeName = pageSize.CustomName;
        profile.CustomPageSize = pageSize.CustomDimens;
        profile.Resolution = new ScanResolution { Dpi = _resolution?.SelectedItem?.Dpi ?? 0 };
        profile.BitDepth = _bitDepth.SelectedItem;
        _profileManager.Save();

        _desktopScanController.ScanWithProfile(profile);
    }

    public bool IsVisible => _sidebarVis.IsVisible;

    public LayoutElement CreateView(IFormBase parentWindow, bool nativeSidebarHost = false)
    {
        _sidebarVis.IsVisible = _config.Get(c => c.SidebarVisible);
        _profile.SelectedItem = _profileManager.DefaultProfile ?? _profileManager.Profiles.FirstOrDefault();

        _deviceSelectorWidget = new DeviceSelectorWidget(_scanPerformer, _deviceCapsCache, _iconProvider, parentWindow)
        {
            ShowChooseDevice = false,
            ProfileFunc = () => _profile.SelectedItem!
        };
        _pageSize = new PageSizeDropDownWidget(parentWindow);
        _resolution = new ResolutionDropDownWidget(parentWindow);
        _profile.SelectedItemChanged += (_, _) =>
        {
            if (_config.Get(c => c.ScanChangesDefaultProfile))
            {
                _profileManager.DefaultProfile = _profile.SelectedItem;
            }
            UpdateUiForProfile();
        };
        _profileManager.ProfilesUpdated += (_, _) => UpdateUiForProfile();
        _paperSource.SelectedItemChanged += (_, _) => UpdateUiForSource();

        UpdateUiForProfile();

        _parentWindow = parentWindow as Window;
        // The button for the tab that's currently shown is disabled, so it reads as "selected"
        var scanTabButton = new Button { Text = "Scan", Enabled = false };
        var filesTabButton = new Button { Text = "My Files" };
        scanTabButton.Click += (_, _) => ShowTab(false);
        filesTabButton.Click += (_, _) => ShowTab(true);
        _scanTabButton = scanTabButton;
        _filesTabButton = filesTabButton;
        AttachPreviewAndFiles();

        int? profileButtonHeight = EtoPlatform.Current.IsMac ? 20 : null;
        int profileButtonWidth = EtoPlatform.Current.IsWpf ? 32 : 30;
        var scanTab = L.Column(
            L.Column(
                C.Button(NewProfileCommand, ButtonImagePosition.Left).Height(30).AlignCenter()
            ).Visible(_onboardingVis),
            L.Column(
                L.Row(
                    C.Label(UiStrings.ProfileLabel).AlignTrailing(),
                    C.Filler(),
                    C.Button(EditProfileCommand, ButtonImagePosition.Overlay)
                        .Height(profileButtonHeight).Width(profileButtonWidth),
                    C.Button(NewProfileCommand, ButtonImagePosition.Overlay)
                        .Height(profileButtonHeight).Width(profileButtonWidth)
                ),
                _profile.AsControl(),
                C.Spacer(),
                _deviceSelectorWidget,
                L.Column(
                    C.Spacer(),
                    C.Label(UiStrings.PaperSourceLabel),
                    _paperSource,
                    C.Label(UiStrings.PageSizeLabel),
                    _pageSize,
                    C.Label(UiStrings.ResolutionLabel),
                    _resolution,
                    C.Label(UiStrings.BitDepthLabel),
                    _bitDepth
                ).Visible(_predefinedVis),
                C.Spacer(),
                C.Button(ScanCommand, ButtonImagePosition.Left).AlignCenter().Height(30)
            ).Visible(!_onboardingVis),
            C.Spacer(),
            C.Label("Preview"),
            _previewBox.Scale(),
            L.Row(
                C.IconButton("arrow_left", "Previous page", () => StepPreview(-1)),
                _previewLabel.Scale(),
                C.IconButton("arrow_right", "Next page", () => StepPreview(1)),
                C.IconButton("zoom", "Open preview", OpenPreviewWindow)
            )
        ).Scale().Visible(_scanTabVis);

        var filesTab = L.Column(
            _fileSearch,
            _fileList.NaturalHeight(150).Scale(),
            L.Row(
                C.Button("Open", OpenSelectedFile).Scale(),
                C.Button("Import", ImportSelectedFile).Scale()
            ),
            L.Row(
                C.Button("Folder...", ChooseMyFilesFolder).Scale(),
                C.Button("Refresh", () => RefreshFiles().AssertNoAwait()).Scale()
            ),
            C.Button("Show in folder", ShowSelectedFileInFolder)
        ).Scale().Visible(_filesTabVis);

        var view = L.Column(
            C.Spacer().NaturalWidth(100),
            L.Row(
                scanTabButton.Scale(),
                filesTabButton.Scale()
            ),
            scanTab,
            filesTab,
            C.Spacer()
        ).Padding(left: nativeSidebarHost ? 10 : parentWindow.LayoutController.DefaultSpacing + 10, right: 10);
        return nativeSidebarHost ? view : view.Visible(_sidebarVis);
    }

    private void UpdateUiForProfile()
    {
        var profile = _profile.SelectedItem;
        if (profile == null) return;

        var deviceDriver = new ScanOptionsValidator().ValidateDriver(
            Enum.TryParse<Driver>(profile.DriverName, true, out var driver)
                ? driver
                : Driver.Default);

        var device = profile.Device?.ToScanDevice(deviceDriver);
        if (device != null)
        {
            _deviceSelectorWidget!.Choice = DeviceChoice.ForDevice(device);
        }
        else
        {
            _deviceSelectorWidget!.Choice = DeviceChoice.ForAlwaysAsk(deviceDriver);
        }

        _predefinedVis.IsVisible = !profile.UseNativeUI;

        if (profile.PageSize == ScanPageSize.Custom && profile.CustomPageSize != null)
        {
            _pageSize!.SetCustom(profile.CustomPageSizeName, profile.CustomPageSize);
        }
        else
        {
            _pageSize!.SetPreset(profile.PageSize);
        }

        _paperSource.SelectedItem = profile.PaperSource;
        _bitDepth.SelectedItem = profile.BitDepth;
        _resolution!.SetDpi(profile.Resolution.Dpi);

        _paperSource.Items = profile.Caps?.PaperSources?.Values is [_, ..] paperSources
            ? paperSources
            : EnumDropDownWidget<ScanSource>.DefaultItems;

        UpdateUiForSource();
    }

    private void UpdateUiForSource()
    {
        var profile = _profile.SelectedItem;
        if (profile == null) return;

        var selectedSource = _paperSource.SelectedItem;
        var perSource = selectedSource switch
        {
            ScanSource.Glass => profile.Caps?.Glass,
            ScanSource.Feeder => profile.Caps?.Feeder,
            ScanSource.Duplex => profile.Caps?.Duplex,
            _ => null
        };

        var validResolutions = perSource?.Resolutions;
        _resolution!.VisiblePresets = validResolutions is [_, ..]
            ? validResolutions
            : EnumDropDownWidget<ScanDpi>.DefaultItems.Select(x => x.ToIntDpi());

        var scanArea = perSource?.ScanArea;
        var sizeCaps = new PageSizeCaps { ScanArea = scanArea };

        // Exclude non-preset values (Custom, SystemDefault)
        var allPresets = EnumDropDownWidget<ScanPageSize>.DefaultItems.Where(x => x.PageDimensions() != null).ToList();
        var conditionalPresets = new[] { ScanPageSize.A3, ScanPageSize.B4 };
        _pageSize!.VisiblePresets = allPresets.Where(preset =>
            !conditionalPresets.Contains(preset) || sizeCaps.Fits(preset.PageDimensions()!.ToPageSize()));
        _pageSize.ShowCustom = perSource?.SupportsCustomPageSize ?? true;
    }

    public void ToggleVisibility()
    {
        _sidebarVis.IsVisible = !_sidebarVis.IsVisible;
        _config.User.Set(c => c.SidebarVisible, _sidebarVis.IsVisible);
        if (_sidebarVis.IsVisible)
        {
            RequestPreview(false);
        }
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Tabs
    // ---------------------------------------------------------------------------------------------------------------

    private void ShowTab(bool files)
    {
        _scanTabVis.IsVisible = !files;
        _filesTabVis.IsVisible = files;
        if (_scanTabButton != null) _scanTabButton.Enabled = files;
        if (_filesTabButton != null) _filesTabButton.Enabled = !files;
        if (files)
        {
            RefreshFiles().AssertNoAwait();
        }
        else
        {
            RequestPreview(false);
        }
    }

    private void AttachPreviewAndFiles()
    {
        if (_attached) return;
        _attached = true;

        _previewBox.Paint += PaintPreview;
        _previewBox.SizeChanged += (_, _) => _previewBox.Invalidate();
        _previewBox.MouseDoubleClick += (_, _) => OpenPreviewWindow();
        _previewLabel.Text = "No pages";

        _fileSearch.TextChanged += (_, _) => RebuildFileList();
        _fileList.Activated += (_, _) => OpenSelectedFile();

        _imageList.SelectionChanged += ImageListChanged;
        _imageList.ImagesUpdated += ImageListImagesUpdated;
        _imageList.ImagesThumbnailInvalidated += ImageListImagesInvalidated;
        _myFilesStore.Changed += MyFilesStoreChanged;
        if (_parentWindow != null)
        {
            _parentWindow.Closed += (_, _) => Detach();
        }

        // Wait until the window has finished loading before the first render
        Invoker.Current.InvokeDispatch(() => RequestPreview(false));
    }

    private void Detach()
    {
        if (_closed) return;
        _closed = true;
        _imageList.SelectionChanged -= ImageListChanged;
        _imageList.ImagesUpdated -= ImageListImagesUpdated;
        _imageList.ImagesThumbnailInvalidated -= ImageListImagesInvalidated;
        _myFilesStore.Changed -= MyFilesStoreChanged;
        _previewBitmap?.Dispose();
        _previewBitmap = null;
        _previewImage = null;
        _previewState = null;
    }

    // ---------------------------------------------------------------------------------------------------------------
    // Preview
    // ---------------------------------------------------------------------------------------------------------------

    private void ImageListChanged(object? sender, EventArgs e) =>
        Invoker.Current.InvokeDispatch(() => RequestPreview(false));

    private void ImageListImagesUpdated(object? sender, ImageListEventArgs e) =>
        Invoker.Current.InvokeDispatch(() => RequestPreview(false));

    private void ImageListImagesInvalidated(object? sender, ImageListEventArgs e) =>
        Invoker.Current.InvokeDispatch(() => RequestPreview(true));

    /// <summary>
    /// Gets the image the preview should show: the first selected page, or the last page if nothing is selected.
    /// </summary>
    private UiImage? GetPreviewTarget()
    {
        var selected = _imageList.Selection.FirstOrDefault();
        if (selected != null)
        {
            return selected;
        }
        var images = _imageList.Images;
        return images.Count > 0 ? images[images.Count - 1] : null;
    }

    private void RequestPreview(bool force)
    {
        if (_closed) return;
        if (force)
        {
            // The image content changed (e.g. rotated or cropped) so we can't reuse what we rendered
            _previewImage = null;
            _previewState = null;
        }
        if (!_sidebarVis.IsVisible || !_scanTabVis.IsVisible)
        {
            // Don't spend time rendering when the preview isn't on screen; we'll catch up when it's shown again
            return;
        }
        if (_previewRendering)
        {
            _previewPending = true;
            return;
        }
        RenderPreview().AssertNoAwait();
    }

    private async Task RenderPreview()
    {
        _previewRendering = true;
        try
        {
            do
            {
                _previewPending = false;
                var target = GetPreviewTarget();
                if (target == null || target.IsDisposed)
                {
                    SetPreview(null, null, null);
                    continue;
                }
                if (ReferenceEquals(target, _previewImage) && _previewBitmap != null &&
                    Equals(target.TransformState, _previewState))
                {
                    UpdatePreviewLabel();
                    continue;
                }
                Bitmap bitmap;
                TransformState state;
                using (var imageToRender = target.GetClonedImage())
                {
                    state = imageToRender.TransformState;
                    using var rendered = await _thumbnailRenderer.Render(imageToRender, PREVIEW_RENDER_SIZE);
                    bitmap = rendered.ToEtoImage();
                }
                if (_closed)
                {
                    bitmap.Dispose();
                    return;
                }
                SetPreview(bitmap, target, state);
            } while (_previewPending);
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error rendering sidebar preview", ex);
        }
        finally
        {
            _previewRendering = false;
        }
    }

    private void SetPreview(Bitmap? bitmap, UiImage? image, TransformState? state)
    {
        var old = _previewBitmap;
        _previewBitmap = bitmap;
        _previewImage = image;
        _previewState = state;
        old?.Dispose();
        UpdatePreviewLabel();
        _previewBox.Invalidate();
    }

    private void UpdatePreviewLabel()
    {
        var images = _imageList.Images;
        var index = _previewImage == null ? -1 : images.IndexOf(_previewImage);
        _previewLabel.Text = index >= 0
            ? string.Format(UiStrings.XOfY, index + 1, images.Count)
            : "No pages";
    }

    private void PaintPreview(object? sender, PaintEventArgs e)
    {
        float width = _previewBox.Width;
        float height = _previewBox.Height;
        if (width < 4 || height < 4) return;
        e.Graphics.FillRectangle(_colorScheme.BackgroundColor, 0, 0, width, height);
        e.Graphics.DrawRectangle(_colorScheme.SeparatorColor, 0, 0, width - 1, height - 1);
        var bitmap = _previewBitmap;
        if (bitmap == null || bitmap.Width <= 0 || bitmap.Height <= 0) return;
        const float margin = 4;
        float availableWidth = width - margin * 2;
        float availableHeight = height - margin * 2;
        if (availableWidth <= 0 || availableHeight <= 0) return;
        float scale = Math.Min(availableWidth / bitmap.Width, availableHeight / bitmap.Height);
        float drawWidth = bitmap.Width * scale;
        float drawHeight = bitmap.Height * scale;
        e.Graphics.ImageInterpolation = ImageInterpolation.High;
        e.Graphics.DrawImage(bitmap, (width - drawWidth) / 2, (height - drawHeight) / 2, drawWidth, drawHeight);
    }

    private void StepPreview(int delta)
    {
        var images = _imageList.Images;
        if (images.Count == 0) return;
        var current = GetPreviewTarget();
        int index = current == null ? -1 : images.IndexOf(current);
        int next = Math.Max(0, Math.Min(images.Count - 1, index + delta));
        _imageList.UpdateSelection(ListSelection.Of(images[next]));
    }

    private void OpenPreviewWindow()
    {
        var target = GetPreviewTarget();
        if (target == null) return;
        if (!_imageList.Selection.Contains(target))
        {
            _imageList.UpdateSelection(ListSelection.Of(target));
        }
        _desktopSubFormController.ShowViewerForm();
    }

    // ---------------------------------------------------------------------------------------------------------------
    // My Files
    // ---------------------------------------------------------------------------------------------------------------

    private void MyFilesStoreChanged(object? sender, EventArgs e) =>
        Invoker.Current.InvokeDispatch(() =>
        {
            if (_closed) return;
            RefreshFiles().AssertNoAwait();
        });

    private sealed class MyFilesScan
    {
        public List<string> Recent { get; init; } = [];
        public List<string> FolderFiles { get; init; } = [];
        public string? Folder { get; init; }
    }

    private async Task RefreshFiles()
    {
        int version = ++_filesRefreshVersion;
        try
        {
            var folder = _myFilesStore.Folder;
            var recent = _myFilesStore.GetRecent();
            var scan = await Task.Run(() => ScanMyFiles(folder, recent));
            if (version != _filesRefreshVersion || _closed) return;
            _recentFiles = scan.Recent;
            _folderFiles = scan.FolderFiles;
            _activeFolder = scan.Folder;
            RebuildFileList();
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error listing My Files", ex);
        }
    }

    private static MyFilesScan ScanMyFiles(string? folder, List<string> recent)
    {
        var recentExisting = recent.Where(x => SafeFileExists(x)).ToList();
        if (string.IsNullOrEmpty(folder))
        {
            // No folder picked yet, so show the folder of the most recently saved file
            folder = recentExisting
                .Select(x => Path.GetDirectoryName(x))
                .FirstOrDefault(x => !string.IsNullOrEmpty(x));
        }
        var folderFiles = new List<string>();
        try
        {
            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                var options = new EnumerationOptions
                {
                    RecurseSubdirectories = true,
                    IgnoreInaccessible = true,
                    MaxRecursionDepth = MAX_FOLDER_DEPTH
                };
                folderFiles = new DirectoryInfo(folder).EnumerateFiles("*", options)
                    .Where(x => MyFilesExtensions.Contains(x.Extension))
                    .OrderByDescending(x => x.LastWriteTimeUtc)
                    .Take(MAX_FOLDER_FILES)
                    .Select(x => x.FullName)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error reading My Files folder", ex);
        }
        return new MyFilesScan
        {
            Recent = recentExisting,
            FolderFiles = folderFiles,
            Folder = folder
        };
    }

    private static bool SafeFileExists(string path)
    {
        try
        {
            return File.Exists(path);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void RebuildFileList()
    {
        if (_closed) return;
        var query = (_fileSearch.Text ?? "").Trim();
        bool Matches(string path) =>
            query.Length == 0 || path.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

        var recent = _recentFiles.Where(Matches).ToList();
        var recentSet = new HashSet<string>(_recentFiles, StringComparer.OrdinalIgnoreCase);
        var folderFiles = _folderFiles.Where(x => !recentSet.Contains(x) && Matches(x)).ToList();

        _fileList.Items.Clear();
        if (recent.Count > 0)
        {
            _fileList.Items.Add(new ListItem { Key = HEADER_KEY_PREFIX + "recent", Text = "--- Recent ---" });
            foreach (var path in recent)
            {
                _fileList.Items.Add(new ListItem { Key = path, Text = Path.GetFileName(path) });
            }
        }
        if (!string.IsNullOrEmpty(_activeFolder))
        {
            var folderName = Path.GetFileName(_activeFolder.TrimEnd(
                Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = _activeFolder;
            }
            _fileList.Items.Add(new ListItem
            {
                Key = HEADER_KEY_PREFIX + "folder",
                Text = "--- " + folderName + " ---"
            });
            foreach (var path in folderFiles)
            {
                _fileList.Items.Add(new ListItem { Key = path, Text = GetFolderDisplayName(_activeFolder, path) });
            }
        }
        if (_fileList.Items.Count == 0)
        {
            _fileList.Items.Add(new ListItem
            {
                Key = HEADER_KEY_PREFIX + "empty",
                Text = query.Length == 0 ? "No files yet. Click Folder..." : "No matching files"
            });
        }
    }

    private static string GetFolderDisplayName(string folder, string path)
    {
        try
        {
            return Path.GetRelativePath(folder, path);
        }
        catch (Exception)
        {
            return Path.GetFileName(path);
        }
    }

    private string? GetSelectedFilePath()
    {
        var key = _fileList.SelectedKey;
        if (string.IsNullOrEmpty(key) || key.StartsWith(HEADER_KEY_PREFIX, StringComparison.Ordinal))
        {
            return null;
        }
        if (!SafeFileExists(key))
        {
            // The file was moved or deleted outside the app, so the list is out of date
            RefreshFiles().AssertNoAwait();
            return null;
        }
        return key;
    }

    private void OpenSelectedFile()
    {
        var path = GetSelectedFilePath();
        if (path == null) return;
        try
        {
            ProcessHelper.OpenFile(path);
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error opening file from My Files", ex);
        }
    }

    private void ImportSelectedFile()
    {
        var path = GetSelectedFilePath();
        if (path == null) return;
        try
        {
            _desktopController.ImportFiles([path]);
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error importing file from My Files", ex);
        }
    }

    private void ShowSelectedFileInFolder()
    {
        try
        {
            var path = GetSelectedFilePath();
            if (path == null)
            {
                if (!string.IsNullOrEmpty(_activeFolder) && Directory.Exists(_activeFolder))
                {
                    ProcessHelper.OpenFolder(_activeFolder);
                }
                return;
            }
            if (OperatingSystem.IsWindows())
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "/select,\"" + path + "\"",
                    UseShellExecute = true
                });
            }
            else
            {
                var folder = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(folder))
                {
                    ProcessHelper.OpenFolder(folder);
                }
            }
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error showing file from My Files", ex);
        }
    }

    private void ChooseMyFilesFolder()
    {
        try
        {
            using var dialog = new SelectFolderDialog { Title = "My Files" };
            var current = _myFilesStore.Folder ?? _activeFolder;
            if (!string.IsNullOrEmpty(current) && Directory.Exists(current))
            {
                dialog.Directory = current;
            }
            var result = _parentWindow != null ? dialog.ShowDialog(_parentWindow) : dialog.ShowDialog(_fileList);
            if (result == DialogResult.Ok && !string.IsNullOrEmpty(dialog.Directory))
            {
                // Setting the folder raises MyFilesStore.Changed, which refreshes the list
                _myFilesStore.Folder = dialog.Directory;
            }
        }
        catch (Exception ex)
        {
            Log.ErrorException("Error choosing My Files folder", ex);
        }
    }
}