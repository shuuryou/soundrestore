# System Sound Player
**A small C# program that will restore the logon, logoff, and unlock sound events in Windows 10.**

In Windows 10, Microsoft removed the ability for the operating system to play a sound when logging on, logging off, or unlocking the computer. This program will help you get these sound events back, because like everything the Windows team do, the removal of these events was only half-assed.

What was removed in Windows 10 was the ability for the operating system to actually play back a configured sound file. This was supposedly because they had optimized startup and shutdown so much that they couldn't get the sounds to play reliably anymore. However, the ability to set a sound file via the control panel still exists and has just been hidden.

## How to Use
1. Put `soundrestore.exe` in a central location, like your Windows directory
1. Put a shortcut to `soundrestore.exe` in your Startup folder
1. Launch the shortcut manually once (or reboot)
1. Use the `RestoreSoundEvents.reg` to unhide the program events for "Windows Logon", "Windows Logoff" and "Windows Unlock" (you need to do this once for every user if you have multiple user accounts)
1. Configure the sounds that you want to play using the Control Panel

The All Users Startup folder is located at `C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup`, and the current user's Startup folder is located at `C:\Users\[User Name]\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup`.

You can use the `HideSoundEvents.reg` file if you want to hide the abovementioned program events again.

# Why?
## Windows 10 Has No Soul
![](https://i.ibb.co/Cbw576Y/W10-Laptop-Start-Mini-Start-16x9-en-US-042315.png)

After you install Windows 10, you're greeted by a dark and cold environment along with a start menu that is basically a giant advertisement board. It feels very much like a dark and lonely subway station. It isn't welcoming at all and makes you want to leave as quickly as possible.

My solution for this was to run some custom-made scripts that remove essentially all safely removable modern apps. I also use Windows 10 Enterprise so that I can apply a few hundred Group Policies that make the operating system largely stop spying on me. They also make it a whole lot less annoying to use.

But it still feels dark and cold.

## Windows Used to be Fun
![](https://i.ibb.co/D19Gr94/Annotation-2020-10-16-150202.jpg)

Remember Windows 98? Yes, it wasn't exactly what people would consider a technological marvel. It would crash daily due to bad drivers and badly written software, and when it crashed, it showed no mercy and usually took down everything with it.

Putting the daily crashes aside for a moment, Windows 98 did at least feel a little bit like it had a *soul*. Call me nostalgic, but I think it tried hard to be friendly and welcoming operating system that you could take seriously. It tried to help you get things done as good as it could.

I tried to extract some Plus! 98 themes and apply them to Windows 10. If you put the effort in, you can actually still customize everything from the wallpaper and colors right down to the desktop icons, mouse pointers and sounds.  That's part of what made the themes fun back in the day. But since Microsoft's Shell Team decided that Windows 10 isn't supposed to be fun, I had to resort to writing this small program to get some of the fun back.
