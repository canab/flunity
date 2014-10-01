using System;
using UnityEngine;
using Flunity.Utils;
using Flunity.Common;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Unity component in which flash scene is beind rendered.
	/// </summary>
	[ExecuteInEditMode]
	public class FlashStage: MonoBehaviour
	{
		/// <summary>
		/// Root DisplayContainer
		/// </summary>
		public DisplayContainer root { get; private set; }

		/// <summary>
		/// Global inout controller
		/// </summary>
		public InputController input { get; private set; }

		/// <summary>
		/// Scene width
		/// </summary>
		public int width = 600;

		/// <summary>
		/// Scene height
		/// </summary>
		public int height = 400;

		internal readonly EventSender drawEvent = new EventSender();
		internal readonly EventSender updateEvent = new EventSender();

		internal TouchController touchController;
		internal DrawBatch debugBatch;
		internal DrawBatch sceneBatch;
		internal bool isDrawPhase;

		#region shader

		[SerializeField]
		private Shader _shader;

		private bool _shaderDirty;

		/// <summary>
		/// Shader used for rendering
		/// </summary>
		public Shader shader
		{
			get { return _shader; }
			set
			{
				if (_shader != value)
				{
					_shader = value;
					_shaderDirty = true;
				}
			}
		}

		#endregion

		#region resourcePath

		[SerializeField]
		private String _resourcePath = "";

		private bool _resourceDirty;

		/// <summary>
		/// Resource wich will be automatically attached to the root.
		/// </summary>
		public String resourcePath
		{
			get { return _resourcePath; }
			set
			{
				if (_resourcePath != value)
				{
					_resourcePath = value;
					_resourceDirty = true;
				}
			}
		}

		#endregion

		#region isPlaying

		[SerializeField]
		private bool _isPlaying;

		/// <summary>
		/// If resource path is set, specifies whether attached instance should by played.
		/// </summary>
		public bool isPlaying
		{
			get	{ return _isPlaying; }
			set
			{
				if (_isPlaying != value)
				{
					_isPlaying = value;
					_resourceDirty = _resourcePath.IsNotEmpty();
				}
			}
		}

		#endregion

		#region life cycle

		void Awake()
		{
			root = new DisplayRoot(this);
			input = new InputController(this);
			touchController = new TouchController(this);
			sceneBatch = new DrawBatch(this, null, 0);
			debugBatch = new DrawBatch(this, null, 1);

			if (shader == null)
				shader = Shader.Find("Flunity/Default");
		}

		void OnEnable()
		{
			if (root == null)
			{
				Awake();
				Start();
			}

			if (Application.isEditor)
				FlashResources.UnloadAllBundles();
		}

		void Start()
		{
			_shaderDirty = true;
			_resourceDirty = _resourcePath.IsNotEmpty();
			FlashResources.Reloaded += onReloaded;
		}

		void onReloaded()
		{
			_resourceDirty = _resourcePath.IsNotEmpty();
		}

		void OnDestroy()
		{
			if (root != null)
				root.stage = null;
		}

		void FixedUpdate()
		{
			isDrawPhase = false;

			if (Application.isPlaying)
			{
				input.DoUpdate();
				touchController.DoUpdate();
				updateEvent.Dispatch();
			}
		}

		void Update()
		{
			isDrawPhase = false;

			if (FlashResources.isReloadingEnabled)
				FlashResources.reloadPendingResources();

			if (_shaderDirty)
			{
				sceneBatch.shader = shader;
				debugBatch.shader = shader;

				_shaderDirty = false;
			}

			if (_resourceDirty)
			{
				UpdateContent();
				_resourceDirty = false;
			}

			isDrawPhase = true;

			root.Draw();

			drawEvent.Dispatch();

			debugBatch.Flush();

			isDrawPhase = false;
		}

		#endregion

		#region transform

		public Matrix4x4 GetGlobalMatrix()
		{
			var stageMatrix = transform != null
				? transform.localToWorldMatrix
				: Matrix4x4.identity;

			var rootScale = new Vector3(1, -1, 1);
			var rootPosition = new Vector3(-0.5f * width, 0.5f * height, 0);
			var rootMatrix = Matrix4x4.TRS(rootPosition, Quaternion.identity, rootScale);
			return stageMatrix * rootMatrix;
		}

		public Vector2 TouchToComponentPoint(Vector3 point)
		{
			var worldPos = Camera.main.ScreenToWorldPoint(point);
			return WorldToComponentPoint(worldPos);
		}

		public Vector2 WorldToComponentPoint(Vector3 point)
		{
			return GetGlobalMatrix().inverse.MultiplyPoint3x4(point);
		}

		#endregion

		#region gizmos

		void OnDrawGizmos()
		{
			var matrix = GetGlobalMatrix();
			var w = width;
			var h = height;

			if (Application.isEditor)
			{
				var lt = matrix.MultiplyPoint(new Vector3(0, 0, 0));
				var rt = matrix.MultiplyPoint(new Vector3(w, 0, 0));
				var lb = matrix.MultiplyPoint(new Vector3(0, h, 0));
				var rb = matrix.MultiplyPoint(new Vector3(w, h, 0));

				Gizmos.DrawLine(lt, rt);
				Gizmos.DrawLine(lb, rb);
				Gizmos.DrawLine(lt, lb);
				Gizmos.DrawLine(rt, rb);

				if (!Application.isPlaying && (root == null || root.numChildren == 0))
				{
					Gizmos.DrawLine(lt, rb);
					Gizmos.DrawLine(rt, lb);
				}
			}
		}

		#endregion

		#region update

		void UpdateContent()
		{
			root.RemoveChildren();

			if (resourcePath.IsNullOrEmpty())
				return;

			var bundleName = resourcePath.Split("/")[0];
			var bundle = FlashResources.GetBundleInstance(bundleName);
			if (bundle == null)
			{
				Debug.Log("Bundle not found: " + bundleName);
				return;
			}

			if (!bundle.isLoaded)
				FlashResources.LoadBundle(bundle);

			var resource = FlashResources.GetResource<IDisplayResource>(resourcePath);
			if (resource == null)
			{
				Debug.Log("Resource not found: " + resourcePath);
				return;
			}

			var instance = resource.CreateInstance();
			root.AddChild(instance);

			if (isPlaying)
			{
				if (instance.totalFrames > 1)
				{
					instance.Play();
				}
				else
				{
					var displayContainer = instance as DisplayContainer;
					if (displayContainer != null)
						displayContainer.PlayAllChildren();
				}
			}
		}

		#endregion
	}
}

