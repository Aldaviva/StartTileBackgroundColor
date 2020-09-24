using System.Collections.Generic;
using System.Text;
using Lnk;
using ShellLink;
using StartTileBackgroundColor;
using Xunit;

namespace Tests {

    public class ShortcutReader {

        [Theory]
        [MemberData(nameof(theories))]
        public void lnk(string lnkFilename, string expectedTarget) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            LnkFile lnkFile = Lnk.Lnk.LoadFile(lnkFilename);
            Assert.True((lnkFile.Header.DataFlags & Header.DataFlag.HasTargetIdList) != 0);
            string actual = lnkFile.LocalPath;
            Assert.Equal(expectedTarget, actual);
        }

        [Theory]
        [MemberData(nameof(theories))]
        public void lnkReader(string lnkFilename, string expectedTarget) {
            var    lnkFile = LnkReader.Lnk.OpenLnk(lnkFilename);
            string actual  = lnkFile.BasePath;
            Assert.Equal(expectedTarget, actual);
        }

        [Theory]
        [MemberData(nameof(theories))]
        public void shellLink(string lnkFilename, string expectedTarget) {
            Shortcut lnkFile = Shortcut.ReadFromFile(lnkFilename);
            string?  actual  = lnkFile.GetDestination();
            Assert.Equal(expectedTarget, actual);
        }

        public static IEnumerable<object[]> theories => new List<string[]> {
            new[] { @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Games\WRC 8.lnk", @"G:\WRC 8\WRC8.exe" },
            new[] { @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Games\Superhot꞉ Mind Control Delete.lnk", @"G:\Superhot - Mind Control Delete\SUPERHOTMCD.exe" },
            new[] { @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Games\American Truck Simulator.lnk", @"G:\Steam\steamapps\common\American Truck Simulator\bin\win_x64\amtrucks.exe" },
        };

    }

}