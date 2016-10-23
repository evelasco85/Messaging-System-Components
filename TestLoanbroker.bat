if "%1"=="nostart" goto nostart

call runbanks

start CreditBureau primaryCreditRequestQueue
start CreditBureau backupCreditRequestQueue
::start CreditBureau creditRequestQueue

start loanbroker loanRequestQueue creditrequestQueue creditReplyQueue bankReplyQueue

start ManagementConsole controlbusQueue creditRequestQueue monitorQueue
start CreditBureauFailOver routerControlQueue creditRequestQueue primaryCreditRequestQueue backupCreditRequestQueue

REM start loanbroker loanRequestQueue bankReplyQueue

goto label1

:nostart
shift

:label1

if "%1"=="" goto noarg

Test\bin\Debug\Test loanbroker loanRequestQueue loanReplyQueue %1
goto end

:noarg
Test\bin\Debug\Test loanbroker loanRequestQueue loanReplyQueue 50

:end