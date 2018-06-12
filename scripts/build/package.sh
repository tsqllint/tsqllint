WORKING_DIRECTORY=$(pwd)
cd assemblies
tar -zcvf   osx-x64.tgz   osx-x64
tar -zcvf   win-x86.tgz   win-x86
tar -zcvf   win-x64.tgz   win-x64
tar -zcvf linux-x64.tgz linux-x64
cd $WORKING_DIRECTORY