call runbanks

::start StartCreditBureau_Primary
::start StartCreditBureau_Primary
start StartCreditBureau_Backup
start StartCreditBureau_Backup

start LoanBroker
start CreditBureauFailOver
start ManagementConsole

Test\bin\Debug\Test 92022db8-750a-4481-afc7-dc2dcfb8fc20\1 ServerRequestQueue ServerReplyQueue