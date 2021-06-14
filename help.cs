using System;
using System.Collections.Generic;
using System.Text;

namespace hix
{
    public class help
    {
        public static string text { get; set; } = @"
watching models
	hix models
		-It gives a list of all the Modela

generating code
	hix generate <File>
		-It takes the specified file and generate code for every table,
		then saves the files in the output folder this way:
			./output/<FileName>/<ModelName>.<FileExtension>

generating single file
	hix generate -f <File> <Model>
		-It takes the specified file and generate code for the specified
		 table, then saves the files in the output folder this way:
			./output/<FileName>/<ModelName>.<FileExtension>

hix generate -c <File> <Model>

utility
	hix generate -p <File>
		-It takes the specified file and generate code for every table,
		then saves the files in the output folder with the filename as
		a prefix this way:
			./output/<FileName>/<FileName><ModelName>.<FileExtension>

	hix generate -s <File>
		-It takes the specified file and generate code for every table,
		then saves the files in the output folder with the filename as
		a sufix this way:
			./output/<FileName>/<ModelName><FileName>.<FileExtension>"; 
    }
}
