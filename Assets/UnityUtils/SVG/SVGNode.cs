using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SVGNode
{
    public readonly SVGNode parent;

    public XmlElement xmlElement;

    protected XmlDocument GetDocument()
    {
        SVGNode n = parent;
        while (n.parent != null)
        {
            n = n.parent;
        }
        SVGDocument doc = n as SVGDocument;
        return doc.doc;
    }

    public SVGNode(SVGNode parent)
    {
        this.parent = parent;
    }

    public SVGGroup CreateGroup(string name)
    {
        return new SVGGroup(this, name);
    }

    public SVGPolygon CreatePolygon(Color fill, List<Vector2D> vertices, bool debugOutput = false)
    {
        return new SVGPolygon(this, fill, vertices, debugOutput);
    }
}
