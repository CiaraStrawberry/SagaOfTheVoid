using UnityEngine;

namespace ParticlePlayground;

public class SimplexNoise
{
	private static int[][] grad3;

	private static int[][] grad4;

	private static int[] p;

	private static int[] perm;

	private static int[][] simplex;

	static SimplexNoise()
	{
		grad3 = new int[12][]
		{
			new int[3] { 1, 1, 0 },
			new int[3] { -1, 1, 0 },
			new int[3] { 1, -1, 0 },
			new int[3] { -1, -1, 0 },
			new int[3] { 1, 0, 1 },
			new int[3] { -1, 0, 1 },
			new int[3] { 1, 0, -1 },
			new int[3] { -1, 0, -1 },
			new int[3] { 0, 1, 1 },
			new int[3] { 0, -1, 1 },
			new int[3] { 0, 1, -1 },
			new int[3] { 0, -1, -1 }
		};
		grad4 = new int[32][]
		{
			new int[4] { 0, 1, 1, 1 },
			new int[4] { 0, 1, 1, -1 },
			new int[4] { 0, 1, -1, 1 },
			new int[4] { 0, 1, -1, -1 },
			new int[4] { 0, -1, 1, 1 },
			new int[4] { 0, -1, 1, -1 },
			new int[4] { 0, -1, -1, 1 },
			new int[4] { 0, -1, -1, -1 },
			new int[4] { 1, 0, 1, 1 },
			new int[4] { 1, 0, 1, -1 },
			new int[4] { 1, 0, -1, 1 },
			new int[4] { 1, 0, -1, -1 },
			new int[4] { -1, 0, 1, 1 },
			new int[4] { -1, 0, 1, -1 },
			new int[4] { -1, 0, -1, 1 },
			new int[4] { -1, 0, -1, -1 },
			new int[4] { 1, 1, 0, 1 },
			new int[4] { 1, 1, 0, -1 },
			new int[4] { 1, -1, 0, 1 },
			new int[4] { 1, -1, 0, -1 },
			new int[4] { -1, 1, 0, 1 },
			new int[4] { -1, 1, 0, -1 },
			new int[4] { -1, -1, 0, 1 },
			new int[4] { -1, -1, 0, -1 },
			new int[4] { 1, 1, 1, 0 },
			new int[4] { 1, 1, -1, 0 },
			new int[4] { 1, -1, 1, 0 },
			new int[4] { 1, -1, -1, 0 },
			new int[4] { -1, 1, 1, 0 },
			new int[4] { -1, 1, -1, 0 },
			new int[4] { -1, -1, 1, 0 },
			new int[4] { -1, -1, -1, 0 }
		};
		p = new int[256]
		{
			151, 160, 137, 91, 90, 15, 131, 13, 201, 95,
			96, 53, 194, 233, 7, 225, 140, 36, 103, 30,
			69, 142, 8, 99, 37, 240, 21, 10, 23, 190,
			6, 148, 247, 120, 234, 75, 0, 26, 197, 62,
			94, 252, 219, 203, 117, 35, 11, 32, 57, 177,
			33, 88, 237, 149, 56, 87, 174, 20, 125, 136,
			171, 168, 68, 175, 74, 165, 71, 134, 139, 48,
			27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
			60, 211, 133, 230, 220, 105, 92, 41, 55, 46,
			245, 40, 244, 102, 143, 54, 65, 25, 63, 161,
			1, 216, 80, 73, 209, 76, 132, 187, 208, 89,
			18, 169, 200, 196, 135, 130, 116, 188, 159, 86,
			164, 100, 109, 198, 173, 186, 3, 64, 52, 217,
			226, 250, 124, 123, 5, 202, 38, 147, 118, 126,
			255, 82, 85, 212, 207, 206, 59, 227, 47, 16,
			58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
			119, 248, 152, 2, 44, 154, 163, 70, 221, 153,
			101, 155, 167, 43, 172, 9, 129, 22, 39, 253,
			19, 98, 108, 110, 79, 113, 224, 232, 178, 185,
			112, 104, 218, 246, 97, 228, 251, 34, 242, 193,
			238, 210, 144, 12, 191, 179, 162, 241, 81, 51,
			145, 235, 249, 14, 239, 107, 49, 192, 214, 31,
			181, 199, 106, 157, 184, 84, 204, 176, 115, 121,
			50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
			222, 114, 67, 29, 24, 72, 243, 141, 128, 195,
			78, 66, 215, 61, 156, 180
		};
		perm = new int[512];
		simplex = new int[64][]
		{
			new int[4] { 0, 1, 2, 3 },
			new int[4] { 0, 1, 3, 2 },
			new int[4],
			new int[4] { 0, 2, 3, 1 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 1, 2, 3, 0 },
			new int[4] { 0, 2, 1, 3 },
			new int[4],
			new int[4] { 0, 3, 1, 2 },
			new int[4] { 0, 3, 2, 1 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 1, 3, 2, 0 },
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 1, 2, 0, 3 },
			new int[4],
			new int[4] { 1, 3, 0, 2 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 2, 3, 0, 1 },
			new int[4] { 2, 3, 1, 0 },
			new int[4] { 1, 0, 2, 3 },
			new int[4] { 1, 0, 3, 2 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 2, 0, 3, 1 },
			new int[4],
			new int[4] { 2, 1, 3, 0 },
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 2, 0, 1, 3 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 3, 0, 1, 2 },
			new int[4] { 3, 0, 2, 1 },
			new int[4],
			new int[4] { 3, 1, 2, 0 },
			new int[4] { 2, 1, 0, 3 },
			new int[4],
			new int[4],
			new int[4],
			new int[4] { 3, 1, 0, 2 },
			new int[4],
			new int[4] { 3, 2, 0, 1 },
			new int[4] { 3, 2, 1, 0 }
		};
		for (int i = 0; i < 512; i++)
		{
			perm[i] = p[i & 0xFF];
		}
	}

	private static int fastfloor(double x)
	{
		return (!(x > 0.0)) ? ((int)x - 1) : ((int)x);
	}

	private static double dot(int[] g, double x, double y)
	{
		return (double)g[0] * x + (double)g[1] * y;
	}

	private static double dot(int[] g, double x, double y, double z)
	{
		return (double)g[0] * x + (double)g[1] * y + (double)g[2] * z;
	}

	private static double dot(int[] g, double x, double y, double z, double w)
	{
		return (double)g[0] * x + (double)g[1] * y + (double)g[2] * z + (double)g[3] * w;
	}

	public double noise(double xin, double yin, double zin)
	{
		double num = 1.0 / 3.0;
		double num2 = (xin + yin + zin) * num;
		int num3 = fastfloor(xin + num2);
		int num4 = fastfloor(yin + num2);
		int num5 = fastfloor(zin + num2);
		double num6 = 1.0 / 6.0;
		double num7 = (double)(num3 + num4 + num5) * num6;
		double num8 = (double)num3 - num7;
		double num9 = (double)num4 - num7;
		double num10 = (double)num5 - num7;
		double num11 = xin - num8;
		double num12 = yin - num9;
		double num13 = zin - num10;
		int num14;
		int num15;
		int num16;
		int num17;
		int num18;
		int num19;
		if (num11 >= num12)
		{
			if (num12 >= num13)
			{
				num14 = 1;
				num15 = 0;
				num16 = 0;
				num17 = 1;
				num18 = 1;
				num19 = 0;
			}
			else if (num11 >= num13)
			{
				num14 = 1;
				num15 = 0;
				num16 = 0;
				num17 = 1;
				num18 = 0;
				num19 = 1;
			}
			else
			{
				num14 = 0;
				num15 = 0;
				num16 = 1;
				num17 = 1;
				num18 = 0;
				num19 = 1;
			}
		}
		else if (num12 < num13)
		{
			num14 = 0;
			num15 = 0;
			num16 = 1;
			num17 = 0;
			num18 = 1;
			num19 = 1;
		}
		else if (num11 < num13)
		{
			num14 = 0;
			num15 = 1;
			num16 = 0;
			num17 = 0;
			num18 = 1;
			num19 = 1;
		}
		else
		{
			num14 = 0;
			num15 = 1;
			num16 = 0;
			num17 = 1;
			num18 = 1;
			num19 = 0;
		}
		double num20 = num11 - (double)num14 + num6;
		double num21 = num12 - (double)num15 + num6;
		double num22 = num13 - (double)num16 + num6;
		double num23 = num11 - (double)num17 + 2.0 * num6;
		double num24 = num12 - (double)num18 + 2.0 * num6;
		double num25 = num13 - (double)num19 + 2.0 * num6;
		double num26 = num11 - 1.0 + 3.0 * num6;
		double num27 = num12 - 1.0 + 3.0 * num6;
		double num28 = num13 - 1.0 + 3.0 * num6;
		int num29 = num3 & 0xFF;
		int num30 = num4 & 0xFF;
		int num31 = num5 & 0xFF;
		int num32 = perm[num29 + perm[num30 + perm[num31]]] % 12;
		int num33 = perm[num29 + num14 + perm[num30 + num15 + perm[num31 + num16]]] % 12;
		int num34 = perm[num29 + num17 + perm[num30 + num18 + perm[num31 + num19]]] % 12;
		int num35 = perm[num29 + 1 + perm[num30 + 1 + perm[num31 + 1]]] % 12;
		double num36 = 0.5 - num11 * num11 - num12 * num12 - num13 * num13;
		double num37;
		if (num36 < 0.0)
		{
			num37 = 0.0;
		}
		else
		{
			num36 *= num36;
			num37 = num36 * num36 * dot(grad3[num32], num11, num12, num13);
		}
		double num38 = 0.6 - num20 * num20 - num21 * num21 - num22 * num22;
		double num39;
		if (num38 < 0.0)
		{
			num39 = 0.0;
		}
		else
		{
			num38 *= num38;
			num39 = num38 * num38 * dot(grad3[num33], num20, num21, num22);
		}
		double num40 = 0.6 - num23 * num23 - num24 * num24 - num25 * num25;
		double num41;
		if (num40 < 0.0)
		{
			num41 = 0.0;
		}
		else
		{
			num40 *= num40;
			num41 = num40 * num40 * dot(grad3[num34], num23, num24, num25);
		}
		double num42 = 0.6 - num26 * num26 - num27 * num27 - num28 * num28;
		double num43;
		if (num42 < 0.0)
		{
			num43 = 0.0;
		}
		else
		{
			num42 *= num42;
			num43 = num42 * num42 * dot(grad3[num35], num26, num27, num28);
		}
		return 32.0 * (num37 + num39 + num41 + num43);
	}

	public double noise(double x, double y, double z, double w)
	{
		double num = ((double)Mathf.Sqrt(5f) - 1.0) / 4.0;
		double num2 = (5.0 - (double)Mathf.Sqrt(5f)) / 20.0;
		double num3 = (x + y + z + w) * num;
		int num4 = fastfloor(x + num3);
		int num5 = fastfloor(y + num3);
		int num6 = fastfloor(z + num3);
		int num7 = fastfloor(w + num3);
		double num8 = (double)(num4 + num5 + num6 + num7) * num2;
		double num9 = (double)num4 - num8;
		double num10 = (double)num5 - num8;
		double num11 = (double)num6 - num8;
		double num12 = (double)num7 - num8;
		double num13 = x - num9;
		double num14 = y - num10;
		double num15 = z - num11;
		double num16 = w - num12;
		int num17 = ((num13 > num14) ? 32 : 0);
		int num18 = ((num13 > num15) ? 16 : 0);
		int num19 = ((num14 > num15) ? 8 : 0);
		int num20 = ((num13 > num16) ? 4 : 0);
		int num21 = ((num14 > num16) ? 2 : 0);
		int num22 = ((num15 > num16) ? 1 : 0);
		int num23 = num17 + num18 + num19 + num20 + num21 + num22;
		int num24 = ((simplex[num23][0] >= 3) ? 1 : 0);
		int num25 = ((simplex[num23][1] >= 3) ? 1 : 0);
		int num26 = ((simplex[num23][2] >= 3) ? 1 : 0);
		int num27 = ((simplex[num23][3] >= 3) ? 1 : 0);
		int num28 = ((simplex[num23][0] >= 2) ? 1 : 0);
		int num29 = ((simplex[num23][1] >= 2) ? 1 : 0);
		int num30 = ((simplex[num23][2] >= 2) ? 1 : 0);
		int num31 = ((simplex[num23][3] >= 2) ? 1 : 0);
		int num32 = ((simplex[num23][0] >= 1) ? 1 : 0);
		int num33 = ((simplex[num23][1] >= 1) ? 1 : 0);
		int num34 = ((simplex[num23][2] >= 1) ? 1 : 0);
		int num35 = ((simplex[num23][3] >= 1) ? 1 : 0);
		double num36 = num13 - (double)num24 + num2;
		double num37 = num14 - (double)num25 + num2;
		double num38 = num15 - (double)num26 + num2;
		double num39 = num16 - (double)num27 + num2;
		double num40 = num13 - (double)num28 + 2.0 * num2;
		double num41 = num14 - (double)num29 + 2.0 * num2;
		double num42 = num15 - (double)num30 + 2.0 * num2;
		double num43 = num16 - (double)num31 + 2.0 * num2;
		double num44 = num13 - (double)num32 + 3.0 * num2;
		double num45 = num14 - (double)num33 + 3.0 * num2;
		double num46 = num15 - (double)num34 + 3.0 * num2;
		double num47 = num16 - (double)num35 + 3.0 * num2;
		double num48 = num13 - 1.0 + 4.0 * num2;
		double num49 = num14 - 1.0 + 4.0 * num2;
		double num50 = num15 - 1.0 + 4.0 * num2;
		double num51 = num16 - 1.0 + 4.0 * num2;
		int num52 = num4 & 0xFF;
		int num53 = num5 & 0xFF;
		int num54 = num6 & 0xFF;
		int num55 = num7 & 0xFF;
		int num56 = perm[num52 + perm[num53 + perm[num54 + perm[num55]]]] % 32;
		int num57 = perm[num52 + num24 + perm[num53 + num25 + perm[num54 + num26 + perm[num55 + num27]]]] % 32;
		int num58 = perm[num52 + num28 + perm[num53 + num29 + perm[num54 + num30 + perm[num55 + num31]]]] % 32;
		int num59 = perm[num52 + num32 + perm[num53 + num33 + perm[num54 + num34 + perm[num55 + num35]]]] % 32;
		int num60 = perm[num52 + 1 + perm[num53 + 1 + perm[num54 + 1 + perm[num55 + 1]]]] % 32;
		double num61 = 0.5 - num13 * num13 - num14 * num14 - num15 * num15 - num16 * num16;
		double num62;
		if (num61 < 0.0)
		{
			num62 = 0.0;
		}
		else
		{
			num61 *= num61;
			num62 = num61 * num61 * dot(grad4[num56], num13, num14, num15, num16);
		}
		double num63 = 0.6 - num36 * num36 - num37 * num37 - num38 * num38 - num39 * num39;
		double num64;
		if (num63 < 0.0)
		{
			num64 = 0.0;
		}
		else
		{
			num63 *= num63;
			num64 = num63 * num63 * dot(grad4[num57], num36, num37, num38, num39);
		}
		double num65 = 0.6 - num40 * num40 - num41 * num41 - num42 * num42 - num43 * num43;
		double num66;
		if (num65 < 0.0)
		{
			num66 = 0.0;
		}
		else
		{
			num65 *= num65;
			num66 = num65 * num65 * dot(grad4[num58], num40, num41, num42, num43);
		}
		double num67 = 0.6 - num44 * num44 - num45 * num45 - num46 * num46 - num47 * num47;
		double num68;
		if (num67 < 0.0)
		{
			num68 = 0.0;
		}
		else
		{
			num67 *= num67;
			num68 = num67 * num67 * dot(grad4[num59], num44, num45, num46, num47);
		}
		double num69 = 0.6 - num48 * num48 - num49 * num49 - num50 * num50 - num51 * num51;
		double num70;
		if (num69 < 0.0)
		{
			num70 = 0.0;
		}
		else
		{
			num69 *= num69;
			num70 = num69 * num69 * dot(grad4[num60], num48, num49, num50, num51);
		}
		return 27.0 * (num62 + num64 + num66 + num68 + num70);
	}
}
