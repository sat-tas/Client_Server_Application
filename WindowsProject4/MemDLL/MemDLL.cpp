#include "stdafx.h"
#include "MemDLL.h"
#include <math.h>
#include <strsafe.h>
#include <objbase.h>
#include <stdio.h>


double Left(double arr[], int size,FPTR pf, double left, double right, double step)
{
	int nom = 0;
	double res = 0;
	for (double i = left; i <= right + step / 2; i += step)
	{
		arr[nom++] = i;
		arr[nom++] = pf(i + step);
		arr[nom++] = i + step;
		arr[nom++] = arr[nom-2];
		res += step * arr[nom-1];
	}
	return res;
}

double Right(double arr[], int size, FPTR pf, double left, double right, double step)
{
	int nom = 0;
	double res = 0;
	for (double i = left; i <= right + step / 2; i += step)
	{
		arr[nom++] = i;
		arr[nom++] = pf(i + step);
		arr[nom++] = i + step;
		arr[nom++] = arr[nom - 2];
		res += step * arr[nom - 1];
	}
	return res;
}

double Mid(double arr[], int size, FPTR pf, double left, double right, double step)
{
	int nom = 0;
	double res = 0;
	for (double i = left; i <= right + step / 2; i += step)
	{
		arr[nom++] = i;
		arr[nom++] = pf(i + step/2);
		arr[nom++] = i + step;
		arr[nom++] = arr[nom - 2];
		res += step * arr[nom - 1];
	}
	return res;
}

double Trap(double arr[], int size, FPTR pf, double left, double right, double step)
{
	int nom = 0;
	double res = 0;
	for (double i = left; i <= right + step / 2; i += step)
	{
		arr[nom++] = i;
		arr[nom++] = pf(i);
		arr[nom++] = i + step;
		arr[nom++] = pf(i+step);
		res += step * (arr[nom-3] + arr[nom-1]) / 2;
	}
	return res;
}

double MidFind(double arr[], int *size, FPTR pf, double left, double right, double eps)
{
	double leftX = left;
	double rightX = right;
	double nowX = (leftX + rightX) / 2;
	int count = 0;
	while (fabs(pf(nowX)) > eps )
	{
		if (count < 1000)
		{
			arr[count++] = nowX;
			arr[count++] = 0;
			arr[count++] = nowX;
			arr[count++] = pf(nowX);
		}
		if ((pf(leftX) > 0 && pf(nowX) < 0) || (pf(leftX) < 0 && pf(nowX) > 0))
		{
			rightX = nowX;
		}
		else
		{
			leftX = nowX;
		}
		nowX = (leftX + rightX) / 2;
	}
	memcpy(size, &count, 4);
	return nowX;
}




