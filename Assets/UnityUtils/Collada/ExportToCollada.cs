using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Collada141;


public class ExportToCollada
{
    private COLLADA collada;
    private asset ass;
    private library_geometries libraryGeometries;
    private library_visual_scenes visualScenes;
    private visual_scene visualScene;
    private List<node> nodes;
    private List<geometry> geometries;
    private library_materials libraryMaterials;
    private library_images libraryImages;
    private List<image> images;
    private List<material> materials;
    private library_effects libraryEffects;
    private List<effect> effects;
    
    private int idCounter;

    private string materialId;
    
    public int maxTextureSize = 64;
    

    private Dictionary<Mesh, string> meshLookup;

    string defaultMaterialName = "DefaultMaterial";
    private string workingPath;

    string CreateId()
    {
        return "ID"+(idCounter++);
    }
    
    public ExportToCollada(string workingPath)
    {
        this.workingPath = workingPath;
        collada = new COLLADA();
        
        geometries = new List<geometry>();
        nodes = new List<node>();

        meshLookup = new Dictionary<Mesh, string>();

        ass = new asset();

        ass.up_axis = UpAxisType.Y_UP;
        ass.unit = new assetUnit();
        ass.unit.meter = 1;
        ass.unit.name = "meter";
        libraryGeometries =  new library_geometries();
        // create effects
        libraryEffects = new library_effects();
        effects = new List<effect>();
        effects.Add(new effect());
        
        var effectId = CreateId();
        effects[0].id = effectId;
        effects[0].Items = new effectFx_profile_abstractProfile_COMMON[]
        {
            new effectFx_profile_abstractProfile_COMMON()
        };
        effects[0].Items[0].technique = new effectFx_profile_abstractProfile_COMMONTechnique();
        effects[0].Items[0].technique.sid = "COMMON";
        var phong = new effectFx_profile_abstractProfile_COMMONTechniquePhong();
        effects[0].Items[0].technique.Item = phong;
        SetPhong(phong, new Vector4(0.5f,0.5f,0.5f,1));


        // create materials
        libraryMaterials = new library_materials();
        materials = new List<material>();
        materials.Add(new material());
        
        materialId = CreateId();
        materials[0].id = materialId;

        materials[0].name = defaultMaterialName;
        materials[0].instance_effect = new instance_effect();
        materials[0].instance_effect.url = "#"+effectId;
        
        // create visual scenes
        visualScenes = new library_visual_scenes();

        visualScene = new visual_scene();
        visualScenes.visual_scene = new visual_scene[]
        {
            visualScene
        };
        var visualSceneId = CreateId();
        visualScenes.visual_scene[0].id = visualSceneId;

        libraryImages = new library_images();
        images = new List<image>();
        
        Build();
        
        collada.scene = new COLLADAScene();
        collada.scene.instance_visual_scene = new InstanceWithExtra();



        collada.scene.instance_visual_scene.url = "#"+visualSceneId;
    }

    private static void SetPhong(effectFx_profile_abstractProfile_COMMONTechniquePhong phong, Vector4 diffuse)
    {
        phong.diffuse = new common_color_or_texture_type();
        var col = new common_color_or_texture_typeColor();
        col.sid = "diffuse";
        phong.diffuse.Item = col;
        col.Values = new double[] {diffuse.x, diffuse.y, diffuse.z, diffuse.w};
        
        phong.emission = new common_color_or_texture_type();
        var emissionColor = new common_color_or_texture_typeColor();
        emissionColor.sid = "emission";
        emissionColor.Values = new double[] {0,0,0,1};
        phong.emission.Item = emissionColor; 
        
        phong.ambient = new common_color_or_texture_type();
        var ambientColor = new common_color_or_texture_typeColor();
        ambientColor.sid = "ambient";
        ambientColor.Values = new double[] {0,0,0,1};
        phong.ambient.Item = ambientColor;
            
        phong.specular = new common_color_or_texture_type();
        var specularColor = new common_color_or_texture_typeColor();
        specularColor.sid = "specular";
        specularColor.Values = new double[] {0.5,0.5,0.5,1};
        phong.specular.Item = specularColor;
        
        phong.shininess = new common_float_or_param_type();
        var f = new common_float_or_param_typeFloat();
        f.Value = 50;
        phong.shininess.Item = f;
        
        phong.index_of_refraction = new common_float_or_param_type();
        var f2 = new common_float_or_param_typeFloat();
        f2.Value = 1;
        phong.index_of_refraction.Item = f2;
        
        
    }
    
    private static void SetPhongTex(effectFx_profile_abstractProfile_COMMONTechniquePhong phong, string texture)
    {
        phong.diffuse = new common_color_or_texture_type();
        var col = new common_color_or_texture_typeTexture();
        phong.diffuse.Item = col;
        col.texture = texture;
        
        phong.emission = new common_color_or_texture_type();
        var emissionColor = new common_color_or_texture_typeColor();
        emissionColor.sid = "emission";
        emissionColor.Values = new double[] {0,0,0,1};
        phong.emission.Item = emissionColor; 
        
        phong.ambient = new common_color_or_texture_type();
        var ambientColor = new common_color_or_texture_typeColor();
        ambientColor.sid = "ambient";
        ambientColor.Values = new double[] {0,0,0,1};
        phong.ambient.Item = ambientColor;
            
        phong.specular = new common_color_or_texture_type();
        var specularColor = new common_color_or_texture_typeColor();
        specularColor.sid = "specular";
        specularColor.Values = new double[] {0.5,0.5,0.5,1};
        phong.specular.Item = specularColor;
        
        phong.shininess = new common_float_or_param_type();
        var f = new common_float_or_param_typeFloat();
        f.Value = 50;
        phong.shininess.Item = f;
        
        phong.index_of_refraction = new common_float_or_param_type();
        var f2 = new common_float_or_param_typeFloat();
        f2.Value = 1;
        phong.index_of_refraction.Item = f2;
    }

    static double[] ToDoubleArray(Vector3[] a)
    {
        double[] res = new double[a.Length*3];
        for (int i = 0; i < a.Length; i++)
        {
            res[i*3] = a[i].x;
            res[i*3+1] = a[i].y;
            res[i*3+2] = a[i].z;
        }
        return res;
    }

    static double[] ToDoubleArray(Vector2[] a)
    {
        double[] res = new double[a.Length*2];
        for (int i = 0; i < a.Length; i++)
        {
            res[i*2] = a[i].x;
            res[i*2+1] = a[i].y;
        }
        return res;
    }


    // return mesh id
    private string CreateMesh(Mesh mesh)
    {
        string id = "";
        if (meshLookup.TryGetValue(mesh, out id))
        {
            return id;
        }
        var geometry = new geometry();
        geometries.Add(geometry);
        id = CreateId();
        geometry.id = id;
        var m = new mesh();
        geometry.Item = m;

        m.source = new source[]
        {
            new source(),// position
            new source(),// normal
            new source(),// uvs
        };
        m.source[0].id = CreateId();
        var fa = new float_array();
        fa.id = CreateId();
        fa.Values = ToDoubleArray(mesh.vertices);
        fa.count = (ulong) fa.Values.Length;
        m.source[0].Item = fa;
        m.source[0].technique_common = new sourceTechnique_common();
        m.source[0].technique_common.accessor = new accessor();
        m.source[0].technique_common.accessor.count = fa.count / 3;
        m.source[0].technique_common.accessor.source = "#"+fa.id;
        m.source[0].technique_common.accessor.stride = 3;
        m.source[0].technique_common.accessor.param = new[]
        {
            new param(),
            new param(),
            new param()
        };
        m.source[0].technique_common.accessor.param[0].name = "X";
        m.source[0].technique_common.accessor.param[0].type = "float";
        m.source[0].technique_common.accessor.param[1].name = "Y";
        m.source[0].technique_common.accessor.param[1].type = "float";
        m.source[0].technique_common.accessor.param[2].name = "Z";
        m.source[0].technique_common.accessor.param[2].type = "float";

        m.source[1].id = CreateId();
        fa = new float_array();
        fa.id = CreateId();
        fa.Values = ToDoubleArray(mesh.normals);
        fa.count = (ulong) fa.Values.Length;
        m.source[1].Item = fa;
        m.source[1].technique_common = new sourceTechnique_common();
        m.source[1].technique_common.accessor = new accessor();
        m.source[1].technique_common.accessor.count = fa.count / 3;
        m.source[1].technique_common.accessor.source = "#"+fa.id;
        m.source[1].technique_common.accessor.stride = 3;
        m.source[1].technique_common.accessor.param = new[]
        {
            new param(),
            new param(),
            new param()
        };
        m.source[1].technique_common.accessor.param[0].name = "X";
        m.source[1].technique_common.accessor.param[0].type = "float";
        m.source[1].technique_common.accessor.param[1].name = "Y";
        m.source[1].technique_common.accessor.param[1].type = "float";
        m.source[1].technique_common.accessor.param[2].name = "Z";
        m.source[1].technique_common.accessor.param[2].type = "float";
        m.source[2].id = CreateId();
        fa = new float_array();
        fa.id = CreateId();
        fa.Values = ToDoubleArray(mesh.uv);
        fa.count = (ulong) fa.Values.Length;
        m.source[2].Item = fa;
        m.source[2].technique_common = new sourceTechnique_common();
        m.source[2].technique_common.accessor = new accessor();
        m.source[2].technique_common.accessor.count = fa.count / 2;
        m.source[2].technique_common.accessor.source = "#"+fa.id;
        m.source[2].technique_common.accessor.stride = 2;
        m.source[2].technique_common.accessor.param = new[]
        {
            new param(),
            new param()
        };
        m.source[2].technique_common.accessor.param[0].name = "X";
        m.source[2].technique_common.accessor.param[0].type = "float";
        m.source[2].technique_common.accessor.param[1].name = "Y";
        m.source[2].technique_common.accessor.param[1].type = "float";


        m.vertices = new vertices();
        m.vertices.id = CreateId();
        m.vertices.input = new InputLocal[]
        {
            new InputLocal(), // position
            new InputLocal(), // normal
            new InputLocal()  // uvs
        };
        m.vertices.input[0].semantic = "POSITION";
        m.vertices.input[0].source = "#"+m.source[0].id;
        m.vertices.input[1].semantic = "NORMAL";
        m.vertices.input[1].source = "#"+m.source[1].id;
        m.vertices.input[2].semantic = "TEXCOORD";
        m.vertices.input[2].source = "#"+m.source[2].id;

        var triangles = new List<int>();
        var stringWriter = new StringWriter();
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            var t = mesh.GetTriangles(i);
            triangles.AddRange(t);
            for (int j=0;j<t.Length;j=j+3)
            {
                stringWriter.Write("{0} ",t[j]);
                stringWriter.Write("{0} ",t[j+1]);
                stringWriter.Write("{0} ",t[j+2]);
            }
        }
        var tris = new triangles();
        tris.count = (ulong) (triangles.Count / 3);
        tris.material = defaultMaterialName;
        tris.input = new InputLocalOffset[]
        {
            new InputLocalOffset(),
            //new InputLocalOffset(),
            //new InputLocalOffset()
        };
        tris.input[0].offset = 0;
        tris.input[0].semantic = "VERTEX";
        tris.input[0].source= "#"+m.vertices.id;
        //           <input semantic="TEXCOORD" source="#Cube-mesh-map-0" offset="2" set="0"/>
        /*tris.input[1].offset = 1;
        tris.input[1].semantic = "NORMAL";
        tris.input[1].source= "#"+m.source[1].id;
        
        tris.input[2].offset = 2;
        tris.input[2].semantic = "TEXCOORD";
        tris.input[2].source= "#"+m.source[2].id;
        tris.input[2].set = 0;
        tris.input[2].setSpecified = true;*/
        tris.p = stringWriter.ToString();
        m.Items = new object[]
        {
            tris
        };
        meshLookup[mesh] = id;
        return id;
    }
    
    // return mesh id
    private string CreateMeshWithSubmeshes(Mesh mesh, List<string> materialNames)
    {
        string id = "";
        if (meshLookup.TryGetValue(mesh, out id))
        {
            return id;
        }
        var geometry = new geometry();
        geometries.Add(geometry);
        id = CreateId();
        geometry.id = id;
        var m = new mesh();
        geometry.Item = m;

        m.source = new source[]
        {
            new source(),// position
            new source(),// normal
            new source(),// uvs
        };
        m.source[0].id = CreateId();
        var fa = new float_array();
        fa.id = CreateId();
        fa.Values = ToDoubleArray(mesh.vertices);
        fa.count = (ulong) fa.Values.Length;
        m.source[0].Item = fa;
        m.source[0].technique_common = new sourceTechnique_common();
        m.source[0].technique_common.accessor = new accessor();
        m.source[0].technique_common.accessor.count = fa.count / 3;
        m.source[0].technique_common.accessor.source = "#"+fa.id;
        m.source[0].technique_common.accessor.stride = 3;
        m.source[0].technique_common.accessor.param = new[]
        {
            new param(),
            new param(),
            new param()
        };
        m.source[0].technique_common.accessor.param[0].name = "X";
        m.source[0].technique_common.accessor.param[0].type = "float";
        m.source[0].technique_common.accessor.param[1].name = "Y";
        m.source[0].technique_common.accessor.param[1].type = "float";
        m.source[0].technique_common.accessor.param[2].name = "Z";
        m.source[0].technique_common.accessor.param[2].type = "float";

        m.source[1].id = CreateId();
        fa = new float_array();
        fa.id = CreateId();
        fa.Values = ToDoubleArray(mesh.normals);
        fa.count = (ulong) fa.Values.Length;
        m.source[1].Item = fa;
        m.source[1].technique_common = new sourceTechnique_common();
        m.source[1].technique_common.accessor = new accessor();
        m.source[1].technique_common.accessor.count = fa.count / 3;
        m.source[1].technique_common.accessor.source = "#"+fa.id;
        m.source[1].technique_common.accessor.stride = 3;
        m.source[1].technique_common.accessor.param = new[]
        {
            new param(),
            new param(),
            new param()
        };
        m.source[1].technique_common.accessor.param[0].name = "X";
        m.source[1].technique_common.accessor.param[0].type = "float";
        m.source[1].technique_common.accessor.param[1].name = "Y";
        m.source[1].technique_common.accessor.param[1].type = "float";
        m.source[1].technique_common.accessor.param[2].name = "Z";
        m.source[1].technique_common.accessor.param[2].type = "float";
        m.source[2].id = CreateId();
        fa = new float_array();
        fa.id = CreateId();
        fa.Values = ToDoubleArray(mesh.uv);
        fa.count = (ulong) fa.Values.Length;
        m.source[2].Item = fa;
        m.source[2].technique_common = new sourceTechnique_common();
        m.source[2].technique_common.accessor = new accessor();
        m.source[2].technique_common.accessor.count = fa.count / 2;
        m.source[2].technique_common.accessor.source = "#"+fa.id;
        m.source[2].technique_common.accessor.stride = 2;
        m.source[2].technique_common.accessor.param = new[]
        {
            new param(),
            new param()
        };
        
        m.source[2].technique_common.accessor.param[0].name = "X";
        m.source[2].technique_common.accessor.param[0].type = "float";
        m.source[2].technique_common.accessor.param[1].name = "Y";
        m.source[2].technique_common.accessor.param[1].type = "float";

        m.vertices = new vertices();
        m.vertices.id = CreateId();
        m.vertices.input = new InputLocal[]
        {
            new InputLocal(), // position
        };
        m.vertices.input[0].semantic = "POSITION";
        m.vertices.input[0].source = "#"+m.source[0].id;
        

        var submeshes = new List<object>();
        
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            var triangles = new List<int>();
            var stringWriter = new StringWriter();
            
            var t = mesh.GetTriangles(i);
            triangles.AddRange(t);
            for (int j=0;j<t.Length;j=j+3)
            {    // index for position, normal and texcoord
                stringWriter.Write("{0} ",t[j]);
                stringWriter.Write("{0} ",t[j]);
                stringWriter.Write("{0} ",t[j]);
                stringWriter.Write("{0} ",t[j+1]);
                stringWriter.Write("{0} ",t[j+1]);
                stringWriter.Write("{0} ",t[j+1]);
                stringWriter.Write("{0} ",t[j+2]);
                stringWriter.Write("{0} ",t[j+2]);
                stringWriter.Write("{0} ",t[j+2]);
            }
            var tris = new triangles();
            tris.count = (ulong) (triangles.Count / 3);
            tris.material = materialNames[i];
            tris.input = new InputLocalOffset[]
            {
                new InputLocalOffset(),
                new InputLocalOffset(),
                new InputLocalOffset()
            };
            tris.input[0].offset = 0;
            tris.input[0].semantic = "VERTEX";
            tris.input[0].source= "#"+m.vertices.id;
            tris.p = stringWriter.ToString();
            
            tris.input[1].offset = 1;
            tris.input[1].semantic = "NORMAL";
            tris.input[1].source = "#"+m.source[1].id;
            
            tris.input[2].offset = 2;
            tris.input[2].semantic = "TEXCOORD";
            tris.input[2].source = "#"+m.source[2].id;
            tris.input[2].set = 0;
            
            submeshes.Add(tris);
        }
        
        m.Items = submeshes.ToArray();
        meshLookup[mesh] = id;
        return id;
    }

    private Dictionary<Material, string> materialCache;
    private Dictionary<Texture, Dictionary<Color32, string> > textureCache;

    private Texture2D MakeReadable(Texture2D texture, Color color, int maxSize)
    {
        int width = texture.width;
        int height = texture.height;
        float max = Mathf.Max(width, height);
        if (max > maxSize)
        {
            float scale = maxSize / max;
            width = (int) (width*scale);
            height = (int) (height*scale);
        }
        Material material = new Material(Shader.Find("Unlit/AlphaSelfIllum"));
        material.SetColor("_ColorX",color);
        // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary( 
            width,
            height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);


        Graphics.Blit(texture, tmp);                               // Blit the pixels on texture to the RenderTexture
        RenderTexture previous = RenderTexture.active;             // Backup the currently set RenderTexture
        RenderTexture.active = tmp;                                // Set the current RenderTexture to the temporary one we created
        Texture2D myTexture2D = new Texture2D(width, height);      // Create a new readable Texture2D to copy the pixels to it
        myTexture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0); // Copy the pixels from the RenderTexture to the new Texture
        myTexture2D.Apply();
        RenderTexture.active = previous;                            // Reset the active RenderTexture
        RenderTexture.ReleaseTemporary(tmp);                        // Release the temporary RenderTexture

        var pixels = myTexture2D.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = pixels[i] * color;
        }
        myTexture2D.SetPixels(pixels);
        return myTexture2D;                                        // "myTexture2D" now has the same pixels from "texture" and it's readable.
    }

    
    
    private string Color2String(Color32 c)
    {
        return "_" + c.r + "_" + c.g + "_" + c.b + "_" + c.a;
    }
    
    private List<string> GetMaterialNames(Material[] materials)
    {
        if (materialCache == null)
        {
            materialCache = new Dictionary<Material, string>();
        }
        if (textureCache == null)
        {
            textureCache = new Dictionary<Texture, Dictionary<Color32, string>>();
        }
        var res = new List<string>();
        for (int i = 0; i < materials.Length; i++)
        {
            Color color = Color.gray;
            if (materials[i].HasProperty("_Color")){
                color = materials[i].color;
            } else if (materials[i].HasProperty("_MainColor"))
            {
                color = materials[i].GetColor("_MainColor");
            }
            Color32 c32 = color;
            
            string textureName = "";
            if (materials[i].mainTexture != null)
            {
                if (textureCache.ContainsKey(materials[i].mainTexture))
                {

                    var colorDict = textureCache[materials[i].mainTexture];
                    colorDict.TryGetValue(c32, out textureName);
                }
                else
                {
                    textureCache[materials[i].mainTexture] = new Dictionary<Color32, string>();
                }
                if (textureName == ""){
                    if (!textureCache[materials[i].mainTexture].TryGetValue(c32, out textureName))
                    {
                        textureName = "tex_" + textureCache.Count + Color2String(c32);
                        textureCache[materials[i].mainTexture][c32] = textureName;
                        Texture2D t2d = (Texture2D)materials[i].mainTexture;
                        
                        byte[] bytes = MakeReadable(t2d,color,maxTextureSize).EncodeToPNG();
                        
                        File.WriteAllBytes(workingPath+"/"+textureName+".png",bytes);
    
                        var image = new image();
                        image.id = textureName;
                        image.name = textureName;
                        image.Item = textureName+".png"; 
                        images.Add(image);
                    }
                }
            }
            if (materialCache.ContainsKey(materials[i]))
            {
                res.Add(materialCache[materials[i]]);
            } 
            else 
            {
                // create effects
                effects.Add(new effect());
                var effect = effects[effects.Count-1];
                var effectId = CreateId();
                effect.id = effectId;
    
                if (textureName != "")
                {
                    effect.newparam = new fx_newparam_common[]
                    {
                        new fx_newparam_common(),
                        new fx_newparam_common()
                    };
                    effect.newparam[0].sid = textureName+"-surface";
                    effect.newparam[0].surface = new fx_surface_common();
                    effect.newparam[0].surface.type = fx_surface_type_enum.Item2D;
                    effect.newparam[0].surface.init_from = new fx_surface_init_from_common[]
                    {
                        new fx_surface_init_from_common()
                    };
                    effect.newparam[0].surface.init_from[0].Value = textureName;
                    
                    effect.newparam[1].sid = textureName+"-sampler";
                    effect.newparam[1].sampler2D = new fx_sampler2D_common();
                    effect.newparam[1].sampler2D.source = textureName+"-surface";
                }
                
                effect.Items = new effectFx_profile_abstractProfile_COMMON[]
                {
                    new effectFx_profile_abstractProfile_COMMON()
                };
                effect.Items[0].technique = new effectFx_profile_abstractProfile_COMMONTechnique();
                effect.Items[0].technique.sid = "COMMON";
                var phong = new effectFx_profile_abstractProfile_COMMONTechniquePhong();
                effect.Items[0].technique.Item = phong;
                
                phong.diffuse = new common_color_or_texture_type();
                //if (true)
                if (textureName == "")
                {
                    
                    SetPhong(phong, new Vector4(color.r, color.g, color.b, color.a));
                }
                else
                {
                    SetPhongTex(phong, textureName+"-sampler");
                }
                
                
                // create materials
                this.materials.Add(new material());
    
                var material = this.materials[this.materials.Count - 1]; 
                
                var materialId = "mat_"+CreateId();
                material.id = materialId;
    
                material.name = materials[i].name;
                material.instance_effect = new instance_effect();
                material.instance_effect.url = "#"+effectId;

                materialCache[materials[i]] = materialId; 
                res.Add(materialId);
            }
        }
        return res;
    }

    public void AddMeshWithMaterials(Mesh mesh, Material[] materials, Matrix4x4 mat_, string name)
    {
        if (mesh == null)
        {
            Debug.LogWarning("Mesh is null. Object name "+name);
            return;
        }
        var matNames = GetMaterialNames(materials);
        mat_ = Matrix4x4.Scale(new Vector3(-1,1,1)) * mat_;

        // create geometry
        var geometryId = CreateMeshWithSubmeshes(mesh, matNames);

        // create node
        var nodeInstance = new node();
        nodes.Add(nodeInstance);
        nodeInstance.name = name;
        nodeInstance.ItemsElementName = new ItemsChoiceType2[]
        {
            ItemsChoiceType2.matrix
        };
        nodeInstance.id = CreateId();
        var mat = new matrix();
        nodeInstance.Items = new object[]
        {
            mat
        };
        mat.Values = new double[16];
        for (int i = 0; i < 16; i++)
        {
            mat.Values[i] = mat_[i/4,i%4];
        }
        nodeInstance.instance_geometry = new instance_geometry[]
        {
            new instance_geometry()
        };
        nodeInstance.instance_geometry[0].url = "#" + geometryId;
        nodeInstance.instance_geometry[0].bind_material = new bind_material();
        nodeInstance.instance_geometry[0].bind_material.technique_common = new instance_material[matNames.Count];
        for (int i = 0; i < matNames.Count; i++)
        {
            var imat = new instance_material();
            imat.symbol = matNames[i];
            imat.target = "#"+matNames[i];
            nodeInstance.instance_geometry[0].bind_material.technique_common[i] = imat;
        }
    }

    public void AddMesh(Mesh mesh, Matrix4x4 mat_, string name)
    {
        if (mesh == null)
        {
            Debug.LogWarning("Mesh is null. Object name "+name);
            return;
        }
        mat_ = Matrix4x4.Scale(new Vector3(-1,1,1)) * mat_;

        // create geometry
        var geometryId = CreateMesh(mesh);

        // create node
        var nodeInstance = new node();
        nodes.Add(nodeInstance);
        nodeInstance.name = name;
        nodeInstance.ItemsElementName = new ItemsChoiceType2[]
        {
            ItemsChoiceType2.matrix
        };
        nodeInstance.id = CreateId();
        var mat = new matrix();
        nodeInstance.Items = new object[]
        {
            mat
        };
        mat.Values = new double[16];
        for (int i = 0; i < 16; i++)
        {
            mat.Values[i] = mat_[i/4,i%4];
        }
        nodeInstance.instance_geometry = new instance_geometry[]
        {
            new instance_geometry()
        };
        nodeInstance.instance_geometry[0].url = "#" + geometryId;
    }

    private void Build()
    {
        libraryImages.image = images.ToArray();
        libraryEffects.effect = (effect[])effects.ToArray();
        libraryMaterials.material = materials.ToArray();
        visualScene.node = nodes.ToArray();
        libraryGeometries.geometry = geometries.ToArray();
        collada.version = VersionType.Item141;
        collada.Items = new object[ ]
        {
            libraryImages,
            libraryMaterials,
            libraryEffects,
            libraryGeometries,
            visualScenes
        };
        collada.asset = ass;
    }

    public void Save(string filename)
    {
        Build();
        collada.Save(filename);
    }
}
