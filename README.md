# ExcelTrans

**ExcelTrans** Transforms Csv to Excel spreadsheets using .NET and EPPlus.

It has the following services:

* ExcelTrans a command based stream csv transformer
* CsvReader for quickly reading csv documents
* CsvWriter for quickly writing csv documents
* ExcelReader for quickly reading excel documents

# Source Code

This repository holds the implementation of ExcelTrans in C#.


## Simple Example

```csharp
static (Stream stream, string meta, string path) MakeInvoiceFile(IEnumerable<MyData> myData)
{
    var transform = ExcelService.Encode(new List<IExcelCommand>
    {
        new WorksheetGet("Invoice"),
        new CellStyle(Address.Range, 0, 1, 2, 1, "lcYellow"),
    });
    
    var s = new MemoryStream() as Stream;
    var w = new StreamWriter(s);
    // add transform to output
    w.WriteLine(transform);
    // add csv file to output
    CsvWriter.Write(w, myData);
    s.Position = 0;
    var result = (s, "text/csv", "invoice.csv");
    // optionally transform
    result = ExcelService.Transform(result);
    return result;
}

static void TransferFile(string path, Stream stream, string file)
{
    path = Path.Combine(path, file);
    if (!Directory.Exists(Path.GetDirectoryName(path)))
        Directory.CreateDirectory(Path.GetDirectoryName(path));
    using (var fileStream = File.Create(path))
    {
        stream.CopyTo(fileStream);
        stream.Seek(0, SeekOrigin.Begin);
    }
}

var path = ...some path...;
var myData = ...some data...;
var file = MakeInvoiceFile(myData);
TransferFile(path, file.stream, file.path);
```


# Reference

Class and Enum definitions.

## Address
*Values for the Address fields*

Enum    | Example   | Description
---     | ---:      | ---
Cell    | A1        | Cell relative address
CellAbs | A1        | Cell absolute address
Range   | A1:B1     | Range relative address
RangeAbs| A1:B1     | Range absoulute address
RowOrCol| A or 1    | Row or Column address
ColToCol| A:B       | Column to Column address
RowToRow| 1:2       | Row to Row addess


## Commands
*List of available commands*

Command     | Description | See Also
---         | --- | ---:
CellStyle   | Applies `.Styles` to the `.Cells` in range | [Styles](#styles)
CellValidation | Applies `.Rules` of `.ValidationKind` to the `.Cells` in range | [CellValidationKind](#cellvalidationkind) [ValidationRules](#validationrules)
CellValue   | Applies `.Value` of `.ValueKind` to the `.Cells` in range | [CellValueKind](#cellvaluekind)
ColumnValue | Applies `.Value` of `.ValueKind` to the `.Col` column | [ColumnValueKind](#columnvaluekind)
Command     | Executes `.Action()`
CommandCol  | Executes `.Func()` per Column
CommandRow  | Executes `.Func()` per Row
ConditionalFormatting | Applies json `.Value` of `.FormattingKind` to `.Address` in range | [ConditionalFormattingKind](#conditionalformattingkind) [Conditionals](#conditional)
Drawing     | Applies json `.Value` of `.DrawingKind` with `.Name` to `.Address` in range | [DrawingKind](#drawingkind) [Drawings](#drawings)
Flush       | Flushes all pending commands
PopFrame    | Pops a Frame off the context stack
PopSet      | Pops a Set off the context stack
Protection  | Applies `.Value` of `.ProtectionKind` to current worksheet | [ProtectionKind](#protectionkind)
PushFrame   | Pushes a new Frame with `cmds` onto the context stack
PushSet     | Pushes a new Set with `group` and `cmds` onto the context stack
RowValue    | Applies `.Value` of `.ValueKind` to the `.Row` row | [RowValueKind](#rowvaluekind)
VbaCodeModule | Applies `.Code` of `.ModuleKind` with `.Name` to the VbaProject | [VbaModuleKind](#vbamodulekind)
VbaModule   | Applies `.Code` with `.Name` to the VbaProject | [VbaCode](#vbacode)
VbaReference | Adds `.Libraries` of type VbaLibrary to the VbaProject | [VbaLibrary](#vbalibrary)
ViewAction  | Applies `.Value` of `.ActionKind` to the active spreadsheet | [ViewActionKind](#viewactionkind)
WorkbookName   | Applies `.Name` range of `.NameKind` to the `.Cells` in range | [WorkbookNameKind](#workbooknamekind)
WorkbookOpen   | Opens a Workbook at `.Path` with optional `.Password`
WorkbookProtection | Applies `.Value` of `.ProtectionKind` to Workbook | [WorkbookProtectionKind](#workbookprotectionkind)
WorksheetAdd   | Adds a Worksheet with `.Name` to current Workbook
WorksheetCopy  | Copies a Worksheet with `.Name` to a new Worksheet with `.NewName` in the current Workbook
WorksheetDelete| Deletes a Worksheet with `.Name` from the current Workbook
WorksheetGet   | Gets a Worksheet with `.Name` from the current Workbook
WorksheetMove  | Moves a Worksheet with `.Name` to the Worksheet with `.TargetName` in the current Workbook


## CellValidationKind
*Values for the CellValidation command*

Enum            | Description
---             | ---
Find            | Returns the first matching validation.
AnyValidation   | Adds a IExcelDataValidationAny to the worksheet.
CustomValidation | Adds a IExcelDataValidationCustom to the worksheet.
DateTimeValidation | Adds an IExcelDataValidationDateTime to the worksheet. The only accepted values are DateTime values.
DecimalValidation | Adds an IExcelDataValidationDecimal to the worksheet. The only accepted values are decimal values.
IntegerValidation | Adds an IExcelDataValidationInt to the worksheet. The only accepted values are integer values.
ListValidation  | Adds an IExcelDataValidationList to the worksheet. The accepted values are defined in a list.
TextLengthValidation | Adds an IExcelDataValidationInt regarding text length to the worksheet.
TimeValidation  | Adds an IExcelDataValidationTime to the worksheet. The only accepted values are Time values.


## CellValueKind
*Values for the CellValue command*

Enum            | Description
---             | ---
Value           | Set the range to a specific value
Text            | Returns the formatted value.
AutoFilter      | Set an autofilter for the range
AutoFitColumns  | Set the column width from the content of the range. The minimum width is the value of the ExcelWorksheet.defaultColumnWidth property. Note: Cells containing formulas must be calculated before autofit is called. Wrapped and merged cells are also ignored. (set-only)
Comment         | The comment text
Copy            | Copies the range of cells to an other range
DataValidation  | Data validation for this range (get-only)
Formula         | Gets or sets a formula for a range.
FormulaR1C1     | Gets or Set a formula in R1C1 format.
Hyperlink       | Set the hyperlink property for a range of cells
Merge           | If the cells in the range are merged.
RichText        | Add a rich text string
RichTextClear   | Clear the collection
StyleName       | The named style


## ColumnValueKind
*Values for the ColumnValue command*

Enum            | Description
---             | ---
AutoFit         | Set the column width from the content of the range. The minimum width is the value of the ExcelWorksheet.defaultColumnWidth property. Note: Cells containing formulas are ignored since EPPlus don't have a calculation engine. Wrapped and merged cells are also ignored. (set-only)
BestFit         | If set to true a column automaticlly resize(grow wider) when a user inputs numbers in a cell.
Merged          | none
Width           | Sets the width of the column in the worksheet
TrueWidth       | Set width to a scaled-value that should result in the nearest possible value to the true desired setting. (set-only)


## ConditionalFormattingKind
*Values for the ConditionalFormatting command*

Enum            | Description
---             | ---
AboveAverage    | Add AboveAverage Rule
AboveOrEqualAverage | Add AboveOrEqualAverage Rule
AboveStdDev     | Add AboveStdDev Rule
BeginsWith      | Add BeginsWith Rule
BelowAverage    | Add BelowAverage Rule
BelowOrEqualAverage | Add BelowOrEqualAverage Rule
BelowStdDev     | Add BelowStdDev Rule
Between         | Add Between Rule
Bottom          | Add Bottom Rule
BottomPercent   | Add BottomPercent Rule
ContainsBlanks  | Add ContainsBlanks Rule
ContainsErrors  | Add ContainsErrors Rule
ContainsText    | Add ContainsText Rule
Databar         | Adds a databar rule
DuplicateValues | Add DuplicateValues Rule
EndsWith        | Add EndsWith Rule
Equal           | Add Equal Rule
Expression      | Add Expression Rule
FiveIconSet     | Adds a FiveIconSet rule
FourIconSet     | Adds a FourIconSet rule
GreaterThan     | Add GreaterThan Rule
GreaterThanOrEqual | Add GreaterThanOrEqual Rule
Last7Days       | Add Last7Days Rule
LastMonth       | Add LastMonth Rule
LastWeek        | Add LastWeek Rule
LessThan        | Add LessThan Rule
LessThanOrEqual | Add LessThanOrEqual Rule
NextMonth       | Add NextMonth Rule
NextWeek        | Add NextWeek Rule
NotBetween      | Add NotBetween Rule
NotContainsBlanks | Add NotContainsBlanks Rule
NotContainsErrors | Add NotContainsErrors Rule
NotContainsText | Add NotContainsText Rule
NotEqual        | Add NotEqual Rule
ThisMonth       | Add ThisMonth Rule
ThisWeek        | Add ThisWeek Rule
ThreeColorScale | Add ThreeColorScale Rule
ThreeIconSet    | Add ThreeIconSet Rule
Today           | Add Today Rule
Tomorrow        | Add Tomorrow Rule
Top             | Add Top Rule
TopPercent      | Add TopPercent Rule
TwoColorScale   | Add TwoColorScale Rule
UniqueValues    | Add Unique Rule
Yesterday       | Add Yesterday Rule


## DrawingKind
*Values for the Drawing command*

Enum            | Description
---             | ---
AddChart        | Add a new chart to the worksheet. Does not support Bubble-, Radar-, Stock- or Surface charts.
AddPicture      | Add a picure to the worksheet
AddShape        | Add a new shape to the worksheet
Clear           | Removes all drawings from the collection
Remove          | Removes a drawing.


## ProtectionKind
*Values for the Protection command*

Enum            | Description
---             | ---
AllowFormatRows | Allow users to Format rows
AllowSort       | Allow users to sort a range
AllowDeleteRows | Allow users to delete rows
AllowDeleteColumns | Allow users to delete columns
AllowInsertHyperlinks | Allow users to insert hyperlinks
AllowInsertRows | Allow users to insert rows
AllowInsertColumns | Allow users to insert columns
AllowAutoFilter | Allow users to use autofilters
AllowPivotTables | Allow users to use pivottables
AllowFormatCells | Allow users to format cells
AllowEditScenarios | Allow users to edit senarios
AllowEditObject | Allow users to edit objects
AllowSelectUnlockedCells | Allow users to select unlocked cells
AllowSelectLockedCells | Allow users to select locked cells
IsProtected    | If the worksheet is protected.
AllowFormatColumns | Allow users to Format columns
SetPassword    | Sets a password for the sheet.


## RowValueKind
*Values for the RowValue command*

Enum            | Description
---             | ---
Collapsed       | If outline level is set this tells that the row is collapsed
CustomHeight    | Set to true if You do not want the row to Autosize
Height          | Sets the height of the row
Hidden          | Allows the row to be hidden in the worksheet
Merged          | Sets the merged row
OutlineLevel    | Outline level.
PageBreak       | Adds a manual page break after the row.
Phonetic        | Show phonetic Information
StyleName       | Sets the style for the entire column using a style name.


## VbaCode
*Values for the VbaCodeModule and VbaModule command*

Class           | Type      | Description
---             | ---       | ---
Name            | string    | The name of the module
Description     | string    | A description of the module
Code            | string    | The code without any module level attributes. Can contain function level attributes.
ReadOnly        | bool?     | If the module is readonly
Private         | bool?     | If the module is private


## VbaLibrary
*Values for the VbaReference command*

Class           | Type      | Description
---             | ---       | ---
Name            | string    | The name of the reference
Libid           | LibraryId | LibID For more info check VbaLibrary.LibraryId


## VbaModuleKind
*Values for the VbaCodeModule command*

Enum            | Description
---             | ---
CodeModule      | Sets the Workbook VBA Module
Module          | Adds a new VBA Module
Class           | Adds a new VBA public class
PrivateClass    | Adds a new VBA private class


## ViewActionKind
*Values for the ViewAction command*

Enum            | Description
---             | ---
FreezePane      | Freeze the columns/rows to left and above the cell
SetTabSelected  | Sets whether the worksheet is selected within the workbook.
UnfreezePane    | Unlock all rows and columns to scroll freely


## WorkbookNameKind
*Values for the WorkbookName command*

Enum            | Description
---             | ---
Add             | Add a new named range
AddFormula      | Sets whether the worksheet is selected within the workbook.
AddValue        | Add a defined name referencing value
Remove          | Remove a defined name from the collection


## WorkbookProtectionKind
*Values for the WorkbookProtection command*

Enum            | Description
---             | ---
LockStructure   | Locks the structure, which prevents users from adding or deleting worksheets or from displaying hidden worksheets.
LockWindows     | Locks the position of the workbook window.
LockRevision    | Lock the workbook for revision
SetPassword     | Sets a password for the workbook. This does not encrypt the workbook.


# Parsing

Formats for parsing string values.

## Conditionals
*Values for the ConditionalFormatting command*


## Drawing
*Values for the Drawing command*


## Styles
*Values for the CellStyle command*

`n*`| The numberformat  | Description
--- | ---:              | ---
n:* | *Format*          | Set the range to a specific value
n$* | *NumberformatPrec* | Set the range to a specific value
n%* | *NumberformatPrec* | Set the range to a specific value
n,* | *NumberformatPrec* | Set the range to a specific value
nd  | ShortDatePattern  | Set the range to a specific value

`f*`| Font styling      | Description
--- | ---:              | ---
f:* | *Font*            | The name of the font (style-only)
fx* | *Size*            | The Size of the font (style-only)
ff* | *Family*          | Font family (style-only)
fc* | *Color*           | Cell color (style-only)
fs* | *Scheme*          | Scheme (style-only)
fB  | true              | Font-bold
fb  | false             | Font-bold
fI  | true              | Font-italic
fi  | false             | Font-italic
fS  | true              | Font-Strikeout
fs  | false             | Font-Strikeout
fU  | true              | Font-Underline
fu  | false             | Font-Underline
fu:* | *FontUnderline*  | Font-Underline Type
fv* | *VerticalAlignmentFont* | Font-Vertical Align (style-only)

`l*`| Fill styling      | Description
--- | ---:              | ---
lc* | *Color*           | The background color
lf* | *FillStyle*       | The pattern for solid fills.

`b*`| Border            | Description
--- | ---:              | ---
bl* | *BorderStyle*     | Left border style
br* | *BorderStyle*     | Right border style
bt* | *BorderStyle*     | Top border style
bb* | *BorderStyle*     | Bottom border style
bdU | true              | A diagonal from the bottom left to top right of the cell (style-only)
bdu | false             | A diagonal from the bottom left to top right of the cell (style-only)
bdD | true              | A diagonal from the top left to bottom right of the cell (style-only)
bdd | false             | A diagonal from the top left to bottom right of the cell (style-only)
bd* | *BorderStyle*     | Diagonal border style (style-only)
ba* | *BorderStyle*     | Set the border style around the range. (style-only)

`ha*`| Horizontal alignment | Description
--- | ---:              | ---
ha* | *HorizontalAlignment* | The horizontal alignment in the cell (style-only)

`va*`| Vertical alignment | Description
--- | ---:              | ---
va* | *VerticalAlignment* | The vertical alignment in the cell (style-only)

`*` | Style             | Description
--- | ---:              | ---
W   | true              | Wrap the text (style-only)
w   | false             | Wrap the text (style-only)


## ValidationRules
*Values for the CellValidation command*

`_` | Base              | Description
--- | ---:              | ---
_   | AllowBlank:true   | Input allowed to be blank
.   | AllowBlank:false  | Input not allowed to be blank
I   | ShowInputMessage:true | Input message should be shown
i   | ShowInputMessage:false | Input message should not be shown
E   | ShowErrorMessage:true | Error message should be shown
e   | ShowErrorMessage:false | Error message should not be shown
et:* | ErrorTitle       | Title of error message box
e:* | Error             | Error message box text
pt:* | PromptTitle      | Title of info box if input message should be shown
p:* | Prompt            | Info message text

`e` | ErrorStyle        | Description
--- | ---:              | ---
undefined | Undefined   | Warning style will be excluded
stop | Stop             | Stop warning style, invalid changes will not be accepted
warning | Earning       | Warning will be presented when an attempt to an invalid change is done, but the change will be accepted
information | Information | Information warning style

`o` | Operator          | Description
--- | ---:              | ---
\>< | Between           | Operator type (or ..)
<>  | NotBetween        | Operator type (or !.)
=   | Equal             | Operator type (or ==)
!=  | NotEqual          | Operator type
<   | LessThan          | Operator type
<=  | LessThanOrEqual   | Operator type
\>  | GreaterThan       | Operator type
\>= | GreaterThanOrEqual | Operator type

`f` | ExcelFormula      | Description
--- | ---:              | ---
f:* | *Formula.ExcelFormula* | An excel formula
f2:* | *Formula2.ExcelFormula* | An excel formula

`v` | Value             | Description
--- | ---:              | ---
v:* | *Formula.Value*   | The value
v2:* | *Formula2.Value* | The value


# Reference - Advanced

## CommandRtn
*Values for the return value of Commands*

Flags           | Description
---             | ---
Normal          | Normal operations.
Formula         | Value should be considered a formula.
Continue        | Continue to the next row.
SkipCmds        | Skip processing the attached commands.


## When
*Values for the When fields of Commands*

Flags           | Description
---             | ---
FirstSet        | Execute on the first set in PushSet.
First           | Execute first before rows.
Normal          | Execute before normal writing.
AfterNormal     | Execute after normal writing.
Last            | Execute last after rows.
LastSet         | Execute on the last set in PushSet.


## IExcelContext
*Values for the When fields*

Class           | Type          | Description
---             | ---           | ---
XStart          | int           | Gets or sets where the cursor X starts per row.
X               | int           | Gets or sets the cursor X coordinate.
Y               | int           | Gets or sets the cursor Y coordinate.
DeltaX          | int           | Gets or sets the amount the cursor X advances.
DeltaY          | int           | Gets or sets the amount the cursor Y advances.
CsvX            | int           | Gets or sets the cursor CsvX coordinate, advances with X.
CsvY            | int           | Gets or sets the cursor CsvY coordinate, advances with Y.
NextDirection   | *NextDirection* | Gets or sets the next direction.
CmdRows         | *Stack`<CommandRow>`* | Gets the stack of commands per row.
CmdCols         | *Stack`<CommandCol>`* | Gets the stack of commands per column.
Sets            | *Stack`<IExcelSet>`* | Gets the stack of sets.
Frames          | *Stack`<object>`* | Gets the stack of frames.
Frame           | object        | Gets the current frame.
Flush           | void          | Flushes all pending commands.
Get(cells)      | *ExcelRangeBase* | Gets the specified range.
Next(range)     | *ExcelRangeBase* | Advances the cursor based on NextDirection.
Next(column)    | *ExcelColumn*   | Advances the cursor to the next row.
Next(row)       | *ExcelRow*      | Advances the cursor to the next row.


# Author

The author of this library is [Sky Morey](https://www.linkedin.com/in/sky-morey/).
