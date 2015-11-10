using UnityEngine;
using System.Collections;


/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.

Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
*/
//[ExecuteInEditMode()]
[AddComponentMenu("Mesh/Antares/Combine Children Extanded")]
public class CombineChildrenExtanded : MonoBehaviour {
	
	/// Usually rendering with triangle strips is faster.
	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
	/// Best is to try out which value is faster in practice.
    public int frameToWait = 0;
	public bool generateTriangleStrips = true, combineOnStart = true, destroyAfterOptimized = false, castShadow = true, receiveShadow = true, keepLayer = true, addMeshCollider = false;
	
    void Start()
    {
        if (combineOnStart && frameToWait == 0) Combine();
        else StartCoroutine(CombineLate());
    }

    IEnumerator CombineLate()
    {
        for (int i = 0; i < frameToWait; i++ ) yield return 0;
        Combine();
    }

    [ContextMenu("Combine Now on Childs")]
    public void CallCombineOnAllChilds()
    {
        CombineChildrenExtanded[] c = gameObject.GetComponentsInChildren<CombineChildrenExtanded>();
        int count = c.Length;
        for (int i = 0; i < count; i++) if(c[i] != this)c[i].Combine();
        combineOnStart = enabled = false;
    }

	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
    [ContextMenu ("Combine Now")]
	public void Combine () {
		Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Hashtable materialToMesh= new Hashtable();
		
		for (int i=0;i<filters.Length;i++) {
			MeshFilter filter = (MeshFilter)filters[i];
			Renderer curRenderer  = filters[i].renderer;
			MeshCombineUtilityIncluded.MeshInstance instance = new MeshCombineUtilityIncluded.MeshInstance ();
			instance.mesh = filter.sharedMesh;
			if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
				Material[] materials = curRenderer.sharedMaterials;
				for (int m=0;m<materials.Length;m++) {
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
					ArrayList objects = (ArrayList)materialToMesh[materials[m]];
					if (objects != null) {
						objects.Add(instance);
					}
					else
					{
						objects = new ArrayList ();
						objects.Add(instance);
						materialToMesh.Add(materials[m], objects);
					}
				}
                if (Application.isPlaying && destroyAfterOptimized && combineOnStart) Destroy(curRenderer.gameObject);
                else if (destroyAfterOptimized) DestroyImmediate(curRenderer.gameObject);
				else curRenderer.enabled = false;
			}
		}
	
		foreach (DictionaryEntry de  in materialToMesh) {
			ArrayList elements = (ArrayList)de.Value;
			MeshCombineUtilityIncluded.MeshInstance[] instances = (MeshCombineUtilityIncluded.MeshInstance[])elements.ToArray(typeof(MeshCombineUtilityIncluded.MeshInstance));

			// We have a maximum of one material, so just attach the mesh to our own game object
			if (materialToMesh.Count == 1)
			{
				// Make sure we have a mesh filter & renderer
				if (GetComponent(typeof(MeshFilter)) == null)
					gameObject.AddComponent(typeof(MeshFilter));
				if (!GetComponent("MeshRenderer"))
					gameObject.AddComponent("MeshRenderer");
	
				MeshFilter filter = (MeshFilter)GetComponent(typeof(MeshFilter));
                if (Application.isPlaying) filter.mesh = MeshCombineUtilityIncluded.Combine(instances, generateTriangleStrips);
                else filter.sharedMesh = MeshCombineUtilityIncluded.Combine(instances, generateTriangleStrips);
                if (Application.isPlaying) renderer.material = (Material)de.Key;
                else renderer.sharedMaterial = (Material)de.Key;
				renderer.enabled = true;
                if (addMeshCollider) gameObject.AddComponent<MeshCollider>();
                renderer.castShadows = castShadow;
                renderer.receiveShadows = receiveShadow;
			}
			// We have multiple materials to take care of, build one mesh / gameobject for each material
			// and parent it to this object
			else
			{
				GameObject go = new GameObject("Combined mesh");
                if (keepLayer) go.layer = gameObject.layer;
				go.transform.parent = transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent("MeshRenderer");
				go.renderer.material = (Material)de.Key;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				if(Application.isPlaying)filter.mesh = MeshCombineUtilityIncluded.Combine(instances, generateTriangleStrips);
                else filter.sharedMesh = MeshCombineUtilityIncluded.Combine(instances, generateTriangleStrips);
                go.renderer.castShadows = castShadow;
                go.renderer.receiveShadows = receiveShadow;
                if (addMeshCollider) go.AddComponent<MeshCollider>();
            }
		}	
	}	
}

public class MeshCombineUtilityIncluded
{

    public struct MeshInstance
    {
        public Mesh mesh;
        public int subMeshIndex;
        public Matrix4x4 transform;
    }

    public static Mesh Combine(MeshInstance[] combines, bool generateStrips)
    {
        int vertexCount = 0;
        int triangleCount = 0;
        int stripCount = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                vertexCount += combine.mesh.vertexCount;

                if (generateStrips)
                {
                    // SUBOPTIMAL FOR PERFORMANCE
                    int curStripCount = combine.mesh.GetTriangles(combine.subMeshIndex).Length;
                    if (curStripCount != 0)
                    {
                        if (stripCount != 0)
                        {
                            if ((stripCount & 1) == 1)
                                stripCount += 3;
                            else
                                stripCount += 2;
                        }
                        stripCount += curStripCount;
                    }
                    else
                    {
                        generateStrips = false;
                    }
                }
            }
        }

        // Precomputed how many triangles we need instead
        if (!generateStrips)
        {
            foreach (MeshInstance combine in combines)
            {
                if (combine.mesh)
                {
                    triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
                }
            }
        }

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector4[] tangents = new Vector4[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        Vector2[] uv1 = new Vector2[vertexCount];
        Color[] colors = new Color[vertexCount];

        int[] triangles = new int[triangleCount];
        int[] strip = new int[stripCount];

        int offset;

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 invTranspose = combine.transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
            }

        }
        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                Matrix4x4 invTranspose = combine.transform;
                invTranspose = invTranspose.inverse.transpose;
                CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
            }

        }
        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset);
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                Copy(combine.mesh.vertexCount, combine.mesh.uv1, uv1, ref offset);
        }

        offset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
                CopyColors(combine.mesh.vertexCount, combine.mesh.colors, colors, ref offset);
        }

        int triangleOffset = 0;
        int stripOffset = 0;
        int vertexOffset = 0;
        foreach (MeshInstance combine in combines)
        {
            if (combine.mesh)
            {
                if (generateStrips)
                {
                    int[] inputstrip = combine.mesh.GetTriangles(combine.subMeshIndex);
                    if (stripOffset != 0)
                    {
                        if ((stripOffset & 1) == 1)
                        {
                            strip[stripOffset + 0] = strip[stripOffset - 1];
                            strip[stripOffset + 1] = inputstrip[0] + vertexOffset;
                            strip[stripOffset + 2] = inputstrip[0] + vertexOffset;
                            stripOffset += 3;
                        }
                        else
                        {
                            strip[stripOffset + 0] = strip[stripOffset - 1];
                            strip[stripOffset + 1] = inputstrip[0] + vertexOffset;
                            stripOffset += 2;
                        }
                    }

                    for (int i = 0; i < inputstrip.Length; i++)
                    {
                        strip[i + stripOffset] = inputstrip[i] + vertexOffset;
                    }
                    stripOffset += inputstrip.Length;
                }
                else
                {
                    int[] inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
                    for (int i = 0; i < inputtriangles.Length; i++)
                    {
                        triangles[i + triangleOffset] = inputtriangles[i] + vertexOffset;
                    }
                    triangleOffset += inputtriangles.Length;
                }

                vertexOffset += combine.mesh.vertexCount;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Combined Mesh";
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.uv1 = uv1;
        mesh.tangents = tangents;
        if (generateStrips)
            mesh.SetTriangles(strip, 0);
        else
            mesh.triangles = triangles;

        return mesh;
    }

    static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyPoint(src[i]);
        offset += vertexcount;
    }

    static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
        offset += vertexcount;
    }

    static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = src[i];
        offset += vertexcount;
    }

    static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
    {
        for (int i = 0; i < src.Length; i++)
            dst[i + offset] = src[i];
        offset += vertexcount;
    }

    static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
    {
        for (int i = 0; i < src.Length; i++)
        {
            Vector4 p4 = src[i];
            Vector3 p = new Vector3(p4.x, p4.y, p4.z);
            p = transform.MultiplyVector(p).normalized;
            dst[i + offset] = new Vector4(p.x, p.y, p.z, p4.w);
        }

        offset += vertexcount;
    }
}