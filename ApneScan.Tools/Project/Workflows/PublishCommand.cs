using ApneScan.Tools.Project.Packaging;
using ApneScan.Tools.Project.Targets;
using ApneScan.Tools.Project.Verification;

namespace ApneScan.Tools.Project.Workflows;

public class PublishCommand : ICommand<PublishOptions>
{
    public int Run(PublishOptions opts)
    {
        new CleanCommand().Run(new CleanOptions());
        new BuildCommand().Run(new BuildOptions
        {
            BuildType = "debug"
        });
        foreach (var buildType in TargetsHelper.GetBuildTypesFromPackageType(opts.PackageType))
        {
            new BuildCommand().Run(new BuildOptions
            {
                BuildType = buildType
            });
        }
        new TestCommand().Run(new TestOptions
        {
            NoGui = opts.NoGui
        });
        new PackageCommand().Run(new PackageOptions
        {
            PackageType = opts.PackageType,
            Platform = opts.Platform,
            XCompile = opts.XCompile
        });
        if (!opts.NoVerify)
        {
            new VerifyCommand().Run(new VerifyOptions
            {
                PackageType = opts.PackageType,
                Platform = opts.Platform
            });
        }
        return 0;
    }
}