public class SVGGroup : SVGNode{

    public SVGGroup(SVGNode parent, string name) : base(parent)
    {
        var doc = GetDocument();
        xmlElement = doc.CreateElement("g");
        parent.xmlElement.AppendChild(xmlElement);
        xmlElement.SetAttribute("name", name);
    }
}
