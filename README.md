# Aven code generator
Aven is a code generator, create an .aven scheme file and the generator will iterate over your Database, tables and columns.

## File extension
Aven read files with the extension ".aven" e.g.:
 - file.htm.aven
 
The name of the file will be used as a Prefix for the generated code file and the extension before ".aven" will be the code file extension e.g.:

 - file { table name } .htm
  
 
## Syntax

Just grab the pice of code you wanna generate and use this syntaxt to generate your schema:

page.htm.rave

```html
<<RootNode>>
<!doctype html>
<html lang="en">
<head>
	<meta http-equiv="content-type" content="text/html; charset=utf-8">
	<title><<Project.Name>></title>
</head>
	<body>
		<<TableNode>>
		<p>This is the table <<Table.Name>> </p>
		
		<table>
			<<ColumnNode>>
			<tr>
				<td>
					Column: <<Column.Name>>
				</td>
				<td>
					Type: <<Column.Type>>
				</td>
			</tr>
			<<EndColumnNode>>
		</table>

		<<EndTableNode>>
	</body>
</html>
<<EndRootNode>>
```

##Functions
This are the **Loop** functions:
 *  **<<RootNode>>** - This tag marks the begining of the document.
 *   **<<EndRootNode>>** - This tag marks the end of the document.
 *   **<<TableNode>>** - This tag marks the begining of the loop through the tables. Should be inside the RootNode
 *   **<<EndTableNode>>** - This tag marks the end of the loop through the tables.
 *   **<<ColumnNode>>** - This tag marks the begining of the loop through the columns. Should be inside the ColumnNode
 *   **<<EndColumnNode>>** - This tag marks the end of the loop through the columns.
 

This are the **Properties** functions:
 * **<<Project.Name>>** - Get the project name from the config.json file.
 * **<<Database.Name>>** - Get the database name from the config.json file. 
 * **<<Table.Name>>** - Get the table name from the Table in course.
 * **<<Column.Name>>** - Get the column name from the Column in course.
 * **<<Column.Type>>** - Get the column type from the Column in course.
