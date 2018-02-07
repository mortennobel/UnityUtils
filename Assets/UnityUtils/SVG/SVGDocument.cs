/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using System.IO;
using System.Xml;

public class SVGDocument : SVGNode
{
    public int width;
    public int height;

    public XmlDocument doc;

    public SVGDocument(int width, int height) : base(null)
    {

        this.width = width;
        this.height = height;
        doc = new XmlDocument( );

        //(1) the xml declaration is recommended, but not mandatory
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration( "1.0", "UTF-8", null );
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore( xmlDeclaration, root );

        //(2) string.Empty makes cleaner code
        XmlElement svg = doc.CreateElement( string.Empty, "svg", string.Empty );
        xmlElement = svg;
        svg.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
        svg.SetAttribute("version", "1.2");
        svg.SetAttribute("baseProfile", "tiny");
        svg.SetAttribute("viewBox", "0 0 "+width+" "+height);
        svg.SetAttribute("shape-rendering", "crispEdges");

        doc.AppendChild( svg );

        XmlElement element2 = doc.CreateElement( string.Empty, "desc", string.Empty );
        XmlText text1 = doc.CreateTextNode( "SVG Export by Morten Nobel-Joergensen" );
        element2.AppendChild(text1);
        svg.AppendChild( element2 );
    }

    public string ToString()
    {
        StringWriter w = new StringWriter();
        doc.Save(w);
        return w.ToString();
    }
}
