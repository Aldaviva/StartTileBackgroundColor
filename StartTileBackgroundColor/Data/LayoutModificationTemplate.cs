using System.Collections.Generic;
using System.Xml.Serialization;

namespace StartTileBackgroundColor.Data {

    internal readonly struct Namespaces {

        internal const string STARTLAYOUT = "http://schemas.microsoft.com/Start/2014/StartLayout";
        internal const string LAYOUTMODIFICATION = "http://schemas.microsoft.com/Start/2014/LayoutModification";
        internal const string FULLDEFAULTLAYOUT = "http://schemas.microsoft.com/Start/2014/FullDefaultLayout";

    }

    [XmlType(TypeName = "LayoutOptions", Namespace = Namespaces.LAYOUTMODIFICATION)]
    public class LayoutOptions {

        [XmlAttribute(AttributeName = "StartTileGroupCellWidth")]
        public int startTileGroupCellWidth { get; set; }

    }

    [XmlType(TypeName = "Group", Namespace = Namespaces.STARTLAYOUT)]
    public class Group { 

        [XmlElement(ElementName = "DesktopApplicationTile", Type = typeof(DesktopApplicationTile), Namespace = Namespaces.STARTLAYOUT),
         XmlElement(ElementName = "Tile", Type = typeof(ImmersiveApplicationTile), Namespace = Namespaces.STARTLAYOUT)]
        public List<AbstractTile> tiles { get; set; } = new List<AbstractTile>();

        [XmlAttribute(AttributeName = "Name")]
        public string? name { get; set; }

    }

    public abstract class AbstractTile {

        [XmlAttribute(AttributeName = "Size")]
        public string? size { get; set; }

        [XmlAttribute(AttributeName = "Column")]
        public int column { get; set; }

        [XmlAttribute(AttributeName = "Row")]
        public int row { get; set; }

    }

    [XmlType(TypeName = "Tile", Namespace = Namespaces.STARTLAYOUT)]
    public class ImmersiveApplicationTile: AbstractTile {

        [XmlAttribute(AttributeName = "AppUserModelID")]
        public string appUserModelId { get; set; } = string.Empty;

    }

    [XmlType(TypeName = "DesktopApplicationTile", Namespace = Namespaces.STARTLAYOUT)]
    public class DesktopApplicationTile: AbstractTile {

        [XmlAttribute(AttributeName = "DesktopApplicationLinkPath")]
        public string desktopApplicationLinkPath { get; set; }

    }

    [XmlType(TypeName = "StartLayout", Namespace = Namespaces.FULLDEFAULTLAYOUT)]
    public class StartLayout {

        [XmlElement(ElementName = "Group", Namespace = Namespaces.STARTLAYOUT)]
        public List<Group>? @group { get; set; }

        [XmlAttribute(AttributeName = "GroupCellWidth")]
        public int groupCellWidth { get; set; }

    }

    [XmlType(TypeName = "StartLayoutCollection", Namespace = Namespaces.LAYOUTMODIFICATION)]
    public class StartLayoutCollection {

        [XmlElement(ElementName = "StartLayout", Namespace = Namespaces.FULLDEFAULTLAYOUT)]
        public StartLayout? startLayout { get; set; }

    }

    [XmlType(TypeName = "DefaultLayoutOverride", Namespace = Namespaces.LAYOUTMODIFICATION)]
    public class DefaultLayoutOverride {

        [XmlElement(ElementName = "StartLayoutCollection", Namespace = Namespaces.LAYOUTMODIFICATION)]
        public StartLayoutCollection? startLayoutCollection { get; set; }

    }

    [XmlRoot(ElementName = "LayoutModificationTemplate", Namespace = Namespaces.LAYOUTMODIFICATION)]
    public class LayoutModificationTemplate {

        [XmlElement(ElementName = "LayoutOptions", Namespace = Namespaces.LAYOUTMODIFICATION)]
        public LayoutOptions? layoutOptions { get; set; }

        [XmlElement(ElementName = "DefaultLayoutOverride", Namespace = Namespaces.LAYOUTMODIFICATION)]
        public DefaultLayoutOverride? defaultLayoutOverride { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string? version { get; set; }

    }

}