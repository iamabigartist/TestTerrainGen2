using System;
using System.Collections.Generic;
using System.Linq;
namespace Utils.JobUtil
{
	public static class NativeContainerUtils
	{
		public static void Dispose(params object[] disposables)
		{
			var dispose_list = new List<IDisposable>();
			foreach (var obj in disposables)
			{
				switch (obj)
				{
					case Array array:
						{
							foreach (var disposable in array.Cast<IDisposable>())
							{
								disposable.Dispose();
							}
							break;
						}
					case IDisposable disposable:
						disposable.Dispose();
						break;
				}
			}
		}
	}
}