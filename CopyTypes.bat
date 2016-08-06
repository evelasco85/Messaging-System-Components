@ECHO OFF
ECHO requires vsvars32

xsd -nologo CreditBureau\bin\debug\CreditBureau.exe
move schema0.xsd CreditBureau\CreditBureau.xsd

xsd -nologo CreditBureau\CreditBureau.xsd /c /n:CreditBureau
copy creditBureau.cs LoanBroker\creditBureauStub.cs 
move CreditBureau.cs Test\CreditBureauStub.cs 

xsd -nologo Bank\bin\Debug\Bank.exe
move schema0.xsd Bank\Bank.xsd

xsd -nologo Bank\Bank.xsd /c /n:Bank
copy Bank.cs LoanBroker\BankStub.cs
move Bank.cs Test\BankStub.cs

xsd -nologo LoanBroker\bin\debug\LoanBroker.exe /t:LoanQuoteRequest /t:LoanQuoteReply
move schema0.xsd LoanBroker\LoanBroker.xsd

xsd -nologo LoanBroker\LoanBroker.xsd /c /n:LoanBroker
move LoanBroker.cs Test