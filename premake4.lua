--
-- Define the project. Put the release configuration first so it will be the
-- default when folks build using the makefile. That way they don't have to 
-- worry about the /scripts argument and all that.
--

if not _OPTIONS["to"] then
	_OPTIONS["to"] = "."
end
--
-- MonoEmbed
--
solution "MonoEmbed"
	configurations { "Release", "Debug" }
	location ( _OPTIONS["to"] )
	
	configuration { "windows" }
		platforms { "x32", "x64" }
	configuration { "macosx" }
		platforms { "x32" }
	
	configuration "Debug"
		defines     "DEBUG"
		flags       { "Symbols" }
		
	configuration "macosx"
		defines		"MAC"
		
	configuration { "windows" }
		defines { "WIN32", "WINDOWS" }
		
	configuration "Release"
		defines     "RELEASE"
		flags       { "OptimizeSpeed" }
		
	configuration { "Debug", "x32" }
		targetdir   "bin/Debug"
		
	configuration { "Debug", "x64" }
		targetdir   "bin/Debug64"
		
	configuration { "Release", "x32" }
		targetdir   "bin/Release"
		
	configuration { "Release", "x64" }
		targetdir   "bin/Release64"
		
		
--
-- MonoEmbed project
--

	project "MonoEmbed"
		uuid		"ae8e1ea1-5b13-11df-a08a-0800200c9a66"
		targetname  "MonoEmbed"
		location 	( _OPTIONS["to"] )
		language    "C++"
		kind        "ConsoleApp"
		flags       { "FatalWarnings", "NoExceptions", "NoRTTI", "FloatFast" }

		includedirs 
		{ 
			"src/",
			"include/",
		}

		files 
		{
			"src/*.cpp", 
			"src/*.h", 
			"include/*.h",
		}

		configuration "macosx"
			linkoptions
			{ 
				"-framework IOKit",
				"-framework QuartzCore",
				"-framework CoreFoundation",
				"-framework Foundation",
			}
			includedirs
			{
				"/Library/Frameworks/Mono.framework/Versions/2.10.1/include/mono-2.0",
			}
			libdirs
			{
				"/Library/Frameworks/Mono.framework/Versions/2.10.1/lib",
			}
			links
			{
				"mono-2.0",
				"pthread"
			}
			defines { "_THREAD_SAFE" }
			
		configuration "windows"
			includedirs { "C:\\Program Files (x86)\\Mono-2.10.1\\include\\mono-2.0" }
			libdirs { "C:\\Program Files (x86)\\Mono-2.10.1\\lib" }
			links { "mono" }
			defines { "_THREAD_SAFE" }
			
		configuration { "vs2010" }
			postbuildcommands
			{
				"copy /Y "C:\Program Files (x86)\Mono-2.10.1\bin\mono-2.0.dll" "$(ProjectDir)bin\$(Configuration)\""
			}
			

--
-- A more thorough cleanup.
--

if _ACTION == "clean" then
	os.rmdir("bin")
	os.rmdir("build")
	os.rmdir("obj")
elseif os.is("macosx") then
	printf("Cleaning user files...")
	local filesToRemove = { "*.pbxuser", "*.xcuserdatad" }
	for j, file in ipairs(filesToRemove) do
		local findCommand = "find " .. path.getabsolute(_OPTIONS["to"]) .. " -name \"" .. file .. "\""
		local xargsRemoveCommand = "xargs rm -rf"
		local finalCommand = findCommand .. " | " .. xargsRemoveCommand

		os.execute(finalCommand)
	end
end
			
newoption {
	trigger = "to",
	value   = "path",
	description = "Set the output location for the generated files",
	optional = true
}