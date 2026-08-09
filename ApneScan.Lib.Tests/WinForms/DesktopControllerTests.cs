using ApneScan.EtoForms;
using ApneScan.EtoForms.Desktop;
using ApneScan.EtoForms.Notifications;
using ApneScan.ImportExport;
using ApneScan.Platform.Windows;
using ApneScan.Recovery;
using ApneScan.Remoting;
using ApneScan.Remoting.Server;
using ApneScan.Remoting.Worker;
using ApneScan.Sdk.Tests;
using ApneScan.Sdk.Tests.Asserts;
using ApneScan.Update;
using NSubstitute;
using Xunit;

namespace ApneScan.Lib.Tests.WinForms;

public class DesktopControllerTests : ContextualTests
{
    // TODO: We should create individual unit tests for everything that's mocked here.
    // TODO: We should also make some things more testable:
    // - Change DesktopFormProvider to e.g. DesktopFormActivator or something else with a mockable interface
    private readonly DesktopController _desktopController;
    private readonly UiImageList _imageList;
    private readonly RecoveryStorageManager _recoveryStorageManager;
    private readonly ThumbnailRenderQueue _thumbnailRenderQueue;
    private readonly OperationProgress _operationProgress;
    private readonly ApneScanConfig _config;
    private readonly IOperationFactory _operationFactory;
    private readonly StillImage _stillImage;
    private readonly IUpdateChecker _updateChecker;
    private readonly INotify _notify;
    private readonly ImageClipboard _imageClipboard;
    private readonly IExportController _exportHelper;
    private readonly DialogHelper _dialogHelper;
    private readonly DesktopImagesController _desktopImagesController;
    private readonly IDesktopScanController _desktopScanController;
    private readonly DesktopFormProvider _desktopFormProvider;
    private readonly IScannedImagePrinter _scannedImagePrinter;
    private readonly ThumbnailController _thumbnailController;
    private readonly ISharedDeviceManager _sharedDeviceManager;
    private readonly ProcessCoordinator _processCoordinator;

    public DesktopControllerTests()
    {
        SetUpFileStorage();
        _imageList = new UiImageList();
        _recoveryStorageManager = RecoveryStorageManager.CreateFolder(ScanningContext.RecoveryPath!, _imageList);
        _thumbnailRenderQueue = new ThumbnailRenderQueue(ScanningContext, new ThumbnailRenderer(ImageContext));
        _operationProgress = Substitute.For<OperationProgress>();
        _config = ApneScanConfig.Stub();
        _operationFactory = Substitute.For<IOperationFactory>();
        _stillImage = new StillImage();
        _updateChecker = Substitute.For<IUpdateChecker>();
        _notify = Substitute.For<INotify>();
        _imageClipboard = new ImageClipboard();
        _exportHelper = Substitute.For<IExportController>();
        _dialogHelper = Substitute.For<DialogHelper>();
        _desktopImagesController = new DesktopImagesController(_imageList);
        _desktopScanController = Substitute.For<IDesktopScanController>();
        _desktopFormProvider = new DesktopFormProvider();
        _scannedImagePrinter = Substitute.For<IScannedImagePrinter>();
        _thumbnailController = new ThumbnailController(_thumbnailRenderQueue, _config);
        _sharedDeviceManager = Substitute.For<ISharedDeviceManager>();
        _processCoordinator =
            new ProcessCoordinator(FolderPath, Guid.NewGuid().ToString("D"));
        ScanningContext.WorkerFactory = Substitute.For<IWorkerFactory>();
        _desktopController = new DesktopController(
            ScanningContext,
            _imageList,
            _recoveryStorageManager,
            _thumbnailController,
            _operationProgress,
            _config,
            _operationFactory,
            _stillImage,
            _updateChecker,
            _notify,
            _imageClipboard,
            new ImageListActions(_imageList, _operationFactory, _operationProgress,
                _config, _thumbnailController, _exportHelper, _notify, null!),
            _dialogHelper,
            _desktopImagesController,
            _desktopScanController,
            _desktopFormProvider,
            _scannedImagePrinter,
            _sharedDeviceManager,
            _processCoordinator,
            new RecoveryManager(ScanningContext)
        );

        _operationFactory.Create<RecoveryOperation>().Returns(
            new RecoveryOperation(Substitute.For<IFormFactory>(), new RecoveryManager(ScanningContext)));
    }

    public override void Dispose()
    {
        base.Dispose();
        _desktopController.Cleanup();
    }

    [Fact]
    public async Task Initialize_IfNotRunBefore_SetsFirstRunDate()
    {
        Assert.False(_config.Get(c => c.HasBeenRun));
        Assert.Null(_config.Get(c => c.FirstRunDate));

        await _desktopController.Initialize();

        Assert.True(_config.Get(c => c.HasBeenRun));
        DateAsserts.Recent(TimeSpan.FromMilliseconds(1000), _config.Get(c => c.FirstRunDate));
        _notify.ReceivedCallsCount(0);
    }

    [Fact]
    public async Task Initialize_IfAlreadyRun_DoesntSetFirstRunDate()
    {
        var firstRunDate = DateTime.Now - TimeSpan.FromDays(29);
        _config.User.Set(c => c.HasBeenRun, true);
        _config.User.Set(c => c.FirstRunDate, firstRunDate);

        await _desktopController.Initialize();

        Assert.True(_config.Get(c => c.HasBeenRun));
        Assert.Equal(firstRunDate, _config.Get(c => c.FirstRunDate));
        _notify.ReceivedCallsCount(0);
    }

    [Fact]
    public async Task Initialize_IfRun30DaysAgo_ShowsDonatePrompt()
    {
        var firstRunDate = DateTime.Now - TimeSpan.FromDays(31);
        _config.User.Set(c => c.HasBeenRun, true);
        _config.User.Set(c => c.FirstRunDate, firstRunDate);

        await _desktopController.Initialize();

        _notify.Received().DonatePrompt();
        Assert.True(_config.Get(c => c.HasBeenPromptedForDonation));
        DateAsserts.Recent(TimeSpan.FromMilliseconds(1000), _config.Get(c => c.LastDonatePromptDate));
    }

    [Fact]
    public async Task Initialize_IfDonatePromptAlreadyShown_DoesntShowDonatePrompt()
    {
        var firstRunDate = DateTime.Now - TimeSpan.FromDays(62);
        _config.User.Set(c => c.HasBeenRun, true);
        _config.User.Set(c => c.FirstRunDate, firstRunDate);
        var donatePromptDate = DateTime.Now - TimeSpan.FromDays(31);
        _config.User.Set(c => c.HasBeenPromptedForDonation, true);
        _config.User.Set(c => c.LastDonatePromptDate, donatePromptDate);

        await _desktopController.Initialize();

        Assert.True(_config.Get(c => c.HasBeenPromptedForDonation));
        Assert.Equal(donatePromptDate, _config.Get(c => c.LastDonatePromptDate));
        _notify.ReceivedCallsCount(0);
    }

    [Fact]
    public async Task Initialize_WithStillImageArgs_StartsScan()
    {
        _stillImage.ParseArgs(new[] { "/StiEvent:blah", "/StiDevice:abc" });

        await _desktopController.Initialize();

        _ = _desktopScanController.Received().ScanWithDevice("abc");
        _notify.ReceivedCallsCount(0);
    }

    [Fact(Skip = "flaky")]
    public async Task Initialize_WithUpdateChecksDisabled_DoesntCheckForUpdate()
    {
        await _desktopController.Initialize();
        await Task.Delay(500);

        Assert.False(_config.Get(c => c.HasCheckedForUpdates));
        Assert.Null(_config.Get(c => c.LastUpdateCheckDate));
        _updateChecker.ReceivedCallsCount(0);
    }

    [Fact(Skip = "flaky")]
    public async Task Initialize_WithNoUpdate_DoesntPromptToUpdate()
    {
        _config.User.Set(c => c.CheckForUpdates, true);

        await _desktopController.Initialize();
        await Task.Delay(500);

        Assert.True(_config.Get(c => c.HasCheckedForUpdates));
        DateAsserts.Recent(TimeSpan.FromMilliseconds(1000), _config.Get(c => c.LastUpdateCheckDate));
        _ = _updateChecker.Received().CheckForUpdates();
        _updateChecker.ReceivedCallsCount(1);
        _notify.ReceivedCallsCount(0);
    }

    [Fact(Skip = "flaky")]
    public async Task Initialize_WithUpdate_NotifiesOfUpdate()
    {
        _config.User.Set(c => c.CheckForUpdates, true);
        var mockUpdateInfo =
            new UpdateInfo("10.0.0", "https://www.example.com", Array.Empty<byte>(), Array.Empty<byte>());
        _updateChecker.CheckForUpdates().Returns(Task.FromResult(mockUpdateInfo));

        await _desktopController.Initialize();
        await Task.Delay(500);

        Assert.True(_config.Get(c => c.HasCheckedForUpdates));
        DateAsserts.Recent(TimeSpan.FromMilliseconds(1000), _config.Get(c => c.LastUpdateCheckDate));
        _ = _updateChecker.Received().CheckForUpdates();
        _notify.UpdateAvailable(_updateChecker, mockUpdateInfo);
        _updateChecker.ReceivedCallsCount(1);
        _notify.ReceivedCallsCount(1);
    }

    [Fact(Skip = "flaky")]
    public async Task Initialize_WithNoUpdatePrompt_DoesntCheckForUpdate()
    {
        _config.AppDefault.Set(c => c.NoUpdatePrompt, true);
        _config.User.Set(c => c.CheckForUpdates, true);

        await _desktopController.Initialize();
        await Task.Delay(500);

        Assert.False(_config.Get(c => c.HasCheckedForUpdates));
        Assert.Null(_config.Get(c => c.LastUpdateCheckDate));
        _updateChecker.ReceivedCallsCount(0);
        _notify.ReceivedCallsCount(0);
    }

    [Fact(Skip = "flaky")]
    public async Task Initialize_WithRecentUpdateCheck_DoesntCheckForUpdate()
    {
        var updateCheckDate = DateTime.Now - TimeSpan.FromDays(6);
        _config.User.Set(c => c.CheckForUpdates, true);
        _config.User.Set(c => c.HasCheckedForUpdates, true);
        _config.User.Set(c => c.LastUpdateCheckDate, updateCheckDate);

        await _desktopController.Initialize();
        await Task.Delay(500);

        Assert.True(_config.Get(c => c.HasCheckedForUpdates));
        Assert.Equal(updateCheckDate, _config.Get(c => c.LastUpdateCheckDate));
        _updateChecker.ReceivedCallsCount(0);
        _notify.ReceivedCallsCount(0);
    }

    [Fact(Skip = "flaky")]
    public async Task Initialize_WithOldUpdateCheck_NotifiesOfUpdate()
    {
        var updateCheckDate = DateTime.Now - TimeSpan.FromDays(8);
        _config.User.Set(c => c.CheckForUpdates, true);
        _config.User.Set(c => c.HasCheckedForUpdates, true);
        _config.User.Set(c => c.LastUpdateCheckDate, updateCheckDate);
        var mockUpdateInfo =
            new UpdateInfo("10.0.0", "https://www.example.com", Array.Empty<byte>(), Array.Empty<byte>());
        _updateChecker.CheckForUpdates().Returns(Task.FromResult(mockUpdateInfo));

        await _desktopController.Initialize();
        await Task.Delay(500);

        Assert.True(_config.Get(c => c.HasCheckedForUpdates));
        DateAsserts.Recent(TimeSpan.FromMilliseconds(1000), _config.Get(c => c.LastUpdateCheckDate));
        _ = _updateChecker.Received().CheckForUpdates();
        _notify.Received().UpdateAvailable(_updateChecker, mockUpdateInfo);
        _updateChecker.ReceivedCallsCount(1);
        _notify.ReceivedCallsCount(1);
    }

    [Fact]
    public async Task ProcessCoordinatorOpenFile()
    {
        var importOp = new ImportOperation(new FileImporter(ScanningContext));
        _operationFactory.Create<ImportOperation>().Returns(importOp);
        var path = CopyResourceToFile(ImageResources.dog, "test.jpg");

        await _desktopController.Initialize();
        Assert.True(_processCoordinator.OpenFile(Process.GetCurrentProcess(), 10000, path));
        await Task.WhenAny(importOp.Success, Task.Delay(10000));

        Assert.Single(_imageList.Images);
        ImageAsserts.Similar(ImageResources.dog, _imageList.Images[0].GetClonedImage().Render());
    }

    [Fact]
    public async Task ProcessCoordinatorScanWithDevice()
    {
        await _desktopController.Initialize();
        Assert.True(_processCoordinator.ScanWithDevice(Process.GetCurrentProcess(), 10000, "abc"));
        await Task.Delay(500);
        _ = _desktopScanController.Received().ScanWithDevice("abc");
    }
}