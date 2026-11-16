using System;
using UnityEngine;

[Serializable]
public class SgtSimplex
{
	[SerializeField]
	private byte[] perm = new byte[512];

	public SgtSimplex()
	{
	}

	public SgtSimplex(int seed)
	{
		SetSeed(seed);
	}

	public void SetSeed(int seed)
	{
		System.Random random = new System.Random(seed);
		for (int i = 0; i < 256; i++)
		{
			perm[i] = (perm[i + 256] = (byte)(random.Next() % 256));
		}
	}

	public float Generate(float x, float y, float z)
	{
		float num = (x + y + z) * (1f / 3f);
		float x2 = x + num;
		float x3 = y + num;
		float x4 = z + num;
		int num2 = FastFloor(x2);
		int num3 = FastFloor(x3);
		int num4 = FastFloor(x4);
		float num5 = (float)(num2 + num3 + num4) * (1f / 6f);
		float num6 = (float)num2 - num5;
		float num7 = (float)num3 - num5;
		float num8 = (float)num4 - num5;
		float num9 = x - num6;
		float num10 = y - num7;
		float num11 = z - num8;
		int num12;
		int num13;
		int num14;
		int num15;
		int num16;
		int num17;
		if (num9 >= num10)
		{
			if (num10 >= num11)
			{
				num12 = 1;
				num13 = 0;
				num14 = 0;
				num15 = 1;
				num16 = 1;
				num17 = 0;
			}
			else if (num9 >= num11)
			{
				num12 = 1;
				num13 = 0;
				num14 = 0;
				num15 = 1;
				num16 = 0;
				num17 = 1;
			}
			else
			{
				num12 = 0;
				num13 = 0;
				num14 = 1;
				num15 = 1;
				num16 = 0;
				num17 = 1;
			}
		}
		else if (num10 < num11)
		{
			num12 = 0;
			num13 = 0;
			num14 = 1;
			num15 = 0;
			num16 = 1;
			num17 = 1;
		}
		else if (num9 < num11)
		{
			num12 = 0;
			num13 = 1;
			num14 = 0;
			num15 = 0;
			num16 = 1;
			num17 = 1;
		}
		else
		{
			num12 = 0;
			num13 = 1;
			num14 = 0;
			num15 = 1;
			num16 = 1;
			num17 = 0;
		}
		float num18 = num9 - (float)num12 + 1f / 6f;
		float num19 = num10 - (float)num13 + 1f / 6f;
		float num20 = num11 - (float)num14 + 1f / 6f;
		float num21 = num9 - (float)num15 + 1f / 3f;
		float num22 = num10 - (float)num16 + 1f / 3f;
		float num23 = num11 - (float)num17 + 1f / 3f;
		float num24 = num9 - 1f + 0.5f;
		float num25 = num10 - 1f + 0.5f;
		float num26 = num11 - 1f + 0.5f;
		int num27 = Mod(num2, 256);
		int num28 = Mod(num3, 256);
		int num29 = Mod(num4, 256);
		float num30 = 0.6f - num9 * num9 - num10 * num10 - num11 * num11;
		float num31;
		if (num30 < 0f)
		{
			num31 = 0f;
		}
		else
		{
			num30 *= num30;
			num31 = num30 * num30 * grad(perm[num27 + perm[num28 + perm[num29]]], num9, num10, num11);
		}
		float num32 = 0.6f - num18 * num18 - num19 * num19 - num20 * num20;
		float num33;
		if (num32 < 0f)
		{
			num33 = 0f;
		}
		else
		{
			num32 *= num32;
			num33 = num32 * num32 * grad(perm[num27 + num12 + perm[num28 + num13 + perm[num29 + num14]]], num18, num19, num20);
		}
		float num34 = 0.6f - num21 * num21 - num22 * num22 - num23 * num23;
		float num35;
		if (num34 < 0f)
		{
			num35 = 0f;
		}
		else
		{
			num34 *= num34;
			num35 = num34 * num34 * grad(perm[num27 + num15 + perm[num28 + num16 + perm[num29 + num17]]], num21, num22, num23);
		}
		float num36 = 0.6f - num24 * num24 - num25 * num25 - num26 * num26;
		float num37;
		if (num36 < 0f)
		{
			num37 = 0f;
		}
		else
		{
			num36 *= num36;
			num37 = num36 * num36 * grad(perm[num27 + 1 + perm[num28 + 1 + perm[num29 + 1]]], num24, num25, num26);
		}
		return 32f * (num31 + num33 + num35 + num37);
	}

	private static int FastFloor(float x)
	{
		return (!(x > 0f)) ? ((int)x - 1) : ((int)x);
	}

	private static int Mod(int x, int m)
	{
		int num = x % m;
		return (num >= 0) ? num : (num + m);
	}

	private static float grad(int hash, float x)
	{
		int num = hash & 0xF;
		float num2 = 1f + (float)(num & 7);
		if ((num & 8) != 0)
		{
			num2 = 0f - num2;
		}
		return num2 * x;
	}

	private static float grad(int hash, float x, float y)
	{
		int num = hash & 7;
		float num2 = ((num >= 4) ? y : x);
		float num3 = ((num >= 4) ? x : y);
		return (((num & 1) == 0) ? num2 : (0f - num2)) + (((num & 2) == 0) ? (2f * num3) : (-2f * num3));
	}

	private static float grad(int hash, float x, float y, float z)
	{
		int num = hash & 0xF;
		float num2 = ((num >= 8) ? y : x);
		float num3;
		if (num < 4)
		{
			num3 = y;
		}
		else
		{
			num3 = ((num != 12 && num != 14) ? z : x);
		}
		return (((num & 1) == 0) ? num2 : (0f - num2)) + (((num & 2) == 0) ? num3 : (0f - num3));
	}

	private static float grad(int hash, float x, float y, float z, float t)
	{
		int num = hash & 0x1F;
		float num2 = ((num >= 24) ? y : x);
		float num3 = ((num >= 16) ? z : y);
		float num4 = ((num >= 8) ? t : z);
		return (((num & 1) == 0) ? num2 : (0f - num2)) + (((num & 2) == 0) ? num3 : (0f - num3)) + (((num & 4) == 0) ? num4 : (0f - num4));
	}
}
