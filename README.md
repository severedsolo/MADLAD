InstallValidator

JSON stanza(s) located in the .version file.

	"INSTALL_LOC":
	{
		"NAME":			"modname",		// optional
		"PATH":         "path",
		"DIRECTORY":    "dirName",
		"FILE":         "fileName",		
		"MESSAGE":      "message to display"
	}

Multiple stanzas can be listed, as long as each has a unique suffix and starts with INSTALL_LOC_.  The 
suffix can be anything you want. It would look like this:

	"INSTALL_LOC_1":
	{
		"NAME":			"modname",		// optional
		"PATH":         "path",
		"DIRECTORY":    "dirName",
		"FILE":         "fileName",
		"MESSAGE":      "message to display"
	}

	NAME		If not specified, uses the Modname from the .version file. 
	PATH		Path to the directory, begins below the GameData
	DIRECTORY	Directory where file is.
	FILE		filename to be checked
	MESSAGE		Optional, if there will be used instead of default message
				Message can have the following substitutions
					<MODNAME>	name of mod
					<FIELD>		replace with either "File", "Directory", "Path"
					<FILE>		replace with fileName
					<DIRECTORY>	replace with dirName
					<PATH>	    replace with path
					<STANZA>	replace with the stanza name

				Default message is:
					
					<MODNAME> + " has been installed incorrectly and will not function properly. All files should be located in KSP/GameData/" + <DIRECTORY>
					
                  
Note that when checking for the existance of a file, the FILE, PATH & DIRECTORY are concatted together as follows:

	PATH/DIRECTORY/FILE

This does imply that you could use only one of either PATH or DIRECTORY, if you put the complete path in that entry
