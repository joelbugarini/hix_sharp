# hoob code generator
hoob is a code generator, create an .hoob scheme file and the generator will iterate over your Database, tables and columns.

## File extension
hoob read files with the extension ".hoob" e.g.:
 - file.htm.hoob
 
The name of the file will be used as a Prefix for the generated code file and the extension before ".hoob" will be the code file extension e.g.:

 - file { table name } .htm
  
 
## Syntax

Just grab the pice of code you want to generate and use this syntaxt to generate your schema:

page.htm.rave

```aspx
[<]
<!doctype html>
<html lang="en">
<head>
	<meta http-equiv="content-type" content="text/html; charset=utf-8">
	<title>[[project.name]]</title>
</head>
	<body>
		[[table]]
		<p>This is the table [[table.name]] </p>
		
		<table>
			[[column]]
			<tr>
				<td>
					Column: [[column.name]]
				</td>
				<td>
					Type: [[column.type]]
				</td>
			</tr>
			[[/column]]
		</table>

		[>]
	</body>
</html>
[>]
```

##Functions
This are the **Loop** functions:
 *   **`[<]`** - This tag marks the begining of the document.
 *   **`[>]`** - This tag marks the end of the document.
 *   **`[[table]]`** - This tag marks the begining of the loop through the tables. Should be inside the RootNode
 *   **`[>]`** - This tag marks the end of the loop through the tables.
 *   **`[[column]]`** - This tag marks the begining of the loop through the columns. Should be inside the ColumnNode
 *   **`[[/column]]`** - This tag marks the end of the loop through the columns.
 

This are the **Properties** functions:
 * **`[[project.name]]`** - Get the project name from the config.json file.
 * **`[[database.name]]`** - Get the database name from the config.json file. 
 * **`[[table.name]]`** - Get the table name from the Table in course.
 * **`[[column.name]]`** - Get the column name from the Column in course.
 * **`[[column.type]]`** - Get the column type from the Column in course.

##Connection to Database
The conection string and some properties are loaded from the `config.json` file. Currently just works with SQL Server and LocalDB, but I am working on other databases.

##Execution


Just execute the exe from the command line and a folder will be created for the files created.

`hoob file.htm` (it has to be an *.hoob file) will output a folder:

file
```bash
-| ../ 
 |-fileTableA.htm
 |-fileTableB.htm
 |-fileTableC.htm
```

