using UnityEngine;
using Flunity;
using System;

namespace Flunity.Internal
{
	class DrawBatchMesh
	{
		private const int INITIAL_QUADS = 64;

		private readonly Mesh _mesh = new Mesh();
		private Material _material;

		private Vector3[] _vertices;
		private Color32[] _colors;
		private Vector2[] _uv;
		private Vector2[] _uv2;
		private int[] _indices;

		private int _vertexCount;
		private int _vertexNum;
		private int _indexCount;
		private int _indexNum;

		public DrawBatchMesh()
		{
			CreateBuffers(INITIAL_QUADS);
			Reset();
		}

		public void Destroy ()
		{
			if (_material != null)
				UnityEngine.Object.DestroyImmediate(_material);

			if (_mesh != null)
			{
				_mesh.Clear();
				UnityEngine.Object.DestroyImmediate(_mesh);
			}
		}

		#region DRAW

		public void DrawQuad(ref SpriteQuad quad)
		{
			// performance inlining
			if (_vertexNum + 4 >= _vertexCount || _indexNum + 6 >= _indexCount)
				EnsureSize(4, 6);

			_vertices[_vertexNum] = quad.leftTop.position;
			_vertices[_vertexNum + 1] = quad.rightTop.position;
			_vertices[_vertexNum + 2] = quad.leftBottom.position;
			_vertices[_vertexNum + 3] = quad.rightBottom.position;

			quad.leftTop.color.GetColor(out _colors[_vertexNum + 0]);
			quad.rightTop.color.GetColor(out _colors[_vertexNum + 1]);
			quad.leftBottom.color.GetColor(out _colors[_vertexNum + 2]);
			quad.rightBottom.color.GetColor(out _colors[_vertexNum + 3]);

			quad.leftTop.color.GetTint(out _uv2[_vertexNum + 0]);
			quad.rightTop.color.GetTint(out _uv2[_vertexNum + 1]);
			quad.leftBottom.color.GetTint(out _uv2[_vertexNum + 2]);
			quad.rightBottom.color.GetTint(out _uv2[_vertexNum + 3]);

			_uv[_vertexNum] = quad.leftTop.texCoord;
			_uv[_vertexNum + 1] = quad.rightTop.texCoord;
			_uv[_vertexNum + 2] = quad.leftBottom.texCoord;
			_uv[_vertexNum + 3] = quad.rightBottom.texCoord;

			_indices[_indexNum] = _vertexNum;
			_indices[_indexNum + 1] = (_vertexNum + 1);
			_indices[_indexNum + 2] = (_vertexNum + 2);
			_indices[_indexNum + 3] = (_vertexNum + 1);
			_indices[_indexNum + 4] = (_vertexNum + 2);
			_indices[_indexNum + 5] = (_vertexNum + 3);

			_vertexNum += 4;
			_indexNum += 6;
		}

		public void DrawTriangles(VertexData[] vertices)
		{
			if (vertices.Length % 3 != 0)
				throw new Exception("Count of vertices must be divided by 3");

			EnsureSize(vertices.Length, vertices.Length);

			for (var i = 0; i < vertices.Length; i++)
			{
				_indices[_indexNum + i] = _vertexNum + i;
			}

			AddVertices(vertices);

			_indexNum += vertices.Length;
		}

		public void DrawTriangles(VertexData[] vertices, short[] indices)
		{
			if (vertices.Length % 3 != 0)
				throw new Exception("Count of vertices must be divided by 3");

			EnsureSize(vertices.Length, indices.Length);

			for (var i = 0; i < indices.Length; i++)
			{
				_indices[_indexNum + i] = _vertexNum + indices[i];
			}

			AddVertices(vertices);

			_indexNum += indices.Length;
		}

		#endregion

		#region BUFFERS

		public void Flush(Texture2D texture,
		                  Shader shader,
		                  Matrix4x4 matrix,
		                  int layer,
		                  int renderQueue)
		{
			if (_vertexNum == 0)
				return;

			if (_material == null || _material.shader != shader)
				_material = new Material(shader);
			
			if (_material.mainTexture != texture)
			{
				_material.mainTexture = texture;
				_material.SetPass(0);
			}

			_material.renderQueue = renderQueue;

			if (_indexNum < _indexCount - 1)
			{
				_vertices[_vertexNum + 1] = Vector3.zero;
				_uv[_vertexNum + 1] = Vector2.zero;
				_colors[_vertexNum + 1] = new Color32(0, 0, 0, 0);
				_uv2[_vertexNum + 1] = new Vector2();

				for (int i = _indexNum + 1; i < _indexCount; i++)
				{
					_indices[i] = _vertexNum + 1;
				}
			}

			_vertexCount = _vertexNum;
			_indexCount = _indexNum;

			_mesh.Clear();
			_mesh.vertices = _vertices;
			_mesh.colors32 = _colors;
			_mesh.uv = _uv;
			_mesh.uv2 = _uv2;
			_mesh.triangles = _indices;

			//			Debug.Log(_vertices.JoinStrings());
			//			Debug.Log(_colors.JoinStrings());
			//			Debug.Log(_uv.JoinStrings());

			Graphics.DrawMesh(_mesh, matrix, _material, layer);

			Reset();
		}

		private void AddVertices(VertexData[] vertices)
		{
			Array.Copy(vertices, 0, _indices, _vertexNum, vertices.Length);
			_vertexNum += vertices.Length;
		}

		private void CreateBuffers(int spriteCount)
		{
			_vertexCount = spriteCount * 4;
			_indexCount = spriteCount * 6;

			_vertices = new Vector3[_vertexCount];
			_colors = new Color32[_vertexCount];
			_uv = new Vector2[_vertexCount];
			_uv2 = new Vector2[_vertexCount];
			_indices = new int[_indexCount];
		}

		internal void Reset()
		{
			_vertexNum = 0;
			_indexNum = 0;
		}

		private void EnsureSize(int verticesCapacity, int indicesCapacity)
		{
			var verticesRequired = _vertexNum + verticesCapacity;
			if (_vertexCount < verticesRequired)
			{
				_vertexCount = verticesRequired + 128;

				Array.Resize(ref _vertices, _vertexCount);
				Array.Resize(ref _colors, _vertexCount);
				Array.Resize(ref _uv, _vertexCount);
				Array.Resize(ref _uv2, _vertexCount);
			}

			var inidicesRequired = _indexNum + indicesCapacity;

			if (_indexCount < inidicesRequired)
			{
				_indexCount = inidicesRequired + 192;
				Array.Resize(ref _indices, _indexCount);
			}
		}

		#endregion
	}
}

