using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Generic obj export
public class ExportToObj
{
    public List<Vector3> positions;
    public List<Vector3> normals;
    public List<Vector2> uvs;
    public List<int> indices;

    private List<string> objectNames;
    private List<int> objectIndices;

    private bool exportNormals;
    private bool exportUvs;
    public bool flipX = true;

    public string name;

    public ExportToObj(bool exportNormals = true, bool exportUvs = true)
    {
        this.exportNormals = exportNormals;
        this.exportUvs= exportUvs;
        positions = new List<Vector3>();
        normals = new List<Vector3>();
        uvs = new List<Vector2>();
        indices = new List<int>();
        objectNames = new List<string>();
        objectIndices = new List<int>();
    }

    public void AddObject(string objectName)
    {
        objectNames.Add(objectName);
        objectIndices.Add(indices.Count);
    }

    public void AddMesh(Mesh mesh, Matrix4x4 mat)
    {
        if (mesh == null)
        {
            return;
        }
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            AddMesh(mesh,i,mat);
        }
    }

    public void AddTriangle(Vector3[] mPosition, Vector3[] mNormals = null, Vector2[] mUvs = null)
    {
        int offset = positions.Count;
        indices.Add(offset);
        indices.Add(offset+1);
        indices.Add(offset+2);

        positions.Add(mPosition[0]);
        positions.Add(mPosition[1]);
        positions.Add(mPosition[2]);

        if (exportNormals)
        {
            if (mNormals == null)
            {
                Debug.LogError("mNormals is null");
                return;
            }
            normals.Add(mNormals[0]);
            normals.Add(mNormals[1]);
            normals.Add(mNormals[2]);
        }
        if (exportUvs)
        {
            if (mUvs == null)
            {
                Debug.LogError("mUvs is null");
                return;
            }
            uvs.Add(mUvs[0]);
            uvs.Add(mUvs[1]);
            uvs.Add(mUvs[2]);
        }
    }

    public void AddMesh(Mesh mesh, int subMesh, Matrix4x4 mat)
    {
        var normalTrans = mat.inverse.transpose;
        int offset = positions.Count;
        var tris = mesh.GetTriangles(subMesh);
        var mVertices = mesh.vertices;
        var mNormals = mesh.normals;
        var mUvs = mesh.uv;
        for (int i = 0; i < tris.Length; i = i + 3)
        {
            indices.Add(i+offset);
            indices.Add(i+offset+1);
            indices.Add(i+offset+2);

            positions.Add(mVertices[tris[i]]);
            positions.Add(mVertices[tris[i+1]]);
            positions.Add(mVertices[tris[i+2]]);

            if (exportNormals)
            {
                if (mNormals == null)
                {
                    normals.Add(Vector3.zero);
                    normals.Add(Vector3.zero);
                    normals.Add(Vector3.zero);
                }
                else
                {
                    normals.Add(mNormals[tris[i]]);
                    normals.Add(mNormals[tris[i+1]]);
                    normals.Add(mNormals[tris[i+2]]);
                }

            }
            if (exportUvs)
            {
                if (mUvs.Length == 0)
                {
                    uvs.Add(Vector2.zero);
                    uvs.Add(Vector2.zero);
                    uvs.Add(Vector2.zero);
                }
                else
                {
                    uvs.Add(mUvs[tris[i]]);
                    uvs.Add(mUvs[tris[i+1]]);
                    uvs.Add(mUvs[tris[i+2]]);
                }

            }
        }
        for (int i = offset; i < positions.Count; i++)
        {
            positions[i] = mat.MultiplyPoint(positions[i]);
        }
        for (int i = offset; i < normals.Count; i++)
        {
            normals[i] = normalTrans.MultiplyVector(normals[i]);
        }
    }

    public string ToString()
    {
        StringWriter st = new StringWriter();
        Save(st);
        return st.GetStringBuilder().ToString();
    }

    public void Save(TextWriter writer)
    {
        writer.Write("# Custom geometry export (UNITY) OBJ File: '"+name+"'\n");
        writer.Write("o ");
        writer.Write(name);
        writer.Write("\n");
        float xScale = flipX?-1:0;
        foreach (var v in positions){
            writer.Write("v {0} {1} {2}\n", v.x*xScale,v.y,v.z);
        }
        foreach (var u in uvs){
            writer.Write("vt {0} {1}\n", 1.0f-u.x,u.y);
        }
        foreach (var n in normals){
            writer.Write("v {0} {1} {2}\n", n.x*xScale,n.y,n.z);
        }
        var formatStr = "f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n";
        if (exportNormals && exportUvs == false)
        {
            formatStr = "f {0}//{0} {1}//{1} {2}//{2}\n";
        }
        if (exportNormals == false && exportUvs )
        {
            formatStr = "f {0}/{0} {1}/{1} {2}/{2}\n";
        }
        if (exportNormals == false && exportUvs == false )
        {
            formatStr = "f {0} {1} {2}\n";
        }
        int objectId = 0;
        for (int i=0;i<indices.Count;i=i+3){
            if (objectIndices.Count > objectId && objectIndices[objectId] == i)
            {
                writer.Write("o ");
                writer.Write(objectNames[objectId]);
                writer.Write("\n");
                objectId++;
            }
            if (flipX)
            {
                // inverse winding order
                writer.Write(formatStr,
                    indices[0+i]+1,
                    indices[2+i]+1,
                    indices[1+i]+1);
            }
            else
            {
                writer.Write(formatStr,
                    indices[0+i]+1,
                    indices[1+i]+1,
                    indices[2+i]+1);
            }
        }
    }
}
