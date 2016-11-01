if "%1"=="nostart" goto nostart

call runbanks

::start 
start StartCreditBureau_Primary
start StartCreditBureau_Primary
start StartCreditBureau_Backup
start StartCreditBureau_Backup

start loanbroker loanRequestQueue creditrequestQueue creditReplyQueue bankReplyQueue

start ManagementConsole controlbusQueue creditRequestQueue_PrimaryProcessor monitorReplyQueue routerControlQueue
start CreditBureauFailOver routerControlQueue creditRequestQueue creditRequestQueue_PrimaryProcessor creditRequestQueue_BackupProcessor

REM start loanbroker loanRequestQueue bankReplyQueue

goto label1

:nostart
shift

:label1

if "%1"=="" goto noarg

Test\bin\Debug\Test loanbroker loanRequestQueue loanReplyQueue %1
goto end

:noarg
::Test\bin\Debug\Test loanbroker loanRequestQueue loanReplyQueue 50
Test\bin\Debug\Test 92022db8-750a-4481-afc7-dc2dcfb8fc20\1 ServerRequestQueue ServerReplyQueue



:end