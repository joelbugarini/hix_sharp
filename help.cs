using System;
using System.Collections.Generic;
using System.Text;

namespace hix
{
    public class help
    {
        public static string text { get; set; } = @"watching types
	hix types
		-It gives a list of any possible type in the Database

	hix typesOf <Table>
		-It gives a list of all the types of the specified Table

generating code
	hix generate <File>
		-It takes the specified file and generate code for every table,
		then saves the files in the output folder this way:
			./output/<FileName>/<TableName>.<FileExtension>

generating single file
	hix generate -f <File> <Table>
		-It takes the specified file and generate code for the specified
		 table, then saves the files in the output folder this way:
			./output/<FileName>/<TableName>.<FileExtension>

hix generate -c <File> <Table>

utility
	hix generate -p <File>
		-It takes the specified file and generate code for every table,
		then saves the files in the output folder with the filename as
		a prefix this way:
			./output/<FileName>/<FileName><TableName>.<FileExtension>

	hix generate -s <File>
		-It takes the specified file and generate code for every table,
		then saves the files in the output folder with the filename as
		a sufix this way:
			./output/<FileName>/<TableName><FileName>.<FileExtension>"; 
    }
}
