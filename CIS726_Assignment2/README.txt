CIS726_Assignment 4 - Readme File

Kevin Dean

Brian Dye

Russell Feldhausen
212 Nichols Hall
russfeld@ksu.edu
http://people.cis.ksu.edu/~russfeld

Notes:
  * This project was developed in Visual Studio 2012. It should work in Visual Studio 2010, but I have not tested it myself.
  * This project uses SQL Server LocalDB as the storage engine. For those of you using Visual Studio 2010, you may have to install that seperately for it to work correctly.
  * For parts that may need updated when building or deploying, search the files for "//todo"
  * For parts with special comments, search the files for "@russfeld"
  * The seed data may not be completely correct; it was the best I could do at the time. So, you may want to update the seed data to more closely match what you are wanting to do.
  * Anywhere that code/js/css was borrowed/adapted from online sources should be marked with the URL of the source. Mad Props to those websites.

Running:
  * Make sure MSMQ is installed
  * Open Visual Studio, and build the project. If NuGet is not configured to automatically download needed packages on Build, you may get an error describing how to turn that on. (In VS 2012, go to Tools > Options > Package Manager and checkmark "Allow NuGet to download missing packages during build".) If all goes well, it should successfully build.
  * Run the MessageParser.exe file from that project's bin/Debug folder (the database files should already be there)
  * Likewise, run the AuthParser.exe file from that project's bin/Debug folder (the database files should already be there)
  * Run the Web project. It should automatically connect to the running MSMQ channels
 