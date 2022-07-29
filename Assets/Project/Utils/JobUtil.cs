using System;
namespace Utils
{
	public static class JobUtil
	{
		public static void Dispose<TDisposable>(this TDisposable[] array)
			where TDisposable : IDisposable
		{
			foreach (var disposable in array)
			{
				disposable.Dispose();
			}
		}
	}
}