STEPS TO EXECUTE MESSAGE QUEUE SAMPLE

[BUILD COMPONENENTS AND RUN WEB SIGNAL-R IMPLEMENTATION]
STEP - 01: Open 'ComposedMessaging.sln' in Visual Studio 2013 or latest
STEP - 02: Build 'ComposedMessaging.sln' by right-clicking the solution and select 'Build solution'
STEP - 03: Set 'Web' project as start-up project by right-clicking the project and select 'Set as StartUp Project'
STEP - 04: 'Run' the solution by selecting "DEBUG >> Start Debugging" in Visual Studio Menu


[RUN MESSAGE QUEUE COMPONENTS]
STEP - 01: Go to solution directory
STEP - 02: Locate and execute 'CLICK_ME_TestLoanbroker.bat' batch file
	>> Ensure that queues are empty(no outstanding queues)
	>> 'Management Console' application will be available at this point
STEP - 03: Click 'Activate Clients' button from 'Management Console'
	>> Observe message queue component activities choreograph
	>> When all-is-good, proceed to [USING WEB END-POINT] section

[USING WEB END-POINT]
STEP - 01: Provided '[RUN MESSAGE QUEUE COMPONENTS]' is up-and-running, navigate to'http://localhost:10627/loanbroker' using web browser
STEP - 02: Click 'Start Loan Broker' button to start sending loan requests
	>> Multiple instance/session (running on different browsers) is applicable when desired
	>> Click 'Start Loan Broker' button again (until successfully invoked) if the error below is encountered:
	>>>>>Uncaught Error: SignalR: Connection has not been fully initialized. Use .start().done() or .start().fail() to run logic after the connection has started.(…)