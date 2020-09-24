using System;
using ShellLink;
using ShellLink.Flags;

namespace StartTileBackgroundColor {

    public static class ShortcutExtensions {

        public static string? GetDestination(this Shortcut shortcut) {
            if ((shortcut.LinkFlags & LinkFlags.PreferEnvironmentPath) != 0) {
                return Environment.ExpandEnvironmentVariables(shortcut.ExtraData.EnvironmentVariableDataBlock.TargetUnicode);
            } else if ((shortcut.LinkFlags & LinkFlags.HasLinkTargetIDList) != 0) {
                return shortcut.LinkTargetIDList.Path;
            } else {
                return null;
            }
        }

    }

}