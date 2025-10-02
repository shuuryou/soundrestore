#include <windows.h>
#include <mmsystem.h>
#include <wtsapi32.h>

#pragma comment(lib, "kernel32.lib")
#pragma comment(lib, "user32.lib")
#pragma comment(lib, "winmm.lib")
#pragma comment(lib, "wtsapi32.lib")

/* Microsoft:
 *
 * ----------------------------------------------------------------------
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
 * ----------------------------------------------------------------------
 *
 * TLDR: We could have kept the sounds, but weren't competent enough to
 * do it, so we decided to just kill the feature.
 *
 * Putting aside the fact that the WAV file could've been loaded into
 * memory earlier, and that the registry settings could've been cached,
 * just keeping all of the existing and working code to play the sound
 * inside Explorer would have cost nothing.
 */

static LRESULT CALLBACK WindowProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
    switch (uMsg) {
    case WM_WTSSESSION_CHANGE:
        if (wParam == WTS_SESSION_LOCK)
            PlaySoundW(L"WindowsLogoff", NULL, SND_ALIAS | SND_SYSTEM | SND_ASYNC);
        else if (wParam == WTS_SESSION_UNLOCK)
            PlaySoundW(L"WindowsLogon", NULL, SND_ALIAS | SND_SYSTEM | SND_ASYNC);
        return 0;
    case WM_QUERYENDSESSION:
        return TRUE;
    case WM_ENDSESSION:
        if (wParam) {
            ShutdownBlockReasonCreate(hWnd, L"Playing shutdown sound");
            // We do not use SND_ASYNC here to block the thread
            PlaySoundW(L"SystemExit", NULL, SND_ALIAS | SND_SYSTEM | SND_SYNC);
            ShutdownBlockReasonDestroy(hWnd);
            PostQuitMessage(0);
        }
        return 0;
    case WM_DESTROY:
        PostQuitMessage(0);
        return 0;
    default:
        return DefWindowProcW(hWnd, uMsg, wParam, lParam);
    }
}

extern "C" void __cdecl Entry() {
    HANDLE hMutex = CreateMutexW(NULL, TRUE, L"sndrst_d3f9ae65");
    if (GetLastError() == ERROR_ALREADY_EXISTS) {
        if (hMutex) CloseHandle(hMutex);
        ExitProcess(0);
    }

    WNDCLASSEXW wc;
    wc.cbSize = sizeof(WNDCLASSEXW);
    wc.style = 0;
    wc.lpfnWndProc = WindowProc;
    wc.cbClsExtra = 0;
    wc.cbWndExtra = 0;
    wc.hInstance = GetModuleHandleW(NULL);
    wc.hIcon = NULL;
    wc.hCursor = NULL;
    wc.hbrBackground = NULL;
    wc.lpszMenuName = NULL;
    wc.lpszClassName = L"DummyForm";
    wc.hIconSm = NULL;
    RegisterClassExW(&wc);

    HWND hWnd = CreateWindowExW(WS_EX_NOACTIVATE, L"DummyForm", L"System Sound Player", 0, 0, 0, 0, 0, NULL, NULL, wc.hInstance, NULL);
    if (!hWnd) {
        if (hMutex) CloseHandle(hMutex);
        ExitProcess(0);
    }

    ShowWindow(hWnd, SW_HIDE);
    WTSRegisterSessionNotification(hWnd, NOTIFY_FOR_THIS_SESSION);
    PlaySoundW(L"SystemStart", NULL, SND_ALIAS | SND_SYSTEM | SND_ASYNC);

    MSG msg;
    while (GetMessageW(&msg, NULL, 0, 0)) {
        TranslateMessage(&msg);
        DispatchMessageW(&msg);
    }

    WTSUnRegisterSessionNotification(hWnd);
    if (hMutex) CloseHandle(hMutex);
    ExitProcess(0);
}