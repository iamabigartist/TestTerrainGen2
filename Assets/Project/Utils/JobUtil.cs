using Unity.Mathematics;
namespace Utils
{
	public static class JobUtil
	{
		public static int volume(this int3 i3) { return i3.x * i3.y * i3.z; }
		public static int area(this int2 i2) { return i2.x * i2.y; }
	}
}