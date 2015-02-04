#include <stdio.h>

#include "CuTest.h"

CuSuite* CuGetSuite();
CuSuite* CuStringGetSuite();

void RunAllTests(void)
{
	CuString *output = CuStringNew();
	CuSuite* suite = CuSuiteNew();

	CuSuiteAddSuite(suite, CuGetSuite());
	CuSuiteAddSuite(suite, CuStringGetSuite());
	CuSuiteAddSuite(suite, CuLinkedListGetSuite());
	CuSuiteAddSuite(suite, CuMemoryGetSuite());

	CuSuiteRun(suite);
	CuSuiteSummary(suite, output);
	CuSuiteDetails(suite, output);
	printf("%s\n", output->buffer);

	getchar();
}

int main(void)
{
	RunAllTests();
}
