﻿// creates a circuit board of given (2D) dimensions.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CircuitBoard : MonoBehaviour
{

    // dimensions of the board
    public int x;
    public int z;

    public BoxCollider ThisCollider;
    public Renderer Renderer;
    public Mesh ThisMesh;

    private void Awake()
    {
        BoardColor = GetComponent<Renderer>().material.color;
        ThisCollider = GetComponent<BoxCollider>();
        Renderer = GetComponent<Renderer>();
        ThisMesh = GetComponent<MeshFilter>().mesh;
    }

    // code is modified from http://ilkinulas.github.io/development/unity/2016/04/30/cube-mesh-in-unity3d.html
    [Button]
    public void CreateCuboid()
    {
        // 3 sets are needed because each vertex of the cuboid is actually 3 vertexes - 1 for each face using it. Vertexes are not reused between faces because that causes visual errors due to messed up normals
        Vector3[] vertices = {
            // set 1
            new Vector3 (0, -0.5f, 0) * 0.15f, // each element is *0.3 because that's what I've decided the global scale shall be, and then by 0.5 because idk it just works
            new Vector3 (2 * x, -0.5f, 0) * 0.15f,
            new Vector3 (2 * x, 0.5f, 0) * 0.15f,
            new Vector3 (0, 0.5f, 0) * 0.15f,
            new Vector3 (0, 0.5f, 2 * z) * 0.15f,
            new Vector3 (2 * x, 0.5f, 2 * z) * 0.15f,
            new Vector3 (2 * x, -0.5f, 2 * z) * 0.15f,
            new Vector3 (0, -0.5f, 2 * z) * 0.15f,

            // set 2
            new Vector3 (0, -0.5f, 0) * 0.15f,
            new Vector3 (2 * x, -0.5f, 0) * 0.15f,
            new Vector3 (2 * x, 0.5f, 0) * 0.15f,
            new Vector3 (0, 0.5f, 0) * 0.15f,
            new Vector3 (0, 0.5f, 2 * z) * 0.15f,
            new Vector3 (2 * x, 0.5f, 2 * z) * 0.15f,
            new Vector3 (2 * x, -0.5f, 2 * z) * 0.15f,
            new Vector3 (0, -0.5f, 2 * z) * 0.15f,

            // set 3
            new Vector3 (0, -0.5f, 0) * 0.15f,
            new Vector3 (2 * x, -0.5f, 0) * 0.15f,
            new Vector3 (2 * x, 0.5f, 0) * 0.15f,
            new Vector3 (0, 0.5f, 0) * 0.15f,
            new Vector3 (0, 0.5f, 2 * z) * 0.15f,
            new Vector3 (2 * x, 0.5f, 2 * z) * 0.15f,
            new Vector3 (2 * x, -0.5f, 2 * z) * 0.15f,
            new Vector3 (0, -0.5f, 2 * z) * 0.15f,
        };

        int[] triangles = {
            // using set 1
            0, 2, 1, //face front
			0, 3, 2,
            5, 4, 7, //face back
			5, 7, 6,

            // using set 2
            10, 11, 12, //face top
			10, 12, 13,
            8, 14, 15, //face bottom
			8, 9, 14,

            // using set 3
            17, 18, 21, //face right
			17, 21, 22,
            16, 23, 20, //face left
			16, 20, 19,
        };

        ThisMesh.Clear();
        ThisMesh.vertices = vertices;
        ThisMesh.triangles = triangles;

        // assign uv maps. No idea how or why this works, code copied directly from the unity manual page for mesh.uv
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        ThisMesh.uv = uvs;

        //mesh.Optimize(); // this is obsolete? Well shit, guess my meshes ain't gettin optimized
        ThisMesh.RecalculateNormals();

        // make collider the proper size and position
        if (ThisCollider != null) // the null check is for the main menu, where it doesn't have colliders
        {
            ThisCollider.size = new Vector3(x, 0.5f, z) * 0.3f;
            ThisCollider.center = new Vector3(x, 0, z) * 0.15f;
        }
    }


    // uses a boxcast to determine if there's an object there
    // used by StuffPlacer to determine if an object can be placed there
    public bool SpaceOccupied(Vector2Int position, bool top)
    {
        if(position.x > x || position.y > z) // don't let stuff be placed there if it's outside the bounds of the box
        {
            return true;
        }

        Vector3 CastCenter = new Vector3(position.x * 0.3f + 0.15f, -0.075f, position.y * 0.3f + 0.15f);
        if (top)
        {
            CastCenter.y = 0.075f;
        }

        CastCenter = transform.TransformPoint(CastCenter);

        RaycastHit[] HitStuff = Physics.BoxCastAll(CastCenter, new Vector3(0.045f, 0.01f, 0.045f), Vector3.up, Quaternion.identity, 0.045f); // cast size is the width of a peg, but very low so as to go under to outputs of not gates, buttons ect
        foreach(RaycastHit hit in HitStuff) // hopefully I didn't screw up the size of the boxcast!
        {
            if(hit.collider.tag != "Wire" && hit.collider.tag != "CircuitBoard" && hit.collider.tag != "World")
            {
                return true;
            }
        }

        return false;
    }


    public Color BoardColor;

    public void SetBoardColor(Color newcolor)
    {
        MegaBoardMeshManager.RemoveBoard(this); // update the mesh of the old color to not include this

        BoardColor = newcolor;
        Renderer.material.color = newcolor;

        MegaBoardMeshManager.AddBoard(this);
    }
}