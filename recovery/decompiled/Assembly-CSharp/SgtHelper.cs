using System;
using System.Collections.Generic;
using UnityEngine;

public static class SgtHelper
{
	public const string ShaderNamePrefix = "Hidden/Sgt";

	public const string ComponentMenuPrefix = "Space Graphics Toolkit/SGT ";

	public const string GameObjectMenuPrefix = "GameObject/Space Graphics Toolkit/";

	public static readonly float InscribedBox = 1f / Mathf.Sqrt(2f);

	public static readonly int QuadsPerMesh = 16250;

	private static List<Material> tempMaterials = new List<Material>();

	private static Stack<UnityEngine.Random.State> seedStates = new Stack<UnityEngine.Random.State>();

	private static List<Material> materials = new List<Material>();

	public static void ClearCapacity<T>(List<T> list, int minCapacity)
	{
		if (list != null)
		{
			list.Clear();
			if (list.Capacity < minCapacity)
			{
				list.Capacity = minCapacity;
			}
		}
	}

	public static bool Enabled(Behaviour b)
	{
		return b != null && b.enabled && b.gameObject.activeInHierarchy;
	}

	public static T GetOrAddComponent<T>(GameObject gameObject, bool recordUndo = true) where T : Component
	{
		if (gameObject != null)
		{
			T val = gameObject.GetComponent<T>();
			if (val == null)
			{
				val = AddComponent<T>(gameObject, recordUndo);
			}
			return val;
		}
		return (T)null;
	}

	public static T AddComponent<T>(GameObject gameObject, bool recordUndo = true) where T : Component
	{
		if (gameObject != null)
		{
			return gameObject.AddComponent<T>();
		}
		return (T)null;
	}

	public static T Destroy<T>(T o) where T : UnityEngine.Object
	{
		if (o != null)
		{
			UnityEngine.Object.Destroy(o);
		}
		return (T)null;
	}

	public static bool Zero(float v)
	{
		return Mathf.Approximately(v, 0f);
	}

	public static float Reciprocal(float v)
	{
		return Zero(v) ? 0f : (1f / v);
	}

	public static float Acos(float v)
	{
		if (v >= -1f && v <= 1f)
		{
			return Mathf.Acos(v);
		}
		return 0f;
	}

	public static Vector3 Reciprocal3(Vector3 xyz)
	{
		xyz.x = Reciprocal(xyz.x);
		xyz.y = Reciprocal(xyz.y);
		xyz.z = Reciprocal(xyz.z);
		return xyz;
	}

	public static float Divide(float a, float b)
	{
		return (b == 0f) ? 0f : (a / b);
	}

	public static Vector4 NewVector4(Vector3 xyz, float w)
	{
		return new Vector4(xyz.x, xyz.y, xyz.z, w);
	}

	public static float Pow(float a, float p)
	{
		if (p >= 0f)
		{
			return Mathf.Pow(a, p);
		}
		return 1f - Mathf.Pow(1f - a, 0f - p);
	}

	public static float CubicInterpolate(float a, float b, float c, float d, float t)
	{
		float num = t * t;
		d = d - c - (a - b);
		return d * (num * t) + (a - b - d) * num + (c - a) * t + b;
	}

	public static float HermiteInterpolate(float a, float b, float c, float d, float t)
	{
		float num = t * t;
		float num2 = num * 3f;
		float num3 = t * num;
		float num4 = num3 * 2f;
		float num5 = (c - a) * 0.5f;
		float num6 = (d - b) * 0.5f;
		float num7 = num4 - num2 + 1f;
		float num8 = num3 - num * 2f + t;
		float num9 = num3 - num;
		float num10 = 0f - num4 + num2;
		return num7 * b + num8 * num5 + num9 * num6 + num10 * c;
	}

	public static Color HermiteInterpolate(Color a, Color b, Color c, Color d, float t)
	{
		float num = t * t;
		float num2 = num * 3f;
		float num3 = t * num;
		float num4 = num3 * 2f;
		Color color = (c - a) * 0.5f;
		Color color2 = (d - b) * 0.5f;
		float num5 = num4 - num2 + 1f;
		float num6 = num3 - num * 2f + t;
		float num7 = num3 - num;
		float num8 = 0f - num4 + num2;
		return num5 * b + num6 * color + num7 * color2 + num8 * c;
	}

	public static Vector3 HermiteInterpolate3(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
	{
		float num = t * t;
		float num2 = num * 3f;
		float num3 = t * num;
		float num4 = num3 * 2f;
		Vector3 vector = (c - a) * 0.5f;
		Vector3 vector2 = (d - b) * 0.5f;
		float num5 = num4 - num2 + 1f;
		float num6 = num3 - num * 2f + t;
		float num7 = num3 - num;
		float num8 = 0f - num4 + num2;
		return num5 * b + num6 * vector + num7 * vector2 + num8 * c;
	}

	public static float DampenFactor(float dampening, float elapsed)
	{
		return 1f - Mathf.Exp((0f - dampening) * elapsed);
	}

	public static float Dampen(float current, float target, float dampening, float elapsed)
	{
		float t = DampenFactor(dampening, elapsed);
		return Mathf.LerpUnclamped(current, target, t);
	}

	public static float Dampen(float current, float target, float dampening, float elapsed, float minStep)
	{
		current = Mathf.MoveTowards(current, target, minStep * elapsed);
		return Dampen(current, target, dampening, elapsed);
	}

	public static Quaternion Dampen(Quaternion current, Quaternion target, float dampening, float elapsed)
	{
		float t = DampenFactor(dampening, elapsed);
		return Quaternion.Slerp(current, target, t);
	}

	public static Quaternion Dampen(Quaternion current, Quaternion target, float dampening, float elapsed, float minStep)
	{
		float b = Quaternion.Angle(current, target);
		float t = DampenFactor(dampening, elapsed) + Divide(minStep * elapsed, b);
		return Quaternion.Slerp(current, target, t);
	}

	public static Vector3 Dampen3(Vector3 current, Vector3 target, float dampening, float elapsed)
	{
		float t = DampenFactor(dampening, elapsed);
		return Vector3.LerpUnclamped(current, target, t);
	}

	public static Vector3 Dampen3(Vector3 current, Vector3 target, float dampening, float elapsed, float minStep)
	{
		current = Vector3.MoveTowards(current, target, minStep * elapsed);
		return Dampen3(current, target, dampening, elapsed);
	}

	public static int Mod(int a, int b)
	{
		int num = a % b;
		if (num < 0)
		{
			return num + b;
		}
		return num;
	}

	public static Bounds NewBoundsFromMinMax(Vector3 min, Vector3 max)
	{
		Bounds result = default;
		result.SetMinMax(min, max);
		return result;
	}

	public static Bounds NewBoundsCenter(Bounds b, Vector3 c)
	{
		float x = Mathf.Max(Mathf.Abs(c.x - b.min.x), Mathf.Abs(c.x - b.max.x));
		float y = Mathf.Max(Mathf.Abs(c.y - b.min.z), Mathf.Abs(c.y - b.max.y));
		float z = Mathf.Max(Mathf.Abs(c.z - b.min.z), Mathf.Abs(c.z - b.max.z));
		return new Bounds(c, new Vector3(x, y, z) * 2f);
	}

	public static void ResizeArrayTo<T>(ref List<T> array, int size, Func<int, T> newT, Action<T> removeT)
	{
		if (array != null)
		{
			while (array.Count < size)
			{
				array.Add((newT == null) ? default(T) : newT(array.Count));
			}
			while (array.Count > size)
			{
				removeT?.Invoke(array[array.Count - 1]);
				array.RemoveAt(array.Count - 1);
			}
		}
	}

	public static bool RequireArraySize<T>(ref T[] array, int size)
	{
		if (array == null || array.Length != size)
		{
			array = new T[size];
			return true;
		}
		return false;
	}

	public static void BeginRandomSeed(int newSeed)
	{
		seedStates.Push(UnityEngine.Random.state);
		UnityEngine.Random.InitState(newSeed);
	}

	public static void EndRandomSeed()
	{
		UnityEngine.Random.state = seedStates.Pop();
	}

	public static Material CreateTempMaterial(string materialName, string shaderName)
	{
		Shader shader = Shader.Find(shaderName);
		if (shader == null)
		{
			Debug.LogError("Failed to find shader: " + shaderName);
			return null;
		}
		Material material = new Material(shader);
		material.name = materialName;
		return material;
	}

	public static Mesh CreateTempMesh(string meshName)
	{
		Mesh mesh = SgtObjectPool<Mesh>.Pop() ?? new Mesh();
		mesh.name = meshName;
		return mesh;
	}

	public static Texture2D CreateTempTexture2D(string name, int width, int height, TextureFormat format = TextureFormat.ARGB32, bool mips = false, bool linear = false)
	{
		Texture2D texture2D = new Texture2D(width, height, format, mips, linear);
		texture2D.name = name;
		return texture2D;
	}

	public static Texture2D CreateTexture2D(string name, int width, int height, TextureFormat format = TextureFormat.ARGB32, bool mips = false, bool linear = false, bool recordUndo = true)
	{
		Texture2D texture2D = new Texture2D(width, height, format, mips, linear);
		texture2D.name = name;
		return texture2D;
	}

	public static Material CreateMaterial(string shaderName, bool recordUndo = true)
	{
		return new Material(Shader.Find(shaderName));
	}

	public static GameObject CloneGameObject(GameObject source, Transform parent, bool keepName = false)
	{
		return CloneGameObject(source, parent, source.transform.localPosition, source.transform.localRotation, keepName);
	}

	public static GameObject CloneGameObject(GameObject source, Transform parent, Vector3 localPosition, Quaternion localRotation, bool keepName = false)
	{
		if (source != null)
		{
			GameObject gameObject = null;
			if (parent != null)
			{
				gameObject = UnityEngine.Object.Instantiate(source);
				gameObject.transform.parent = parent;
				gameObject.transform.localPosition = localPosition;
				gameObject.transform.localRotation = localRotation;
				gameObject.transform.localScale = source.transform.localScale;
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate(source, localPosition, localRotation);
			}
			if (keepName)
			{
				gameObject.name = source.name;
			}
			return gameObject;
		}
		return source;
	}

	public static GameObject CreateGameObject(string name, int layer, Transform parent = null, bool recordUndo = true)
	{
		return CreateGameObject(name, layer, parent, Vector3.zero, Quaternion.identity, Vector3.one, recordUndo);
	}

	public static GameObject CreateGameObject(string name, int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, bool recordUndo = true)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.layer = layer;
		gameObject.transform.SetParent(parent, worldPositionStays: false);
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.localRotation = localRotation;
		gameObject.transform.localScale = localScale;
		return gameObject;
	}

	public static void SetSharedMaterial(Renderer r, Material m)
	{
		if (r != null && r.sharedMaterial != m)
		{
			r.sharedMaterial = m;
		}
	}

	public static void SetSharedMesh(MeshFilter mf, Mesh m)
	{
		if (mf != null && mf.sharedMesh != m)
		{
			mf.sharedMesh = m;
		}
	}

	public static void SetEnabled(Behaviour b, bool enabled)
	{
		if (b != null && b.enabled != enabled)
		{
			b.enabled = enabled;
		}
	}

	public static void SetEnabled(Collider c, bool enabled)
	{
		if (c != null && c.enabled != enabled)
		{
			c.enabled = enabled;
		}
	}

	public static void SetEnabled(Renderer r, bool enabled)
	{
		if (r != null && r.enabled != enabled)
		{
			r.enabled = enabled;
		}
	}

	public static void SetPosition(Transform t, float x, float y, float z)
	{
		SetPosition(t, new Vector3(x, y, z));
	}

	public static void SetPosition(Transform t, Vector3 v)
	{
		if (t != null)
		{
			t.position = v;
		}
	}

	public static void SetLocalPosition(Transform t, float x, float y, float z)
	{
		SetLocalPosition(t, new Vector3(x, y, z));
	}

	public static void SetLocalPosition(Transform t, Vector3 v)
	{
		if (t != null)
		{
			t.localPosition = v;
		}
	}

	public static void SetLocalScale(Transform t, float v)
	{
		SetLocalScale(t, new Vector3(v, v, v));
	}

	public static void SetLocalScale(Transform t, float x, float y, float z)
	{
		SetLocalScale(t, new Vector3(x, y, z));
	}

	public static void SetLocalScale(Transform t, Vector3 v)
	{
		if (t != null && !(t.localScale == v))
		{
			t.localScale = v;
		}
	}

	public static void SetLocalRotation(Transform t, Quaternion q)
	{
		if (t != null)
		{
			t.localRotation = q;
		}
	}

	public static void SetRotation(Transform t, Quaternion q)
	{
		if (t != null)
		{
			t.rotation = q;
		}
	}

	public static float DistanceToHorizon(float radius, float distanceToCenter)
	{
		if (distanceToCenter > radius)
		{
			return Mathf.Sqrt(distanceToCenter * distanceToCenter - radius * radius);
		}
		return 0f;
	}

	public static Vector2 CartesianToPolar(Vector3 xyz)
	{
		float x = Mathf.Atan2(xyz.x, xyz.z);
		float y = Mathf.Asin(xyz.y / xyz.magnitude);
		return new Vector2(x, y);
	}

	public static Vector2 CartesianToPolarUV(Vector3 xyz)
	{
		Vector2 result = CartesianToPolar(xyz);
		result.x = Mathf.Repeat(0.5f - result.x / ((float)Math.PI * 2f), 1f);
		result.y = 0.5f + result.y / (float)Math.PI;
		return result;
	}

	public static Vector4 CalculateSpriteUV(Sprite s)
	{
		Vector4 result = default;
		if (s != null)
		{
			Rect textureRect = s.textureRect;
			Texture2D texture = s.texture;
			result.x = Divide(textureRect.xMin, texture.width);
			result.y = Divide(textureRect.yMin, texture.height);
			result.z = Divide(textureRect.xMax, texture.width);
			result.w = Divide(textureRect.yMax, texture.height);
		}
		return result;
	}

	public static void CalculateAtmosphereThicknessAtHorizon(float groundRadius, float skyRadius, float distance, out float groundThickness, out float skyThickness)
	{
		float a = DistanceToHorizon(groundRadius, distance);
		float num = Mathf.Sin(Mathf.Acos(Divide(groundRadius, skyRadius))) * skyRadius;
		float num2 = Mathf.Min(a, num);
		groundThickness = Mathf.Max(num2, skyRadius - groundRadius);
		skyThickness = num + num2;
	}

	public static void EnableKeyword(string keyword, Material material)
	{
		if (material != null && !material.IsKeywordEnabled(keyword))
		{
			material.EnableKeyword(keyword);
		}
	}

	public static void DisableKeyword(string keyword, Material material)
	{
		if (material != null && material.IsKeywordEnabled(keyword))
		{
			material.DisableKeyword(keyword);
		}
	}

	public static void SetKeywords(Material m, List<string> keywords)
	{
		if (m != null && !ArraysEqual(m.shaderKeywords, keywords))
		{
			m.shaderKeywords = keywords.ToArray();
		}
	}

	public static void SetKeywords(Material m, List<string> keywords, ref string[] lastKeywords)
	{
		if (!(m != null) || ArraysEqual(lastKeywords, keywords))
		{
			return;
		}
		if (lastKeywords != null && lastKeywords.Length == keywords.Count)
		{
			for (int num = keywords.Count - 1; num >= 0; num--)
			{
				lastKeywords[num] = keywords[num];
			}
		}
		else
		{
			lastKeywords = keywords.ToArray();
		}
		m.shaderKeywords = lastKeywords;
	}

	public static bool ArraysEqual<T>(T[] a, List<T> b)
	{
		if (a != null && b == null)
		{
			return false;
		}
		if (a == null && b != null)
		{
			return false;
		}
		if (a != null && b != null)
		{
			if (a.Length != b.Count)
			{
				return false;
			}
			EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a.Length; i++)
			{
				if (!equalityComparer.Equals(a[i], b[i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	public static void AddMaterial(Renderer r, Material m)
	{
		if (!(r != null) || !(m != null))
		{
			return;
		}
		Material[] sharedMaterials = r.sharedMaterials;
		Material[] array = sharedMaterials;
		foreach (Material material in array)
		{
			if (material == m)
			{
				return;
			}
		}
		Material[] array2 = sharedMaterials;
		foreach (Material material2 in array2)
		{
			if (material2 != null)
			{
				materials.Add(material2);
			}
		}
		materials.Add(m);
		r.sharedMaterials = materials.ToArray();
		materials.Clear();
	}

	public static void ReplaceMaterial(Renderer r, Material m)
	{
		if (!(r != null) || !(m != null))
		{
			return;
		}
		Material[] sharedMaterials = r.sharedMaterials;
		Material[] array = sharedMaterials;
		foreach (Material material in array)
		{
			if (material == m)
			{
				return;
			}
		}
		Material[] array2 = sharedMaterials;
		foreach (Material material2 in array2)
		{
			if (material2 != null && material2.shader != m.shader)
			{
				materials.Add(material2);
			}
		}
		materials.Add(m);
		r.sharedMaterials = materials.ToArray();
		materials.Clear();
	}

	public static void RemoveMaterial(Renderer r, Material m)
	{
		if (!(r != null))
		{
			return;
		}
		Material[] sharedMaterials = r.sharedMaterials;
		Material[] array = sharedMaterials;
		foreach (Material material in array)
		{
			if (material != null && material != m)
			{
				materials.Add(material);
			}
		}
		r.sharedMaterials = materials.ToArray();
		materials.Clear();
	}

	public static float UniformScale(Vector3 scale)
	{
		scale.x = Math.Abs(scale.x);
		scale.y = Math.Abs(scale.y);
		scale.z = Math.Abs(scale.z);
		return (scale.x + scale.y + scale.z) / 3f;
	}

	public static Matrix4x4 Rotation(Quaternion q)
	{
		return Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
	}

	public static Matrix4x4 Translation(Vector3 xyz)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m03 = xyz.x;
		identity.m13 = xyz.y;
		identity.m23 = xyz.z;
		return identity;
	}

	public static Matrix4x4 Scaling(float xyz)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = xyz;
		identity.m11 = xyz;
		identity.m22 = xyz;
		return identity;
	}

	public static Matrix4x4 Scaling(Vector3 xyz)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = xyz.x;
		identity.m11 = xyz.y;
		identity.m22 = xyz.z;
		return identity;
	}

	public static Matrix4x4 Scaling(float x, float y, float z)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = x;
		identity.m11 = y;
		identity.m22 = z;
		return identity;
	}

	public static Matrix4x4 ShearingX(Vector2 yz)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m01 = yz.x;
		identity.m02 = yz.y;
		return identity;
	}

	public static Matrix4x4 ShearingY(Vector2 xz)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m10 = xz.x;
		identity.m12 = xz.y;
		return identity;
	}

	public static Matrix4x4 ShearingZ(Vector2 xy)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m20 = xy.x;
		identity.m21 = xy.y;
		return identity;
	}

	public static Color Brighten(Color color, float brightness)
	{
		color.r *= brightness;
		color.g *= brightness;
		color.b *= brightness;
		return color;
	}

	public static Color Premultiply(Color color)
	{
		color.r *= color.a;
		color.g *= color.a;
		color.b *= color.a;
		return color;
	}

	public static void CalculateLight(Light light, Vector3 center, Transform directionTransform, Transform positionTransform, ref Vector3 position, ref Vector3 direction, ref Color color)
	{
		if (light != null)
		{
			direction = -light.transform.forward;
			position = light.transform.position;
			color = Brighten(light.color, light.intensity);
			switch (light.type)
			{
			case LightType.Point:
				direction = Vector3.Normalize(position - center);
				break;
			case LightType.Directional:
				position = center + direction * 10000f;
				break;
			}
			if (directionTransform != null)
			{
				direction = directionTransform.InverseTransformDirection(direction);
			}
			if (positionTransform != null)
			{
				position = positionTransform.InverseTransformPoint(position);
			}
		}
	}

	public static void SetTempMaterial(Material material)
	{
		tempMaterials.Clear();
		tempMaterials.Add(material);
	}

	public static void SetTempMaterial(Material material1, Material material2)
	{
		tempMaterials.Clear();
		tempMaterials.Add(material1);
		tempMaterials.Add(material2);
	}

	public static void SetTempMaterial(List<Material> materials)
	{
		tempMaterials.Clear();
		if (materials != null)
		{
			tempMaterials.AddRange(materials);
		}
	}

	public static void EnableKeyword(string keyword)
	{
		for (int num = tempMaterials.Count - 1; num >= 0; num--)
		{
			Material material = tempMaterials[num];
			if (material != null && !material.IsKeywordEnabled(keyword))
			{
				material.EnableKeyword(keyword);
			}
		}
	}

	public static void DisableKeyword(string keyword)
	{
		for (int num = tempMaterials.Count - 1; num >= 0; num--)
		{
			Material material = tempMaterials[num];
			if (material != null && material.IsKeywordEnabled(keyword))
			{
				material.DisableKeyword(keyword);
			}
		}
	}

	public static void SetMatrix(string key, Matrix4x4 value)
	{
		for (int num = tempMaterials.Count - 1; num >= 0; num--)
		{
			Material material = tempMaterials[num];
			if (material != null)
			{
				material.SetMatrix(key, value);
			}
		}
	}

	public static void WriteLights(bool lit, List<Light> lights, int maxLights, Vector3 center, Transform directionTransform, Transform positionTransform, Color tint, float scatterStrength)
	{
		int num = 0;
		if (lit && lights != null)
		{
			for (int i = 0; i < lights.Count; i++)
			{
				if (num >= maxLights)
				{
					break;
				}
				Light light = lights[i];
				if (!Enabled(light) || !(light.intensity > 0f))
				{
					continue;
				}
				string text = "_Light" + ++num;
				Vector3 direction = default;
				Vector3 position = default;
				Color color = default;
				CalculateLight(light, center, directionTransform, positionTransform, ref position, ref direction, ref color);
				Vector4 value = NewVector4(position, 1f);
				for (int num2 = tempMaterials.Count - 1; num2 >= 0; num2--)
				{
					Material material = tempMaterials[num2];
					if (material != null)
					{
						material.SetVector(text + "Direction", direction);
						material.SetVector(text + "Position", value);
						material.SetColor(text + "Color", color * tint);
						material.SetColor(text + "Scatter", color * tint * scatterStrength);
					}
				}
			}
		}
		for (int j = 0; j <= maxLights; j++)
		{
			if (lit && j == num)
			{
				EnableKeyword("LIGHT_" + j);
			}
			else
			{
				DisableKeyword("LIGHT_" + j);
			}
		}
	}

	public static void WriteShadows(List<SgtShadow> shadows, int maxShadows)
	{
		int num = 0;
		if (shadows != null)
		{
			int count = shadows.Count;
			for (int i = 0; i < count; i++)
			{
				if (num >= maxShadows)
				{
					break;
				}
				SgtShadow sgtShadow = shadows[i];
				Matrix4x4 matrix = default;
				float ratio = 0f;
				if (!Enabled(sgtShadow) || !sgtShadow.CalculateShadow(ref matrix, ref ratio))
				{
					continue;
				}
				string text = "_Shadow" + ++num;
				for (int num2 = tempMaterials.Count - 1; num2 >= 0; num2--)
				{
					Material material = tempMaterials[num2];
					if (material != null)
					{
						material.SetTexture(text + "Texture", sgtShadow.GetTexture());
						material.SetMatrix(text + "Matrix", matrix);
						material.SetFloat(text + "Ratio", ratio);
					}
				}
			}
		}
		for (int j = 1; j <= maxShadows; j++)
		{
			if (j == num)
			{
				EnableKeyword("SHADOW_" + j);
			}
			else
			{
				DisableKeyword("SHADOW_" + j);
			}
		}
	}

	public static void WriteShadowsNonSerialized(List<SgtShadow> shadows, int maxShadows)
	{
		if (shadows == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < shadows.Count; i++)
		{
			if (num >= maxShadows)
			{
				break;
			}
			SgtShadow sgtShadow = shadows[i];
			Matrix4x4 matrix = default;
			float ratio = 0f;
			if (Enabled(sgtShadow) && sgtShadow.CalculateShadow(ref matrix, ref ratio))
			{
				string text = "_Shadow" + ++num;
				SetMatrix(text + "Matrix", matrix);
			}
		}
	}

	public static void WriteMie(float sharpness, float strength)
	{
		sharpness = Mathf.Pow(10f, sharpness);
		strength *= (Mathf.Log10(sharpness) + 1f) * 0.75f;
		float num = 0f - (1f - 1f / sharpness);
		Vector4 value = new Vector4(num * 2f, 1f - num * num, 1f + num * num, strength);
		for (int num2 = tempMaterials.Count - 1; num2 >= 0; num2--)
		{
			Material material = tempMaterials[num2];
			if (material != null)
			{
				material.SetVector("_Mie", value);
			}
		}
	}

	public static void WriteRayleigh(float strength)
	{
		for (int num = tempMaterials.Count - 1; num >= 0; num--)
		{
			Material material = tempMaterials[num];
			if (material != null)
			{
				material.SetFloat("_Rayleigh", strength);
			}
		}
	}
}
