@echo "###### Execute Test Request ######"
TestHarness\Repository\UI /r .\TestHarness\Repository /t .\TestHarness\Repository\SampleCodeTestRequest.xml .\TestHarness\Repository\XmlParserTestRequest.xml

@echo "###### Client query for Test Request Logs ######"
TestHarness\Repository\UI /r .\TestHarness\Repository /q .\TestHarness\Repository\SampleCodeTestRequest.xml

@echo "###### Client query for all author tests ######"
TestHarness\Repository\UI /r .\TestHarness\Repository /a "Sahil"

@echo " ###### Client query test summary ######"
TestHarness\Repository\UI /r .\TestHarness\Repository /s

