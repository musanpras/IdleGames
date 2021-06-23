using System;
using System.Collections.Generic;
using UnityEngine;

	public static class MathUtil
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				T value = list[i];
				int index = UnityEngine.Random.Range(i, list.Count);
				list[i] = list[index];
				list[index] = value;
			}
		}

		public static bool IsPointInOABB(Vector3 point, BoxCollider box)
		{
			point = box.transform.InverseTransformPoint(point) - box.center;
			float num = box.size.x * 0.5f;
			float num2 = box.size.y * 0.5f;
			float num3 = box.size.z * 0.5f;
			if (point.x < num && point.x > 0f - num && point.y < num2 && point.y > 0f - num2 && point.z < num3 && point.z > 0f - num3)
			{
				return true;
			}
			return false;
		}

		public static float IntegrateCurve(AnimationCurve curve, float startTime, float endTime, int steps)
		{
			return Integrate(curve.Evaluate, startTime, endTime, steps);
		}

		public static float Integrate(Func<float, float> f, float x_low, float x_high, int N_steps)
		{
			float num = (x_high - x_low) / (float)N_steps;
			float num2 = (f(x_low) + f(x_high)) / 2f;
			for (int i = 1; i < N_steps; i++)
			{
				num2 += f(x_low + (float)i * num);
			}
			return num * num2;
		}

		public static Vector3 GetPointInPlane(Ray ray, Plane plane)
		{
			float d = ray.origin.y - (plane.normal * plane.distance).y;
			Vector3 a = ray.direction / ray.direction.y;
			return ray.origin - a * d;
		}

		public static Vector3 GetPointInPlane(Camera camera, Vector3 point, Plane plane)
		{
			return GetPointInPlane(camera.ScreenPointToRay(point), plane);
		}
	}

