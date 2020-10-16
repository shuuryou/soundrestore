using System;
using System.Windows.Forms;
using System.Threading;

namespace soundrestore
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			Mutex mutex = new Mutex(true, "sndrst_d3f9ae65");

			if (!mutex.WaitOne(TimeSpan.Zero, true))
				return;

			Application.Run(new DummyForm());

			mutex.ReleaseMutex();
			mutex.Dispose();
		}
	}
}
