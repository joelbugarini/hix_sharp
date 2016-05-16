# hix code generator
hix is a code generator, create a .hix scheme file and the generator will iterate over your Database, tables and columns.

## File extension
hix read files with the extension ".hix" e.g.:
 - file.htm.hix
 
The name of the file will be used as a Prefix for the generated code file and the extension before ".hix" will be the code file extension e.g.:

 - file { table name } .htm
  
 
## Syntax

Just grab the pice of code you want to generate and use this syntaxt to generate your schema:

page.htm.hix

![hix-type](https://cloud.githubusercontent.com/assets/4912547/15022272/b5fbbbf0-11df-11e6-8bfc-577859ead4e6.JPG)



![hix-cap](https://cloud.githubusercontent.com/assets/4912547/15022273/b5fc088a-11df-11e6-8c51-268c1f078779.JPG)

Save the template next to the `config` file and execute the command:
```bash
hix generate page.htm
```

Note: the file name goes without the `hix` extension

This will generate a file with all the tables under an `output` folder, but if you want to generate a file per table you can join the root element to the table element as follows:

![hix-cap2](https://cloud.githubusercontent.com/assets/4912547/15022274/b5fc65c8-11df-11e6-81d7-75c373329401.JPG)

##Reserved Words
This are the **Loop** functions:
 *   **`[<]`** - This tag marks the begining of the document.
 *   **`[>]`** - This tag marks the end of the document.
 *   **`[[table]]`** - This tag marks the begining of the loop through the tables. Should be inside the RootNode
 *   **`[[/table]]`** - This tag marks the end of the loop through the tables.
 *   **`[[column]]`** - This tag marks the begining of the loop through the columns. Should be inside the ColumnNode
 *   **`[[/column]]`** - This tag marks the end of the loop through the columns.
 

This are the **Properties** functions:
 * **`[[project.name]]`** - Get the project name from the config.json file.
 * **`[[database.name]]`** - Get the database name from the config.json file. 
 * **`[[table.name]]`** - Get the table name from the Table in course.
 * **`[[column.name]]`** - Get the column name from the Column in course.
 * **`[[column.type]]`** - Get the column type from the Column in course.

##Column types
The `[[column]]` tag is used to iterate over every single column of the table, but you can use the data type to use different code for each type `[[column int]]`:

```aspx
[<][[table]]
<!doctype html>
<html lang="en">
<head>
	<meta http-equiv="content-type" content="text/html; charset=utf-8">
	<title>[[project.name]]</title>
</head>
	<body>
		<p>This is the table [[table.name]] </p>
		
		<table>
			[[column int]]
			<tr>
				<td style="color:red">
					Column: [[column.name]]
				</td>
				<td>
					Type: [[column.type]]
				</td>
			</tr>
			[[/column int]]
			
			[[column datetime]]
			<tr>
				<td style="color:blue">
					Column: [[column.name]]
				</td>
				<td>
					Type: [[column.type]]
				</td>
			</tr>
			[[/column datetime]]
		</table>
	</body>
</html>
[[/table]][>]
```

Only Columns of the specified types will be generated in the file. If you need to know the aviable types just use the command: `hix types` or: `hix types TableName` for a specific table.

##Connection to Database
The conection string and some properties are loaded from the `config.json` file. 

##Execution


Just execute the exe from the command line and a folder will be created for the files created.

`hix generate file.htm` (it has to be a *.hix file) in `output/name/` folder:

file
```bash
-| ../ 
 |-fileTableA.htm
 |-fileTableB.htm
 |-fileTableC.htm
```

Author Notes:
Currently just works with SQL Server and LocalDB, but I am working on other databases. As you can see in the source, this is not a parser nor an interpreter, is just a tool that helps me to avoid tedious work, maybe in the near future I will have the time to rewrite the code to include a parser, debugger or use another language for use of others developers using different development environment. Maybe I'll take the Javascript/Node wave? Haskell for learning propouses? just port it to Mono for any OS? any suggestion will be appreciated.
In the mean time, i hope you will find this tool useful.
