# System Sound Player
**A tiny program to restore logon, logoff, system start, and system exit sounds in modern versions of Windows.**

Microsoft removed the ability for the operating system to play a sound when locking or unlocking a session, as well as when logging on or logging off. This program will help you get these features back, because like everything the Windows Shell Team at Microsoft does, the removal was only half-assed.

What was actually removed was the ability for the operating system to play a configured sound file. This was supposedly because they had optimized startup and shutdown so much that they couldn't get the sounds to play reliably anymore. However, the ability to set a sound file via the Control Panel still exists and was just hidden.

## How to Use
1. Put `soundrestore.exe` in `C:\Windows\`
1. Use the `SetTaskman.reg` to start the program as early as possible after signing in to Windows
1. Use the `RestoreSoundEvents.reg` file to unhide the program events for "Windows Logon", "Windows Logoff", "Exit Windows" and "Start Windows" (you need to do this once for every user if you have multiple user accounts)
1. Configure the sounds that you want to play using the Control Panel (hint: Press <kbd>Windows</kbd> + <kbd>R</kbd> on your keyboard, type `control mmsys.cpl`, and press Enter)
1. Reboot

> [!CAUTION]
> The [Taskman registry key](https://www.geoffchappell.com/notes/windows/shell/explorer/taskman.htm) is fairly obscure. Some anti-virus programs may flag setting it as suspicious. You can verify that `SetTaskman.reg` is not doing anything malicious by examining its contents with a text editor.
>
> Likewise, `soundrestore.exe` is a tiny program that lacks a digital signature, doesn't have an icon, and doesn't include any file version information. This makes it look scary to "AI-powered" or "cloud-enhanced" security software. If you don't trust me, grab a copy of Visual Studio and compile the code yourself.

## When Program Events Are Played
| Event | When Played |
|-------|-------------|
| Windows Logon | Session is unlocked (e.g., after entering password) |
| Windows Logoff | Session is locked (<kbd>Windows</kbd> + <kbd>L</kbd>) or "Switch user" is selected |
| Exit Windows | System shuts down, restarts, or user signs out |
| Start Windows | At session startup, triggered by the Task Scheduler task |

It is absolutely pathetic that you need to use a third-party program like this one to do such a simple thing, but alas, such is the state of modern Windows.