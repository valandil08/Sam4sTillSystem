# Sam4s Till System
.NET 4.6 WPF based till software built for the Sam4s S160 till system

This is uploaded for proof of work purposes and is not maintained, if you wish to modify it please fork it into a seperate repo instead of creating pull requests

Copy the AppSettings folder to the root of the build folder before running, it contains a default set of configuration files that the application depends on

How the config elements reference each other
![image](https://user-images.githubusercontent.com/62212805/177217260-e31a5e01-c83a-4159-8c7f-7f78c264637f.png)

All the values highlighted in yellow are used for referencing what control is used or references another id of another control
Make sure these are correct as a incorrect value will cause the system to crash.

All the values not highlighted in yellow are values specific to the element and don't reference anything outside of that element.

This was designed to be configured via a website that validate all the inputs not via a user.
