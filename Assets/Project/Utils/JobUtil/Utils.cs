using System;
namespace Utils.JobUtil
{
	public static class Utils
	{
		public delegate void ResultGen<TResult>(out TResult result);
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