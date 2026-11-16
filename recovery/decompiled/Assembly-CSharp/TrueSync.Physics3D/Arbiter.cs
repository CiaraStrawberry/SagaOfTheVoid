using System;

namespace TrueSync.Physics3D;

public class Arbiter : IComparable, ResourcePoolItem
{
	public static ResourcePool<Arbiter> Pool = new ResourcePool<Arbiter>();

	internal RigidBody body1;

	internal RigidBody body2;

	internal ContactList contactList;

	public RigidBody Body1 => body1;

	public RigidBody Body2 => body2;

	public ContactList ContactList => contactList;

	public Arbiter(RigidBody body1, RigidBody body2)
	{
		contactList = new ContactList();
		this.body1 = body1;
		this.body2 = body2;
	}

	public Arbiter()
	{
		contactList = new ContactList();
	}

	public int CompareTo(object obj)
	{
		if (obj is Arbiter)
		{
			long num = ((Arbiter)obj).GetHashCode();
			long num2 = GetHashCode();
			long num3 = num - num2;
			if (num3 < 0)
			{
				return 1;
			}
			if (num3 > 0)
			{
				return -1;
			}
		}
		return 0;
	}

	public override int GetHashCode()
	{
		return Body1.GetHashCode() + Body2.GetHashCode();
	}

	public void CleanUp()
	{
		contactList.Clear();
	}

	public void Invalidate()
	{
		contactList.Clear();
	}

	public Contact AddContact(TSVector point1, TSVector point2, TSVector normal, FP penetration, ContactSettings contactSettings)
	{
		TSVector.Subtract(ref point1, ref body1.position, out var result);
		lock (contactList)
		{
			int index;
			if (contactList.Count == 4)
			{
				index = SortCachedPoints(ref result, penetration);
				ReplaceContact(ref point1, ref point2, ref normal, penetration, index, contactSettings);
				return null;
			}
			index = GetCacheEntry(ref result, contactSettings.breakThreshold);
			if (index >= 0)
			{
				ReplaceContact(ref point1, ref point2, ref normal, penetration, index, contactSettings);
				return null;
			}
			Contact contact = Contact.Pool.GetNew();
			contact.Initialize(body1, body2, ref point1, ref point2, ref normal, penetration, newContact: true, contactSettings);
			contactList.Add(contact);
			return contact;
		}
	}

	private void ReplaceContact(ref TSVector point1, ref TSVector point2, ref TSVector n, FP p, int index, ContactSettings contactSettings)
	{
		Contact contact = contactList[index];
		contact.Initialize(body1, body2, ref point1, ref point2, ref n, p, newContact: false, contactSettings);
	}

	private int GetCacheEntry(ref TSVector realRelPos1, FP contactBreakThreshold)
	{
		FP fP = contactBreakThreshold * contactBreakThreshold;
		int count = contactList.Count;
		int result = -1;
		for (int i = 0; i < count; i++)
		{
			TSVector.Subtract(ref contactList[i].relativePos1, ref realRelPos1, out var result2);
			FP sqrMagnitude = result2.sqrMagnitude;
			if (sqrMagnitude < fP)
			{
				fP = sqrMagnitude;
				result = i;
			}
		}
		return result;
	}

	private int SortCachedPoints(ref TSVector realRelPos1, FP pen)
	{
		int num = -1;
		FP fP = pen;
		for (int i = 0; i < 4; i++)
		{
			if (contactList[i].penetration > fP)
			{
				num = i;
				fP = contactList[i].penetration;
			}
		}
		FP x = 0;
		FP y = 0;
		FP z = 0;
		FP w = 0;
		if (num != 0)
		{
			TSVector.Subtract(ref realRelPos1, ref contactList[1].relativePos1, out var result);
			TSVector.Subtract(ref contactList[3].relativePos1, ref contactList[2].relativePos1, out var result2);
			TSVector.Cross(ref result, ref result2, out var result3);
			x = result3.sqrMagnitude;
		}
		if (num != 1)
		{
			TSVector.Subtract(ref realRelPos1, ref contactList[0].relativePos1, out var result4);
			TSVector.Subtract(ref contactList[3].relativePos1, ref contactList[2].relativePos1, out var result5);
			TSVector.Cross(ref result4, ref result5, out var result6);
			y = result6.sqrMagnitude;
		}
		if (num != 2)
		{
			TSVector.Subtract(ref realRelPos1, ref contactList[0].relativePos1, out var result7);
			TSVector.Subtract(ref contactList[3].relativePos1, ref contactList[1].relativePos1, out var result8);
			TSVector.Cross(ref result7, ref result8, out var result9);
			z = result9.sqrMagnitude;
		}
		if (num != 3)
		{
			TSVector.Subtract(ref realRelPos1, ref contactList[0].relativePos1, out var result10);
			TSVector.Subtract(ref contactList[2].relativePos1, ref contactList[1].relativePos1, out var result11);
			TSVector.Cross(ref result10, ref result11, out var result12);
			w = result12.sqrMagnitude;
		}
		return MaxAxis(x, y, z, w);
	}

	internal static int MaxAxis(FP x, FP y, FP z, FP w)
	{
		int result = -1;
		FP fP = FP.MinValue;
		if (x > fP)
		{
			result = 0;
			fP = x;
		}
		if (y > fP)
		{
			result = 1;
			fP = y;
		}
		if (z > fP)
		{
			result = 2;
			fP = z;
		}
		if (w > fP)
		{
			result = 3;
			fP = w;
		}
		return result;
	}
}
