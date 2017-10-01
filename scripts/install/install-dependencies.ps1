# tries to load ScriptDom, if not found download its package and unzup

Try {
	[System.Reflection.Assembly]::Load('Microsoft.SqlServer.TransactSql.ScriptDom, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91')
}
Catch {
	write-host 'Dependency not found: Microsoft.SqlServer.TransactSql.ScriptDom. Downloading from Nuget.'

	# get working directory
	$invocation = (Get-Variable MyInvocation).Value
	$working_directory = Split-Path $invocation.MyCommand.Path

	# download package from nuget
	$url = "https://www.nuget.org/api/v2/package/Microsoft.SqlServer.TransactSql.ScriptDom/12.0.0"
	$package_name = "Microsoft.SqlServer.TransactSql.ScriptDom"
	$package_file = $working_directory + "/" + $package_name + ".nupkg"
	Invoke-WebRequest $url -OutFile $package_file

	# unzip package 
	Add-Type -AssemblyName System.IO.Compression.FileSystem
	$unzip_path = $working_directory + "/" + $package_name
	[System.IO.Compression.ZipFile]::ExtractToDirectory($package_file, $unzip_path)

	# copy dll from unzipped package into tsqllint bin directory
	$dll = $unzip_path + "/lib/" + $package_name + ".dll"
	$destination = $env:npm_config_prefix + "/node_modules/tsqllint/TSQLLint.Console/bin/Release/"
	Copy-Item $dll -Destination $destination

	# cleanup
	Remove-Item $unzip_path -Force -Recurse
	Remove-Item $package_file -Force
}