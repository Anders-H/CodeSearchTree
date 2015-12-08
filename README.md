# CodeSearchTree
Simple Roslyn based code analysis library.

CodeSearchTree.dll uses Roslyn but does not require that your program references Roslyn.

## Search expressions

A search expression is a slash separated list of nodes. To get from a namespace to a method via a class, type:

`namespace/class/method`

Or:

`ns/cls/method`

If your namespace contains several classes, and your classes contains several methods, you can add an index. This will give you the third (index 2) method in the second (index 1) class.

`ns/cls[1]/method[2]`

You can also specify names instead of indexes. This will give you the method called `MyFunction` in the second class:

`ns/cls[1]/method[MyFunction]`

The node type can be unspecified. This will give you anything named `MyFunction` in the first class in the namespace.

`ns/cls/*[MyFunction]`

And this will deliver the first node with two parent nodes, regardless of name and type:

`*/*/*`

Attribute filter:

`ns/cls/method[@MyAttribute]`

Return type filter:

`ns/cls/property[#string]`

## C# Examples

Get class node from file:

```C#
var code_tree = CodeSearchTree.Node.CreateTreeFromFile(filename);
var my_class = code_tree.GetChild("ns/cls");
```

Acquire a child node, regardless of type, named `Main` and display its source.

```C#
var main = my_class.GetChild("*[Main]");
Console.WriteLine(main.Source);
```

Search for file that contains node a class named `MyClass`:

```C#
var result = CodeSearchTree.FileSystem.CreateTreesFromFolder(foldername, "*/cls[MyClass]");
if (result.Count > 0)
   Console.WriteLine("Success!");
```

Find the first test method in a file:

```C#
var code_tree = CodeSearchTree.Node.CreateTreeFromFile(filename);
var my_testmethod = code_tree.DeepSearch("method[@TestMethod]").FirstOrDefault();
Console.WriteLine(my_testmethod.Source);
```

Given that `my_class` references to a class, find property or method in a class that returns an `int`:

```C#
var x = my_class.GetChild("*[#int]");
```
