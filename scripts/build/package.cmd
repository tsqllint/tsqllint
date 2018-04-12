cd assemblies && 7z a -ttar -so osx-x64.tar     osx-x64 | 7z a -si   osx-x64.tgz && cd ..
cd assemblies && 7z a -ttar -so win-x64.tar     win-x64 | 7z a -si   win-x64.tgz && cd ..
cd assemblies && 7z a -ttar -so win-x86.tar     win-x86 | 7z a -si   win-x86.tgz && cd ..
cd assemblies && 7z a -ttar -so linux-x64.tar linux-x64 | 7z a -si linux-x64.tgz && cd ..
