using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * A crap weighted list implementation. Loops through the whole list each Get() invocation.
 * 
 */
public class WeightedList<T>
{
	private class ItemHolder
	{
		public T Item;
		public double Weight;

		public ItemHolder(T item, double weight)
		{
			this.Item = item;
			this.Weight = weight;
		}
	}

	List<ItemHolder> items = new List<ItemHolder>();
	double totalWeight;

	public void Add(T t, double weight)
	{
		items.Add(new ItemHolder(t, weight));
		totalWeight += weight;
	}

	public T Get(double rngIndex)
	{
		double w = rngIndex * totalWeight;

		double cumulW = 0;
		foreach (ItemHolder i in items)
		{
			cumulW += i.Weight;
			if (w <= cumulW) return i.Item;
		}
		throw new System.IndexOutOfRangeException();
	}
}
