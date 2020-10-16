using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace soundrestore
{
	/* Microsoft:
	 * 
	 * As part of speeding up logon, logoff, shutdown, we experiment a lot
	 * with what process is in control of the startup and shutdown sounds.
	 * In an interim build of Windows 8 we were able to speed things up
	 * considerably by moving the shutdown sound from Explorer.exe (which
	 * is running while you’re still logged on) to logonui.exe (which
	 * is the thing that shows the “Shutting down” circle.)
	 * 
	 * However moving the shutdown sound this late started running into
	 * other problems. The code we use to play the sound (PlaySound) needs
	 * to read from the registry (to see what the preferences for this
	 * sound were) and from the disk (to read the .wav file), and we ran
	 * into issues where the sound was unable to play (or got cutoff
	 * halfway) because we had shut down the registry or the disk already!
	 * 
	 * We could have spent time rewriting the API but we decided the safest
	 * and most performant thing to do was to eliminate the sound altogether.
	 * 
	 * 
	 * TLDR: We could have kept the sounds, but weren't competent enough to
	 * do it, so we decided to just kill the feature.
	 */

	public partial class DummyForm : Form
	{
		[DllImport("user32.dll")]
		private extern static bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string pwszReason);

		[DllImport("user32.dll")]
		private extern static bool ShutdownBlockReasonDestroy(IntPtr hWnd);

		private enum Sound
		{
			None = 0,
			Logon = 1,
			Logoff = 2,
			Unlock = 3
		}

		protected override bool ShowWithoutActivation
		{
			get { return true; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams baseParams = base.CreateParams;

				const int WS_EX_NOACTIVATE = 0x08000000;
				baseParams.ExStyle |= (int)(WS_EX_NOACTIVATE);

				return baseParams;
			}
		}

		public DummyForm()
		{
			InitializeComponent();

			SystemEvents.SessionEnded += new SessionEndedEventHandler(SystemEvents_SessionEnded);
			SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);
		}

		private void DummyForm_Shown(object sender, EventArgs e)
		{
			Hide();
			Play(Sound.Logon);
		}

		private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
		{
			ShutdownBlockReasonCreate(this.Handle, "Playing shutdown sound");
			Play(Sound.Logoff);
			ShutdownBlockReasonDestroy(this.Handle);
		}

		private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
		{
			if (e.Reason == SessionSwitchReason.SessionUnlock)
				Play(Sound.Unlock);
		}

		private void Play(Sound what)
		{
			const string SUBKEY_TPL = "AppEvents\\Schemes\\Apps\\.Default\\{0}\\.Current";
			string subkey = null, file = null; ;

			switch (what)
			{
				case Sound.Logon:
					subkey = "WindowsLogon";
					break;
				case Sound.Logoff:
					subkey = "WindowsLogoff";
					break;
				case Sound.Unlock:
					subkey = "WindowsUnlock";
					break;
			}

			if (string.IsNullOrEmpty(subkey))
				return;

			subkey = string.Format(CultureInfo.InvariantCulture, SUBKEY_TPL, subkey);

			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(subkey))
				if (key != null)
					file = (string)key.GetValue("", null);

			if (string.IsNullOrEmpty(file))
				return;

			if (!File.Exists(file))
				return;

			try
			{
				using (SoundPlayer sp = new SoundPlayer(file))
					sp.PlaySync();
			}
			catch (Exception e)
			{
				EventLog.WriteEntry("Application", string.Format(CultureInfo.InvariantCulture,
					"Cannot play system sound: {0}", e), EventLogEntryType.Error);
			}
		}
	}
}