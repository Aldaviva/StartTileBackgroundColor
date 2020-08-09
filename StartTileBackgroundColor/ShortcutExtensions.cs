using System;
using System.Linq;
using Shellify;
using Shellify.Core;
using Shellify.ExtraData;

namespace StartTileBackgroundColor {

    public static class ShortcutExtensions {

        public static string? getTarget(this ShellLinkFile shortcut) {
            string? target = null;
            if ((shortcut.Header.LinkFlags & LinkFlags.PreferEnvironmentPath) != 0 &&
                shortcut.ExtraDataBlocks.FirstOrDefault(block => block.Signature == ExtraDataBlockSignature.EnvironmentVariableDataBlock) is EnvironmentVariableDataBlock dataBlock) {

                target = Environment.ExpandEnvironmentVariables(dataBlock.ValueUnicode);
            } else if ((shortcut.Header.LinkFlags & LinkFlags.HasLinkInfo) != 0) {
                target = shortcut.LinkInfo.LocalBasePath;
            }

            return target;
        }

    }

}