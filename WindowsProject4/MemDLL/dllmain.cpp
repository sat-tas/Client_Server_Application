// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "stdafx.h"
#include "MemDLL.h"
#include <windows.h>
#include <cstdio>
#include <iostream>


BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	{ 
		break;
	}
	case DLL_THREAD_ATTACH:
	{
		/*TCHAR buf[20] = L"\0";
		GetEnvironmentVariable(L"HANDLE", buf, sizeof(HWND));
		HWND hWND=(HWND)_wtoi(buf);
		MessageBox(hWND,L"Запущен поток",L"",MB_OK);
		*/break;
	}
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

