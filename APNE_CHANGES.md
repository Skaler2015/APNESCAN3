# Changes in this copy (not part of the original NAPS2)

This is a modified copy of NAPS2 8.6.0 (https://www.naps2.com, https://github.com/cyanfish/naps2).
NAPS2 is licensed under the GNU GPL 2.0 (or later); see the LICENSE file. This copy stays under the same license.

Modified on 2026-11-16:

1. **Sidebar tabs** - the left sidebar now has two tabs, "Scan" and "My Files".
2. **Sidebar preview** - below the scan settings, the selected page is shown (or the last scanned page when nothing
   is selected), with previous / next buttons and a button to open the full preview window. Double-clicking the
   preview also opens the full preview window.
3. **My Files** - lists the files most recently saved from the app ("Recent", up to 30) followed by the PDF and
   image files in a folder you pick (newest first, up to 500, 3 sub-folder levels). It has a search box and
   Open / Import / Folder... / Refresh / Show in folder buttons. Double-clicking a file opens it.
   The list is stored in `myfiles.txt` in the NAPS2 data folder.

## Files

| File | Change |
| --- | --- |
| `NAPS2.Lib/EtoForms/Desktop/Sidebar.cs` | tabs, preview, My Files UI |
| `NAPS2.Lib/ImportExport/MyFilesStore.cs` | new - stores the recent files and the chosen folder |
| `NAPS2.Lib/EtoForms/Notifications/Notify.cs` | records each saved PDF / image in MyFilesStore |
| `NAPS2.Lib/Modules/GuiModule.cs` | registers MyFilesStore |
| `.github/workflows/build-naps2-apne.yml` | new - builds the Windows zip on GitHub |

## Building

Needs the .NET 10 and .NET 11 SDKs on Windows.

```
dotnet run --project NAPS2.Tools -- pkg zip -p win64 --nosign -v
```

The zip is written under `NAPS2.Setup/publish/`.

Note: these changes were written without access to a .NET build machine, so they have not been compiled or run yet.
