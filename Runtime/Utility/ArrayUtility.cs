using System.Collections.Generic;
using UnityEngine;

namespace BreadThief.EasyDungeonGenerator
{
	/// <summary>
	/// Provides utility methods for array and list operations used in the dungeon generation system.
	/// Currently contains methods for randomizing list elements to support procedural content generation.
	/// </summary>
	public static class ArrayUtility
	{
		/// <summary>
		/// Randomly shuffles the elements of a list using the Fisher-Yates shuffle algorithm.
		/// This method modifies the original list in-place.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to shuffle. The list is modified directly.</param>
		public static void ShuffleList<T>(List<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				T temp = list[i];
				int randomIndex = Random.Range(i, list.Count);
				list[i] = list[randomIndex];
				list[randomIndex] = temp;
			}
		}
	}
}