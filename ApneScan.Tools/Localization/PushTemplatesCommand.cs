using Crowdin.Api.SourceFiles;
using ApneScan.Tools.Project;
using File = System.IO.File;

namespace ApneScan.Tools.Localization;

public class PushTemplatesCommand : ICommand<PushTemplatesOptions>
{
    public int Run(PushTemplatesOptions opts)
    {
        return Task.Run(async () =>
        {
            var client = CrowdinHelper.GetClient();

            await using var templatesFile = File.OpenRead(Paths.TemplatesFile);
            var storage = await client.Storage.AddStorage(templatesFile, "templates.pot");

            await client.SourceFiles.UpdateOrRestoreFile(CrowdinHelper.PROJECT_ID, CrowdinHelper.TEMPLATES_FILE_ID,
                new ReplaceFileRequest
                {
                    StorageId = storage.Id
                });

            return 0;
        }).Result;
    }
}