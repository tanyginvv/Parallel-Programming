#include <windows.h>
#include <iostream>
#include <string>

DWORD WINAPI ThreadProc(CONST LPVOID lpParam) {
    int threadNum = *(int*)lpParam;
    std::cout  << threadNum << " thread is working " << std::endl;
    return 0;
}

int main(int argc, char* argv[]) {
    if (argc != 2) {
        std::cout << "Usage: pplw1.exe [quantity of threads]" << std::endl;
        return 1;
    }

    int numThreads = std::stoi(argv[1]);
    HANDLE* handles = new HANDLE[numThreads];
    int* threadNums = new int[numThreads];

    for (int i = 0; i < numThreads; i++) {
        threadNums[i] = i + 1;
        handles[i] = CreateThread(NULL, 0, &ThreadProc, &i , 0, NULL);
        if (handles[i] == NULL) {
            std::cerr << "Failed to create thread " << i + 1 << std::endl;
            return 1;
        }
    }

    for (int i = 0; i < numThreads; i++) {
        ResumeThread(handles[i]);
    }

    WaitForMultipleObjects(numThreads, handles, true, INFINITE);

    for (int i = 0; i < numThreads; i++) {
        CloseHandle(handles[i]);
    }

    delete[] handles;
    delete[] threadNums;

    return 0;
}
