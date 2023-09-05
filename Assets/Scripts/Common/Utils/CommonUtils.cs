using System;
using UnityEngine;

namespace Common.Utils
{
    public static class CommonUtils
    {
        /// <summary>
        /// Возвращает новый вектор с измененными x, y или z. Исходный вектор не меняется!
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 CreateFrom(this Vector3 vector, float? x = null, float? y = null,
            float? z = null)
        {
            var newX = x ?? vector.x;
            var newY = y ?? vector.y;
            var newZ = z ?? vector.z;
            return new Vector3(newX, newY, newZ);
        }

        public static void SetAllEntriesToValue<T>(this T[,] arr, T val)
        {
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                for (var y = 0; y < arr.GetLength(1); y++)
                {
                    arr[x, y] = val;
                }
            }
        }

        public static bool ContainsSafe(this int[,] arr, int val)
        {
            if (arr == null)
                return false;

            foreach (var e in arr)
            {
                if (e == val)
                    return true;
            }

            return false;
        }

        public static int CountSafe(this int[,] arr, int val)
        {
            if (arr == null)
                return 0;

            var count = 0;

            foreach (var e in arr)
            {
                if (e == val) count++;
            }

            return count;
        }

        public static float RoundToCeilRemValue(float orig, float f)
        {
            var rem = orig % f;
            var a = orig - rem;
            if (Math.Abs(rem) > 0.0001) a += f;

            return a;
        }

        public static float RoundToFloorRemValue(float orig, float f)
        {
            var rem = orig % f;
            var a = orig - rem;

            return a;
        }
    }
}