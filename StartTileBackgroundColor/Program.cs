using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using LnkReader;
using Microsoft.PowerShell;
using NLog;
using StartTileBackgroundColor.Data;
using VisualElementsManifest;
using VisualElementsManifest.Data;

namespace StartTileBackgroundColor {

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
                    string shortcutFilename = Environment.ExpandEnvironmentVariables(tile.desktopApplicationLinkPath);
                    var shortcut = Lnk.OpenLnk(shortcutFilename);

                    if (shortcut.BasePath is string destination) {
                        LOGGER.Trace("{0}: {1}", Path.GetFileNameWithoutExtension(shortcutFilename), destination);

                        string manifestFilename = Path.ChangeExtension(destination, ".VisualElementsManifest.xml");
                        if (!File.Exists(manifestFilename)) {
                            Application applicationManifest = visualElementsManifestEditor.CreateDefault();
                            applicationManifest.VisualElements.ForegroundText = TextBrightness.Light;
                            applicationManifest.VisualElements.ShowNameOnSquare150X150Logo = NameVisibility.On;
                            applicationManifest.VisualElements.BackgroundColor = TILE_BACKGROUND_COLOR;

                            visualElementsManifestEditor.Save(applicationManifest, manifestFilename);
                            File.SetLastWriteTime(shortcutFilename, DateTime.Now);
                            
                            LOGGER.Info("Saved {0}", manifestFilename);
                            Interlocked.Increment(ref savedManifestCount);
                        }
                    } else {
                        LOGGER.Warn("No destination in {0}", shortcutFilename);
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