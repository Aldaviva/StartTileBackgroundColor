using System.IO;
using System.Xml.Serialization;
using StartTileBackgroundColor.Data;

namespace StartTileBackgroundColor {

    public class StartLayoutEditor {

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(LayoutModificationTemplate));

        public LayoutModificationTemplate load(string filename) {
            using Stream fileStream = new FileStream(filename, FileMode.Open);
            return (LayoutModificationTemplate) serializer.Deserialize(fileStream);
        }

    }

}