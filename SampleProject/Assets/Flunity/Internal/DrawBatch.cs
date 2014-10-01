using System;
using UnityEngine;
using Flunity.Utils;

namespace Flunity.Internal
{
	/// <summary>
	/// Forms vertex data for rendering scene
	/// </summary>
	internal class DrawBatch
	{
		private const int INITIAL_QUADS = 64;

		public int layer = 0;

		internal Rect? drawRect;

		private DrawOptions _drawOptions;
		private Texture2D _currentTexture;
		private Material _material;
		private int _renderQueue;

		private readonly FlashStage _stage;
		private readonly Mesh _mesh;

		private Vector3[] _vertices;
		private Color32[] _colors;
		private Vector2[] _uv;
		private Vector2[] _uv2;
		private int[] _indices;

		private int _vertexCount;
		private int _vertexNum;
		private int _indexCount;
		private int _indexNum;

		private Shader _shader;

		public Shader shader
		{
			get { return _shader; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_shader = value;
				_material = new Material(_shader);
				_material.renderQueue = _renderQueue;
			}
		}

		internal DrawBatch(FlashStage stage, Shader shader, int renderQueue = 0)
		{
			_stage = stage;
			_renderQueue = renderQueue;
			_drawOptions = new DrawOptions();

			_mesh = new Mesh();
			_mesh.MarkDynamic();

			if (shader != null)
				this.shader = shader;

			CreateBuffers(INITIAL_QUADS);
			Reset();
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

		public void DrawTriangles(Texture2D texture, VertexData[] vertices)
		{
			if (texture != _currentTexture)
			{
				Flush();
				ApplyTexture(texture);
			}

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

		public void DrawTriangles(Texture2D texture, VertexData[] vertices, short[] indices)
		{
			if (texture != _currentTexture)
			{
				Flush();
				ApplyTexture(texture);
			}

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

		public void DrawQuads(Texture2D texture, QuadCollection quadCollection)
		{
			var quads = quadCollection.quads;
			var quadsCount = quadCollection.quadsCount;

			for (int i = 0; i < quadsCount; i++)
			{
				DrawQuad(texture, ref quads[i]);
			}
		}
		
		public void DrawQuad(Texture2D texture, ref SpriteQuad quad)
		{
			if (texture != _currentTexture)
			{
				Flush();
				ApplyTexture(texture);
			}
					
			if (drawRect != null)
			{
				if (!drawRect.Value.Intersects(quad.GetBounds()))
					return;
			}

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

		public virtual void Flush()
		{
			if (_vertexNum == 0)
				return;

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

			Graphics.DrawMesh(_mesh, _stage.GetGlobalMatrix(), _material, layer);

			Reset();
		}

		internal void Reset()
		{
			_vertexNum = 0;
			_indexNum = 0;
		}

		private void ApplyTexture(Texture2D texture)
		{
			_currentTexture = texture;
			_material.mainTexture = _currentTexture;
			_material.SetPass(0);
		}
		
		public void ApplyOptions(DrawOptions drawOptions)
		{
			if (drawOptions.EqualsTo(_drawOptions))
				return;
			
			Flush();
			
			_drawOptions.CopyFrom(drawOptions);
			
//			if (_currentEffect != _drawOptions.effect)
//			{
//				_currentEffect = _drawOptions.effect;
//				_currentEffect.CurrentTechnique.Passes[0].Apply();
//			}
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
		
		private void AddVertices(VertexData[] vertices)
		{
			Array.Copy(vertices, 0, _indices, _vertexNum, vertices.Length);
			_vertexNum += vertices.Length;
		}
	}
}