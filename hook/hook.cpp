#include<Windows.h>

#pragma data_seg(".hookdata")
HINSTANCE hModule = NULL;
HHOOK hKeyboardHook = NULL;
HHOOK hMouseHook = NULL;
HWND hWnd = NULL;
#pragma data_seg()
#pragma comment(linker, "/SECTION:.hookdata,RWS")

// http://www.devpia.com/MAEUL/Contents/Detail.aspx?BoardID=278&MAEULNo=20&no=13213&ref=13213	

LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
	if(nCode == HC_ACTION) 
	{
		// KeyDown
		if((wParam == 229 && lParam == -2147483647) || (wParam == 229 && lParam == -2147483648)) 
		{			
			return true; 
		}

		// KeyUp
		if(lParam == -1073741825)
		{
			return true; 
		}
	
	} 

	return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
}

LRESULT CALLBACK MouseProc(int nCode, WPARAM wParam, LPARAM lParam)
{	
	LPMOUSEHOOKSTRUCT MouseParam;
	/*
    if(nCode >= 0)
    {        
        MouseParam = (MOUSEHOOKSTRUCT *)lParam;

        if((MouseParam->hwnd == hwndMain)&&(wParam == WM_LBUTTONDOWN))
        {
            {   
				SendMessage(hwndMain, WM_USER+10, wParam, lParam);
                return true;
            }
        }
    }
	*/
	return CallNextHookEx(hMouseHook, nCode, wParam, lParam);
}

extern "C" _declspec(dllexport) void __cdecl InstallHook()
{
	hKeyboardHook = SetWindowsHookEx(WH_KEYBOARD, KeyboardProc, hModule, GetCurrentThreadId());
	hMouseHook = SetWindowsHookEx(WH_MOUSE, MouseProc, hModule, NULL);
}

extern "C" _declspec(dllexport) void __cdecl UnInstallHook()
{
	UnhookWindowsHookEx(hKeyboardHook);
	UnhookWindowsHookEx(hMouseHook);
}

#pragma unmanaged

BOOL WINAPI DllMain(HINSTANCE hInst, DWORD fdwReason, LPVOID lpRes)
{
	switch(fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		hModule = hInst;
		break;

	case DLL_PROCESS_DETACH:
		break;
	}

	return TRUE;
}

#pragma unmanaged