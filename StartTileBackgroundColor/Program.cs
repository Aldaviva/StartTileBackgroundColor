using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.PowerShell;
using NLog;
using Shellify;
using StartTileBackgroundColor.Data;
using VisualElementsManifest;
using VisualElementsManifest.Data;

namespace StartTileBackgroundColor {

    /// <remarks>
    /// <para>After adding or modifying a VisualElementsManifest XML file for a program, its tile will not immediately update in Start. You have to clear the tile cache for the update to take effect.</para>
    /// <para>You can clear the tile cache with WinAero Tweaker (https://winaero.com/download.php?view.1796) using the Reset Live Tile Cache tweak.</para>
    /// <para>You can also clear the tile cache manually by setting a registry value (<c>HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ImmersiveShell\StateStore\ResetCache</c>) to <c>0x1</c> and restarting Explorer (https://winaero.com/blog/clear-live-tile-cache-windows-10/).</para>
    /// </remarks>
    internal static class Program {

        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private static readonly Color TILE_BACKGROUND_COLOR = Color.FromArgb(0x40, 0x40, 0x40);

        private static void Main() {
            LOGGER.Info("Starting");

            var startLayoutEditor = new StartLayoutEditor();
            var visualElementsManifestEditor = new VisualElementsManifestEditorImpl();
            string exportedLayoutTempFile = exportStartLayoutToFile();

            try {
                LOGGER.Debug("Loading start layout");
                LayoutModificationTemplate startLayout = startLayoutEditor.load(exportedLayoutTempFile);
                LOGGER.Debug("Loaded start layout");

                IEnumerable<DesktopApplicationTile> desktopApplicationTiles = startLayout
                        .defaultLayoutOverride?
                        .startLayoutCollection?
                        .startLayout?
                        .group?
                        .SelectMany(group => group.tiles)
                        .Where(tile => tile is DesktopApplicationTile)
                        .Cast<DesktopApplicationTile>()
                 ?? Enumerable.Empty<DesktopApplicationTile>();

                int savedManifestCount = 0;

                LOGGER.Debug("Parsing shortcuts");
                IEnumerable<Task> tasks = desktopApplicationTiles.Select(tile => Task.Run(() => {
                    ShellLinkFile shortcut = ShellLinkFile.Load(Environment.ExpandEnvironmentVariables(tile.desktopApplicationLinkPath));

                    if (shortcut.getTarget() is string destination) {
                        LOGGER.Trace("{0}: {1}", Path.GetFileNameWithoutExtension(tile.desktopApplicationLinkPath), destination);

                        string manifestFilename = Path.ChangeExtension(destination, ".VisualElementsManifest.xml");
                        if (!File.Exists(manifestFilename)) {
                            Application applicationManifest = visualElementsManifestEditor.CreateDefault();
                            applicationManifest.VisualElements.ForegroundText = TextBrightness.Light;
                            applicationManifest.VisualElements.ShowNameOnSquare150X150Logo = NameVisibility.On;
                            applicationManifest.VisualElements.BackgroundColor = TILE_BACKGROUND_COLOR;

                            visualElementsManifestEditor.Save(applicationManifest, manifestFilename);
                            LOGGER.Info("Saved {0}", manifestFilename);
                            Interlocked.Increment(ref savedManifestCount);
                        }
                    } else {
                        LOGGER.Warn("No destination in {0}", tile.desktopApplicationLinkPath);
                    }
                }));

                Task.WaitAll(tasks.ToArray());

                LOGGER.Info("Saved {0:N0} VisualElementsManifest file(s).", savedManifestCount);
            } finally {
                File.Delete(exportedLayoutTempFile);
            }
        }

        private static string exportStartLayoutToFile() {
            LOGGER.Debug("Exporting start layout");
            string exportedLayoutTempFile = Path.GetTempFileName();

            InitialSessionState initialSessionState = InitialSessionState.CreateDefault2();
            initialSessionState.ExecutionPolicy = ExecutionPolicy.RemoteSigned;
            var pipeline = PowerShell.Create(initialSessionState);

            LOGGER.Debug("PowerShell pipeline created");

            pipeline.AddCommand("Export-StartLayout")
                .AddParameter("Path", exportedLayoutTempFile);
            pipeline.Invoke();

            LOGGER.Debug("Exported start layout");
            return exportedLayoutTempFile;
        }

    }

}