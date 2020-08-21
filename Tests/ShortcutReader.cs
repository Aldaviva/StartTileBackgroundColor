using System.Text;
using Lnk;
using ShellLink;
using Xunit;

#nullable enable

namespace Tests {

    public class ShortcutReader {

        private const string LNK_FILENAME = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Games\WRC 8.lnk";
        private const string EXPECTED_TARGET = @"G:\WRC 8\WRC8.exe";

        [Fact]
        public void lnk() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            LnkFile lnkFile = Lnk.Lnk.LoadFile(LNK_FILENAME);
            Assert.True((lnkFile.Header.DataFlags & Header.DataFlag.HasTargetIdList) != 0);
            string actual = "";
            Assert.Equal(EXPECTED_TARGET, actual);
        }

        [Fact]
        public void lnkReader() {
            var lnkFile = LnkReader.Lnk.OpenLnk(LNK_FILENAME);
            string actual = lnkFile.BasePath;
            Assert.Equal(EXPECTED_TARGET, actual);
        }

        [Fact]
        public void shellLink() {
            Shortcut lnkFile = ShellLink.Shortcut.ReadFromFile(LNK_FILENAME);
            string actual = lnkFile.LinkTargetIDList.Path;
            Assert.Equal(EXPECTED_TARGET, actual);
        }

    }

}