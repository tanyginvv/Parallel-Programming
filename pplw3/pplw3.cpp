#include <iostream>
#include <stdio.h>
#include <string>
#include <windows.h>
#include <fstream>
#pragma comment(lib, "winmm.lib")

using namespace std;

DWORD startTime = 0;
SYSTEMTIME sttime;

ofstream thread_first = ofstream("t1.log");
ofstream thread_second = ofstream("t2.log");

DWORD WINAPI ThreadProc(CONST LPVOID lpParam)
{
    int param = (int)lpParam;
    for (int j = 0; j < 20; j++)
    {
        for (int i = 0; i < 10000000; i++)
        {
            int iteration = 1 / sqrt(11);
        }
        SYSTEMTIME curtime;
        GetSystemTime(&curtime);

        DWORD_PTR currentTime = timeGetTime();

        float t = 2 * ((currentTime - startTime) % 20000) / 20000.0f;
        if (t > 1.0f)
        {
            t = 2 - t;
        }
        int calcul1 = sttime.wSecond * 1000 + sttime.wMilliseconds;
        int calcul2 = curtime.wSecond * 1000 + curtime.wMilliseconds;
        string dur = to_string(calcul2 - calcul1);
        if (param == 1)
        {
            thread_first << dur << endl;
        }
        if (param == 2)
        {
            thread_second << dur << endl;
        }
    }

    ExitThread(0);
}

int main()
{
	int count_threads = 2;
	HANDLE* handles = new HANDLE[count_threads];
	for (int i = 0; i < count_threads ; i++)
	{
		int ThreadCount = i + 1;
		handles[i] = CreateThread(NULL, 0, &ThreadProc, (LPVOID)ThreadCount, CREATE_SUSPENDED, NULL);
	}
    SetThreadPriority(handles[1], 1);
	for (int i = 0; i < count_threads; i++)
	{
		ResumeThread(handles[i]);
	}

	WaitForMultipleObjects(count_threads, handles, true, INFINITE);
	return 0;
}