#pragma once

#ifdef MemDLL_EXPORTS
#define MemDLL_API __declspec(dllexport)
#else
#define MemDLL_API __declspec(dllimport)
#endif

#include <windows.h>

typedef double (CALLBACK *FPTR)(double a);

extern "C" MemDLL_API double Left(double arr[], int size, FPTR pf, double left, double right, double step);
extern "C" MemDLL_API double Trap(double arr[], int size, FPTR pf, double left, double right, double step);
extern "C" MemDLL_API double Mid(double arr[], int size, FPTR pf, double left, double right, double step);
extern "C" MemDLL_API double MidFind(double arr[], int *size, FPTR pf, double left, double right, double eps);

extern "C" MemDLL_API int G(FPTR);
extern "C" MemDLL_API int TestCallBack(int);



