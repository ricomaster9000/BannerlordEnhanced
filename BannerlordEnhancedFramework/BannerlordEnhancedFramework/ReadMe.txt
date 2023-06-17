Visual Studio Setup & Debugging

Project Setup

Clone project from Github.
Open the csproj file as a project file/open/Project/Solution
Then a solution is created you can then add another project (csproj) by right click on solution on solution in Solution Explorer and Add/ExistingProject search for csproj


Debugging

Right click on the project in Solution Explorer then go to properties.
From the following menus

Build
	Output
		OutputPath ->
			It can stay bin\Debug\ since it gets manually moved in csproj file to Bannerlord Modules folder
			(This is where dll is build to)

Debug
	Start action
		Start External Program ->
			C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\Bannerlord.exe
			(Since game is run from steam we should start at that path since our mod will run from the game)

Start options
	Command line arguments ->
		/singleplayer _MODULES_*Bannerlord.Harmony*Native*SandBoxCore*CustomBattle*SandBox*StoryMode*BannerlordEnhancedPartyRolesModule*_MODULES_
		(NOTE make sure BannerlordEnhancedPartyRolesModule is name of module you use for debugging.)
	Working Directory ->
		C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\
