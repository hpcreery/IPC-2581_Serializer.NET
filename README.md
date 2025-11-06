# IPC-2581 Serializer and Validation .NET Console App

### Getting Started
Requirements
- .NET Version 8.0 or Later
```
➜ dotnet run
Enter path to IPC-2581 XML file: /Users/Me/Developer/PCBs/IPC-2581 Test Cases/Testcase1-RevC/testcase1-RevC-full.xml
Select version to validate:
1. Rev A
2. Rev B
3. Rev B1
4. Rev C
Enter choice (1-4): 4
Error | Line: 1834 Position: 12 | The element 'Circle' in namespace 'http://webstds.ipc.org/2581' has invalid child element 'Xform' in namespace 'http://webstds.ipc.org/2581'. List of possible elements expected: 'LineDescGroup, LineDesc, LineDescRef, FillDescGroup, FillDesc, FillDescRef' in namespace 'http://webstds.ipc.org/2581'.
Error | Line: 1853 Position: 12 | The element 'Circle' in namespace 'http://webstds.ipc.org/2581' has invalid child element 'Xform' in namespace 'http://webstds.ipc.org/2581'. List of possible elements expected: 'LineDescGroup, LineDesc, LineDescRef, FillDescGroup, FillDesc, FillDescRef' in namespace 'http://webstds.ipc.org/2581'.
...
```
