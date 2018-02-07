/*
 *  UnityUtils (https://github.com/mortennobel/UnityUtils)
 *
 *  Created by Morten Nobel-Jørgensen ( http://www.nobel-joergnesen.com/ )
 *  License: MIT
 */

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SVGPolygon : SVGNode
{
    public SVGPolygon(SVGNode svgNode, Color fill, List<Vector2D> vertices, bool debugOutput = false)
    :base(svgNode)
    {
        var doc = GetDocument();
        xmlElement = doc.CreateElement("polygon");
        parent.xmlElement.AppendChild(xmlElement);
        xmlElement.SetAttribute("fill", ColorToHex(fill));
        if (debugOutput)
        {
            xmlElement.SetAttribute("stroke", "black");
            xmlElement.SetAttribute("stroke-width", "1");
        }

        //xmlElement.SetAttribute("stroke-linecap", "round");
        StringWriter sw = new StringWriter();
        bool first = true;
        foreach (var v in vertices)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sw.Write(" ");
            }
            sw.Write("{0},{1}", v.x.ToString("R"), v.y.ToString("R"));
        }
        xmlElement.SetAttribute("points", sw.ToString());
    }

    string ColorToHex(Color c)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}",
            Mathf.RoundToInt(c.r * 255),
            Mathf.RoundToInt(c.g * 255),
            Mathf.RoundToInt(c.b * 255));
    }
}
