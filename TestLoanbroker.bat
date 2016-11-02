if "%1"=="nostart" goto nostart

call runbanks

::start 
start StartCreditBureau_Primary
start StartCreditBureau_Primary
start StartCreditBureau_Backup
start StartCreditBureau_Backup



start loanbroker loanRequestQueue creditrequestQueue creditReplyQueue bankReplyQueue
start ManagementConsole controlbusQueue creditRequestQueue_PrimaryProcessor monitorReplyQueue routerControlQueue
start CreditBureauFailOver\bin\Debug\CreditBureauFailOver 962c38f8-35a1-4ace-811e-73a100295e6f\1 ServerRequestQueue ServerReplyQueue

Test\bin\Debug\Test 92022db8-750a-4481-afc7-dc2dcfb8fc20\1 ServerRequestQueue ServerReplyQueue

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

:end