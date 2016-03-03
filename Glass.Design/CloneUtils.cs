using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

namespace Glass.Design {

    public static class CloneUtils {

        public static Visual CloneWithSameAppearance(this Control visual) {

            var gridXaml = XamlWriter.Save(visual);

            var stringReader = new StringReader(gridXaml);
            var xmlReader = XmlReader.Create(stringReader);
            return (Visual) XamlReader.Load(xmlReader);
        }

    }
}
