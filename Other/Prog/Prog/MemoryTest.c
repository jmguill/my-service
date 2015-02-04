#include <string.h>

#include "CuTest.h"
#include "Memory.h"


void TestInitMemory(CuTest *tc) {
    initMemory();
	CuAssertIntEquals(tc, getNumStrings(), 0);
}

void TestNewString(CuTest *tc) {
	char * myString;

	initMemory();
	CuAssertIntEquals(tc, 0, getNumStrings());
	myString = newString(10);
	CuAssertIntEquals(tc, 1, getNumStrings());
	CuAssertTrue(tc, (myString[0] == NULL));
}

void TestAddAndRemoveStrings(CuTest *tc) {
	char string1[] = "top expert\0";
	char string2[] = "talented sans fellow\0";
	char string3[] = "hotel california\0";
	char *strPtr1, *strPtr2, *strPtr3;

	initMemory();
	strPtr1 = newString(strlen(string1));
	CuAssertIntEquals(tc, 1, getNumStrings());
	strcpy(strPtr1, string1);
	strPtr2 = newString(strlen(string2));
	CuAssertIntEquals(tc, 2, getNumStrings());
	strcpy(strPtr2, string2);
	strPtr3 = newString(strlen(string3));
	CuAssertIntEquals(tc, 3, getNumStrings());
	strcpy(strPtr3, string3);

	CuAssertIntEquals(tc, 3, getNumStrings());
	CuAssertStrEquals(tc, string1, string1);
}

CuSuite* CuMemoryGetSuite() {
	CuSuite* suite = CuSuiteNew();
	
	SUITE_ADD_TEST(suite, TestInitMemory);
	SUITE_ADD_TEST(suite, TestNewString);
	SUITE_ADD_TEST(suite, TestAddAndRemoveStrings);

    return suite;
}
