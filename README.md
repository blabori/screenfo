# screenfo
Windows tool to gather information about connected displays using Win32 Setup-API.

# Summary
Serial numbers and model names of display devices can easily be gathered by WMI etc, but you do not get the resolution of the monitor. To get the resolution you would probably use something like Screen.AllScreens, unfortunately you cannot link these two pieces of information together because there is no shared identifier. With calls to the Win32 Setup-API, however, you are able to get this link ;-)

# Remarks
Struct and function definitions were mostly taken from pInvoke.net, the extraction of the EDID from registry is based on the code from Roger Zander (rzander), see [here]. 

[here]: https://github.com/rzander/MonitorDetails/blob/master/DisplayInfoWMIProvider/WMIProvider.cs
