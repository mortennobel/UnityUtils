/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

public class SVGGroup : SVGNode{

    public SVGGroup(SVGNode parent, string name) : base(parent)
    {
        var doc = GetDocument();
        xmlElement = doc.CreateElement("g");
        parent.xmlElement.AppendChild(xmlElement);
        xmlElement.SetAttribute("name", name);
    }
}
