#include <stdlib.h>
#include <string.h>

#include "LinkedList.h"

LinkedList *strings;

void initMemory() {	

	strings = initLL();
}

int getNumStrings() {
	return sizeLL(strings);
}

char * newString(int size)
{
	char * string;

	string = (char *) malloc(size + 1);
	memset(string, 0, size + 1);
	if (!NULL) {
		insertLL(strings, string, sizeLL(strings) + 1);
	}
	showLL(strings);
	return string;
}

void freeString(char * string) {
	if (removeLL(strings, string) == SUCCESS) {
		free(string);
	}
}