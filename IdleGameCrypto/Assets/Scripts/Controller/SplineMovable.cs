using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;


	public class SplineMovable : MonoBehaviour
	{
		[Serializable]
		public struct Curve
		{
			public int indexA;

			public int indexB;

			public Vector3 pivot;
		}

		[SerializeField]
		private float _movementSpeed;

		[Header("Path Parameters")]
		[SerializeField]
		private float _turnStartDistance;

		[SerializeField]
		[Range(0f, 90f)]
		private float _minCurveAngle = 20f;


		public List<Vector3> Path;

		public List<Curve> Curves;

		private Action _onComplete;

		private Sequence _currentSequence;

		public int CurrentIndex
		{
			get;
			private set;
		}

		public void Initialize()
		{
		}

		public void Teleport(Vector3 point)
		{
			FinishCurrentSequence(executeCallbacks: false);
			base.transform.position = point;
		}

		public void TeleportToPathNode(int nodeIndex)
		{
			if (Path != null && nodeIndex >= 0 && nodeIndex < Path.Count)
			{
				Vector3 vector;
				Vector3 b;
				if (nodeIndex == 0)
				{
					vector = Path[0];
					b = Path[1];
				}
				else
				{
					vector = Path[nodeIndex];
					b = Path[nodeIndex - 1];
				}
				Vector3 normalized = (vector - b).normalized;
				Teleport(vector);
				base.transform.rotation = Quaternion.LookRotation(normalized);
			}
		}

		public void TeleportToLastPathNode()
		{
			TeleportToPathNode(Path.Count - 1);
		}

		public void Move(Vector3[] points, Action onComplete = null)
		{
			if (points != null)
			{
				_onComplete = onComplete;
				RecalculatePath(points);
				StartMoving();
			}
		}

		public void RecalculatePath(Vector3[] points)
		{
			Path = new List<Vector3>();
			Curves = new List<Curve>();
			bool flag = false;
			Curve curve = default(Curve);
			for (int i = 0; i < points.Length; i++)
			{
				int num = i;
				int num2 = i + 1;
				int num3 = i + 2;
				bool num4 = num2 < points.Length - 1;
				bool flag2 = num3 < points.Length;
				float num5 = 0f;
				if (flag2)
				{
					Vector3 normalized = (points[num2] - points[num]).normalized;
					Vector3 normalized2 = (points[num3] - points[num2]).normalized;
					num5 = Mathf.Abs(Vector3.Angle(normalized, normalized2));
				}
				bool flag3 = num5 > _minCurveAngle;
				if (num4 & flag2 & flag3)
				{
					Vector3 vector = points[num];
					Vector3 vector2 = points[num2];
					Vector3 b = points[num3];
					if (!flag)
					{
						RegisterPathNode(vector);
					}
					Vector3 vector3 = CalculateOutTurnPoint(vector, vector2, _turnStartDistance);
					Vector3 vector4 = CalculateInTurnPoint(vector2, b, _turnStartDistance);
					Vector3 pivot = vector2;
					RegisterPathNode(vector3);
					RegisterPathNode(vector4);
					int indexA = Path.IndexOf(vector3);
					int indexB = Path.IndexOf(vector4);
					curve.indexA = indexA;
					curve.indexB = indexB;
					curve.pivot = pivot;
					RegisterCurve(curve);
					flag = true;
				}
				else
				{
					if (!flag)
					{
						RegisterPathNode(points[i]);
					}
					flag = false;
				}
			}
		}

		public void Clear()
		{
			Path.Clear();
			Curves.Clear();
		}

		public void Stop()
		{
			base.transform.DOKill();
		}

		public void FinishInstantly(bool executeCallback = true)
		{
			FinishCurrentSequence(executeCallbacks: false);
			Vector3 vector = Path[Path.Count - 1];
			base.transform.position = vector;
			if (Path.Count - 2 >= 0)
			{
				Vector3 b = Path[Path.Count - 2];
				Vector3 normalized = (vector - b).normalized;
				base.transform.rotation = Quaternion.LookRotation(normalized);
			}
			if (executeCallback && _onComplete != null)
			{
				_onComplete();
			}
		}

		public void RegisterPathNode(Vector3 node)
		{
			Path.Add(node);
		}

		public void RegisterCurve(Curve curve)
		{
			Curves.Add(curve);
		}

		public Vector3 FindCurvePivot(int indexA, int indexB)
		{
			foreach (Curve curf in Curves)
			{
				if (curf.indexA == indexA && curf.indexB == indexB)
				{
					return curf.pivot;
				}
				if (curf.indexA == indexB && curf.indexB == indexA)
				{
					return curf.pivot;
				}
			}
			return Vector3.zero;
		}

		private void StartMoving()
		{
			base.transform.DOKill();
			base.transform.position = Path[0];
			CurrentIndex = 1;
			MoveToNode(CurrentIndex);
		}

		private void OnNodeReached()
		{
			CurrentIndex++;
			if (CurrentIndex < Path.Count)
			{
				MoveToNode(CurrentIndex);
			}
			else if (_onComplete != null)
			{
				_onComplete();
			}
		}

		private void MoveToNode(int nodeIndex)
		{
			Sequence sequence;
			if (IsCurve(nodeIndex - 1, nodeIndex))
			{
				sequence = TakeCurve(nodeIndex - 1, nodeIndex);
			}
			else
			{
				Ease ease = SelectEasingForSegment(nodeIndex);
				sequence = TakeStraightLine(nodeIndex, ease);
			}
			sequence.OnComplete(OnNodeReached);
			_currentSequence = sequence;
		}

		private Ease SelectEasingForSegment(int nodeIndex)
		{
			bool num = nodeIndex == 1;
			bool flag = nodeIndex == Path.Count - 1;
			if (num)
			{
				return Ease.InSine;
			}
			if (flag)
			{
				return Ease.OutSine;
			}
			return Ease.Linear;
		}

		private Vector3 CalculateInTurnPoint(Vector3 a, Vector3 b, float turnDistance)
		{
			Vector3 normalized = (b - a).normalized;
			return a + normalized * turnDistance;
		}

		private Vector3 CalculateOutTurnPoint(Vector3 a, Vector3 b, float turnDistance)
		{
			Vector3 normalized = (b - a).normalized;
			return b - normalized * turnDistance;
		}

		public bool IsCurve(int indexA, int indexB)
		{
			foreach (Curve curf in Curves)
			{
				if (curf.indexA == indexA && curf.indexB == indexB)
				{
					return true;
				}
				if (curf.indexA == indexB && curf.indexB == indexA)
				{
					return true;
				}
			}
			return false;
		}

		private Sequence TakeCurve(int indexA, int indexB)
		{
			Vector3 nodeA = Path[indexA];
			Vector3 nodeB = Path[indexB];
			bool num = IsCurve(indexA, indexB);
			Vector3 pivot = FindCurvePivot(indexA, indexB);
			float duration = CalculateCurveLength(nodeA, nodeB, pivot) / _movementSpeed;
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("Gameplay");
			sequence.Append(base.transform.DOMove(nodeA, 0f));
			if (num)
			{
				float t = 0f;
				sequence.Append(DOTween.To(() => t, delegate(float x)
				{
					t = x;
					base.transform.position = CalculateCurvePosition(nodeA, nodeB, pivot, x);
					Vector3 forward = CalculateCurveLookDirection(nodeA, nodeB, pivot, x);
					base.transform.rotation = Quaternion.LookRotation(forward);
				}, 1f, duration).SetEase(Ease.Linear));
			}
			else
			{
				sequence.Append(base.transform.DOMove(nodeB, duration)).SetEase(Ease.Linear);
			}
			return sequence;
		}

		private Sequence TakeStraightLine(int index, Ease ease = Ease.Linear)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("Gameplay");
			Vector3 vector = Path[index];
			float duration = Vector3.Distance(base.transform.position, vector) / _movementSpeed;
			Vector3 normalized = (vector - base.transform.position).normalized;
			sequence.Append(base.transform.DOMove(vector, duration).SetEase(ease));
			sequence.Insert(0f, base.transform.DORotate(Quaternion.LookRotation(normalized).eulerAngles, 0.5f).SetEase(ease));
			return sequence;
		}

		private void FinishCurrentSequence(bool executeCallbacks = true)
		{
			if (_currentSequence != null)
			{
				_currentSequence.Kill(executeCallbacks);
			}
		}

		public static Vector3 CalculateCurvePosition(Vector3 start, Vector3 end, Vector3 pivot, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			return num * num * start + 2f * num * t * pivot + t * t * end;
		}

		public static Vector3 CalculateCurveLookDirection(Vector3 start, Vector3 end, Vector3 pivot, float t)
		{
			return 2f * (1f - t) * (pivot - start) + 2f * t * (end - pivot);
		}

		public static float CalculateCurveLength(Vector3 start, Vector3 end, Vector3 pivot)
		{
			float num = Vector3.Distance(start, pivot);
			return (float)Math.PI * 2f * num * 0.25f;
		}
	}

