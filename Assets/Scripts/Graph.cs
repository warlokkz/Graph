using UnityEngine;

public class Graph : MonoBehaviour
{
	#region Public Fields

	/// <summary>
	/// A primitive cube object used as a point in the Graph.
	/// </summary>
	public Transform PointPrefab;
	/// <summary>
	/// The current Graph Function.
	/// </summary>
	public GraphFunctionName Function;
	/// <summary>
	/// The amount of Points in the Graph.
	/// </summary>
	[Range(10, 100)] public int Resolution = 10;

	#endregion

	#region Private Fields

	/// <summary>
	/// List of available GraphFunctions.
	/// </summary>
	private static readonly GraphFunction[] GraphFunctions =
	{
		SineFunction,
		Sine2DFunction,
		CrossSine2DFunction,
		MultiSineFunction,
		MultiSine2DFunction,
		RippleFunction,
		WeirdFunction,
		Cylinder,
		WobblyCylinder,
		TwistingStar,
		Sphere
	};
	/// <summary>
	/// List of instantiated points in the Graph.
	/// </summary>
	private Transform[] _points;
	/// <summary>
	/// The relative scale of a point in the graph.
	/// </summary>
	private Vector3 _pointScale;
	/// <summary>
	/// The spacing between a point and the next in the graph.
	/// </summary>
	private float _pointStep;

	#endregion

	#region Unity Event Functions

	private void Awake()
	{
		_points = new Transform[Resolution * Resolution];
		_pointStep = 2f / Resolution;
		_pointScale = Vector3.one * _pointStep;

		for (var i = 0; i < _points.Length; i++)
		{
				Transform point = Instantiate(PointPrefab);
				point.localScale = _pointScale;
				point.SetParent(transform, worldPositionStays: false);
				_points[i] = point;
		}
	}

	private void Update()
	{
		var f = GraphFunctions[(int)Function];
		var t = Time.time;
		for (int i = 0, z = 0; z < Resolution; z++)
		{
			var v = (z + 0.5f) * _pointStep - 1f;
			for (var x = 0; x < Resolution; x++, i++)
			{
				var u = (x + 0.5f) * _pointStep - 1f;
				_points[i].localPosition = f(u, v, t);
			}
		}
	}

	#endregion

	#region 2D Graph Functions
	/// <summary>
	/// Determine a sine wave value based on the X
	/// position and time value.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value from a sine wave.
	/// </returns>
	static Vector3 SineFunction(float x, float z, float t)
	{
		return new Vector3
		{
			x = x,
			y = Mathf.Sin(Mathf.PI * (x + t)),
			z = z
		};
	}

	/// <summary>
	/// Determine a sine wave value based on a 2D matrix of values
	/// and time.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value from a sine wave.
	/// </returns>
	static Vector3 Sine2DFunction(float x, float z, float t)
	{
		return new Vector3
		{
			x = x,
			y = Mathf.Sin(Mathf.PI * (x + z + t)),
			z = z
		};
}

	/// <summary>
	/// Determine a cross-sine wave vallue based on a 2D matrix of values
	/// and time.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value based on the sine wave.
	/// </returns>
	static Vector3 CrossSine2DFunction(float x, float z, float t)
	{
		var y = Mathf.Sin(Mathf.PI * (x + t));
		y += Mathf.Sin(Mathf.PI * (z + t));
		y *= 0.5f;

		return new Vector3
		{
			x = x,
			y = y,
			z = z
		};
	}

	/// <summary>
	/// Determine a multi-sine wave value based on the X
	/// position and time value.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value from a multi-sine wave.
	/// </returns>
	static Vector3 MultiSineFunction(float x, float z, float t)
	{
		var y = Mathf.Sin(Mathf.PI * (x + t));
		y += Mathf.Sin(2f * Mathf.PI * (x + 2f * t)) / 2f;
		y *= 2f / 3f;

		return new Vector3
		{
			x = x,
			y = y,
			z = z
		};
	}

	/// <summary>
	/// A main sine wave, with two smaller cross-waves for each dimensions.
	/// f(x,z,t) = M + Sx + Sz
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value from a sine wave.
	/// </returns>
	static Vector3 MultiSine2DFunction(float x, float z, float t)
	{
		// M = 4 * sin(π(x + z + t/2))) : 4
		var y = 4f * Mathf.Sin(Mathf.PI * (x + z + t * 0.5f));
		// Sx = sin(π(x + t)) : 1
		y += Mathf.Sin(Mathf.PI * (x + t));
		// Sz = sin(2π(z + 2t)) : 0.5
		y += Mathf.Sin(2f * Mathf.PI * (z + 2f * t)) * 0.5f;
		// Normalize the value to stay within the -1-1 range.
		y *= 1f / 5.5f;

		return new Vector3
		{
			x = x,
			y = y,
			z = z
		};
	}

	/// <summary>
	/// A main sine wave, with smaller waves inside. I don't even know.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value from a sine wave.
	/// </returns>
	static Vector3 WeirdFunction(float x, float z, float t)
	{
		var y = Mathf.Sin(Mathf.PI * (x + z + t));
		y += Mathf.Sin(2f * Mathf.PI * (x + t)) / 2f;
		y += Mathf.Sin(2f * Mathf.PI * (y + t)) / 2f;
		y *= 0.25f;

		return new Vector3
		{
			x = x,
			y = y,
			z = z
		};
	}

	/// <summary>
	/// A ripple made of a sine wave.
	/// f(x,z,t) = sin(πD), where D is distance from the origin
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value from a sine wave.
	/// </returns>
	static Vector3 RippleFunction(float x, float z, float t)
	{
		// D = distance from origin
		float d = Mathf.Sqrt(x * x + z * z);
		// sin(πD)
		float y = Mathf.Sin(Mathf.PI * (4f * d - t));
		// * 1/10d
		y /= 1 + 10f * d;

		return new Vector3
		{
			x = x,
			y = y,
			z = z
		};
	}

	#endregion

	#region 3D Graph Functions

	/// <summary>
	/// Create a 3D Cylinder from points on a graph.
	/// </summary>
	/// <param name="u"></param>
	/// <param name="v"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 value for a point's position in the cylinder.
	/// </returns>
	private static Vector3 Cylinder(float u, float v, float t)
	{
		var r = 1f;
		return new Vector3
		{
			x = r * Mathf.Sin(Mathf.PI * u),
			y = v,
			z = r * Mathf.Cos(Mathf.PI * u)
		};
	}

	/// <summary>
	/// Create a 3D 'Wobbly' Cylinder from points on a graph.
	/// </summary>
	/// <param name="u"></param>
	/// <param name="v"></param>
	/// <param name="t"></param>
	/// <returns>
	/// a Vector3 position
	/// </returns>
	private static Vector3 WobblyCylinder(float u, float v, float t)
	{
		var r = 1f + Mathf.Sin(6f * Mathf.PI * u) * 0.2f;
		
		return new Vector3
		{
			x = r * Mathf.Sin(Mathf.PI * u),
			y = v,
			z = r * Mathf.Cos(Mathf.PI * u)
		};
	}

	/// <summary>
	/// Create a 3D Twisting Star cylinder from points on a graph
	/// </summary>
	/// <param name="u"></param>
	/// <param name="v"></param>
	/// <param name="t"></param>
	/// <returns>
	/// a Vector3 position
	/// </returns>
	private static Vector3 TwistingStar(float u, float v, float t)
	{
		var r = 0.8f + Mathf.Sin(Mathf.PI * (6f * u + 2f * v + t)) * 0.2f;
		
		return new Vector3
		{
			x = r * Mathf.Sin(Mathf.PI * u),
			y = v,
			z = r * Mathf.Cos(Mathf.PI * u)
		};
	}

	/// <summary>
	/// Create a 3D sphere from points on a graph
	/// </summary>
	/// <param name="u"></param>
	/// <param name="v"></param>
	/// <param name="t"></param>
	/// <returns>
	/// A Vector3 position for a point on a graph
	/// </returns>
	private static Vector3 Sphere(float u, float v, float t)
	{
		var r = Mathf.Cos(Mathf.PI * 0.5f * v);
		
		return new Vector3
		{
			x = r * Mathf.Sin(Mathf.PI * u),
			y = Mathf.Sin(Mathf.PI * 0.5f * v),
			z = r * Mathf.Cos(Mathf.PI * u)
		};
	}

	#endregion
}
