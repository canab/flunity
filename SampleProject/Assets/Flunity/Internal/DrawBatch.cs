using System;
using UnityEngine;
using Flunity.Utils;
using System.Collections.Generic;

namespace Flunity.Internal
{
	/// <summary>
	/// Forms vertex data for rendering scene
	/// </summary>
	internal class DrawBatch
	{
		public int layer = 0;
		public int renderQueue = 0;
		public Shader shader;

		internal Rect? drawRect;

		private readonly List<DrawBatchMesh> _meshes = new List<DrawBatchMesh>();

		private Matrix4x4 _matrix;
		private DrawBatchMesh _currentMesh;
		private Texture2D _currentTexture;
		private DrawOptions _drawOptions = new DrawOptions(); // not implemented

		internal DrawBatch()
		{
		}

		public void DrawTriangles(Texture2D texture, VertexData[] vertices)
		{
			if (texture != _currentTexture)
				BeginNewMesh(texture);

			_currentMesh.DrawTriangles(vertices);
		}

		public void DrawTriangles(Texture2D texture, VertexData[] vertices, short[] indices)
		{
			if (texture != _currentTexture)
				BeginNewMesh(texture);

			_currentMesh.DrawTriangles(vertices, indices);
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
				BeginNewMesh(texture);
					
			if (drawRect != null)
			{
				if (!drawRect.Value.Intersects(quad.GetBounds()))
					return;
			}

			_currentMesh.DrawQuad(ref quad);
		}

		internal void Begin(Matrix4x4 matrix)
		{
			_matrix = matrix;
			_currentMesh = null;
			_currentTexture = null;
			renderQueue = 0;
		}

		internal void End()
		{
			if (_currentMesh != null)
				_currentMesh.Flush(_currentTexture, shader, _matrix, layer, renderQueue);
		}

		private void BeginNewMesh(Texture2D texture)
		{
			if (_currentMesh != null)
				_currentMesh.Flush(_currentTexture, shader, _matrix, layer, renderQueue);

			var meshIndex = _meshes.IndexOf(_currentMesh) + 1;
			if (meshIndex == _meshes.Count)
				_meshes.Add(new DrawBatchMesh());

			_currentMesh = _meshes[meshIndex];
			_currentTexture = texture;
			renderQueue += 1;
		}

		public void ApplyOptions(DrawOptions drawOptions)
		{
			if (drawOptions.EqualsTo(_drawOptions))
				return;
			
			_currentMesh.Flush(_currentTexture, shader, _matrix, layer, renderQueue);
			_drawOptions.CopyFrom(drawOptions);
		}

		public void Destroy()
		{
			foreach (var mesh in _meshes)
			{
				mesh.Destroy();
			}
		}
	}
}